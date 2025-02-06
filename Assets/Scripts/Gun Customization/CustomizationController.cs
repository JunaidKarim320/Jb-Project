using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationController : MonoBehaviour
{
    #region Instance
 
    private static CustomizationController _instance;

    public static CustomizationController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CustomizationController>();
            }

            return _instance;
        }
    }
    
    #endregion
    public enum CustomizationType
    {
        Grip,
        Lazer,
        Muzzle,
        Scope
    }

    public CustomizationType CurrentCustomizationType;
    public CustomizationCategory CurrentCustomizationCategory;
    public CustomizationCategory[] CustomizationCategories;
    
    public Transform m_ItemBtnParent;
    public GameObject m_ItemBtnPrefab;

    public TextMeshProUGUI PriceText;
    public Button Buy;

    private List<InventoryItemBtn> m_InventoryItemBtns;
    private GameObject m_CurrentItem;
    
    private int tempcustomtype;
    private int tempSelectedItem;
    private int j,itemsLength,customizationItemButtonsLength;
    
    private MainMenuController _mainMenuController;
    private DataController _dataController;
    private InventoryController _inventoryController;
    private CustomizationHandler _customizationHandler;
    private InGameShopController _inGameShopController;

    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_InventoryItemBtns = new List<InventoryItemBtn>();
        _mainMenuController = MainMenuController.instance;
        _dataController = DataController.instance;
        _inventoryController = InventoryController.instance;
        _inGameShopController = FindObjectOfType<InGameShopController>();

    }

   

    public void Display()
    {
        _mainMenuController.AddPanelToStackAndLoad(9);
        
        _customizationHandler = _inventoryController.m_CurrentItem.GetComponentInChildren<CustomizationHandler>();
        
        if (_customizationHandler)
        {
            InitiateCustomization();
            SelectCategory(0,true);
        }
    }
    public void DisplayGameShop()
    {
        _customizationHandler = _inventoryController.m_CurrentItem.GetComponentInChildren<CustomizationHandler>();
        
        if (_customizationHandler)
        {
            InitiateCustomization();
            SelectCategory(0,true);
        }
    }
    
    
    
    public void InitiateCustomization()
    {
        for (int k = 0; k < CustomizationCategories.Length; k++)
        {
            CustomizationCategories[k].m_CustomizationItemInfo = new List<CustomizationItemInfo>();
            CustomizationCategories[k].m_Items = new List<GameObject>();
            CustomizationCategories[k].m_CustomizationItemInfo = _customizationHandler.CustomizationCategories[k].m_CustomizationItemInfo;
            CustomizationCategories[k].m_Items = _customizationHandler.CustomizationCategories[k].m_Items;
            if (CustomizationCategories[k].m_CustomizationItemInfo.Count > 0)
            {
                CustomizationCategories[k].selectedImage.transform.parent.GetComponent<Button>().interactable = true;
                CustomizationCategories[k].LockImage.SetActive(false);
            }
            else
            {
                CustomizationCategories[k].selectedImage.transform.parent.GetComponent<Button>().interactable = false;
                CustomizationCategories[k].LockImage.SetActive(true);
            }
        } 
    }

    public void SelectCategoryBtn(int i)
    {
        SelectCategory(i,false);
    }

    public void SelectCategory(int i, bool shouldCheckInit)
    {
        CurrentCustomizationType = (CustomizationType)i;
      
    //    ApplicationController.SelectedCustomizationType =  (int)CurrentCustomizationType;
        
        //CustomizationCategories = _customizationHandler.CustomizationCategories;
        CurrentCustomizationCategory = CustomizationCategories[i];

        tempcustomtype = i;

        if (shouldCheckInit)
        {
            if (CurrentCustomizationCategory.m_Items.Count > 0)
            {
                InitializeCategory(i);
            }
            else
            {
                SelectCategory(tempcustomtype + 1, true);
            }
        }
        else
        {
            InitializeCategory(i);
        }
    }
    
    public void InitializeCategory(int i)
    {
        for (int k = 0; k < CustomizationCategories.Length; k++)
        {
            CustomizationCategories[k].selectedImage.SetActive(false);
        }
        
        CustomizationCategories[i].selectedImage.SetActive(true);

        for (int k = 0; k < m_InventoryItemBtns.Count; k++)
        {
            Destroy(m_InventoryItemBtns[k].gameObject);
        }
        
        if(m_InventoryItemBtns.Count>0)
            m_InventoryItemBtns.Clear();


        for (int k = 0; k < CurrentCustomizationCategory.m_Items.Count; k++)
        {
            GameObject item = Instantiate(m_ItemBtnPrefab,m_ItemBtnParent);
            InventoryItemBtn inventoryItemBtn = item.GetComponent<InventoryItemBtn>();
            
            if(CurrentCustomizationCategory.m_CustomizationItemInfo[k].Icon)
                inventoryItemBtn.Icon.sprite = CurrentCustomizationCategory.m_CustomizationItemInfo[k].Icon;

            inventoryItemBtn.Id = k;
            item.GetComponent<Button>().onClick.AddListener(() => SelectCustomizationItem(inventoryItemBtn.Id));
            m_InventoryItemBtns.Add(inventoryItemBtn);
           
        }
        
        UnlockingInventoryItem(i);

        if (CurrentCustomizationType == CustomizationType.Grip)
        {
            SelectCustomizationItem(ApplicationController.SelectedGripItem);
        }
        if (CurrentCustomizationType == CustomizationType.Lazer)
        {
            SelectCustomizationItem(ApplicationController.SelectedLazerItem);
        }
        if (CurrentCustomizationType == CustomizationType.Muzzle)
        {
            SelectCustomizationItem(ApplicationController.SelectedMuzzleItem);
        }
        if (CurrentCustomizationType == CustomizationType.Scope)
        {
            SelectCustomizationItem(ApplicationController.SelectedScopeItem);
        }
    }
    
    public void UnlockInventoryItem()
    {
        _dataController.SetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,ApplicationController.SelectedInventoryItem,(int)CurrentCustomizationType,tempSelectedItem);
        _dataController.RemoveCoins(CurrentCustomizationCategory.m_CustomizationItemInfo[tempSelectedItem].price);
        m_InventoryItemBtns[tempSelectedItem].lockButton.SetActive(false);
        SetSelectedItem();
    }
    public void UnlockInventoryItemRewarded()
    {
        _dataController.SetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,ApplicationController.SelectedInventoryItem,(int)CurrentCustomizationType,tempSelectedItem);
        m_InventoryItemBtns[tempSelectedItem].lockButton.SetActive(false);
        SetSelectedItem();
    }
    public void BuyInventoryItem()
    {
        if (_dataController.coins >= CurrentCustomizationCategory.m_CustomizationItemInfo[tempSelectedItem].price)
        {
            UnlockInventoryItem();
            Buy.gameObject.SetActive(false);
        }
        else
        {
            //ApplicationController._RewardBtnValue = RewardBtn.GunCustomization;
            _mainMenuController.AddPanelToStackAndLoad(8);

        }
    }
 
    
    public void UnlockingInventoryItem(int j)
    {
        customizationItemButtonsLength = m_InventoryItemBtns.Count;

        if (customizationItemButtonsLength <= 0) return;

        for (int i = 0; i < customizationItemButtonsLength; i++)
        {
            if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,ApplicationController.SelectedInventoryItem,(int)CurrentCustomizationType,i))
            {
                m_InventoryItemBtns[i].lockButton.SetActive(false);
            }
            else
            {
                m_InventoryItemBtns[i].lockButton.SetActive(true);
            }
        }
    }

    public void ResetCustomization()
    {
        if (CurrentCustomizationType == CustomizationType.Grip)
        {
            _customizationHandler.SetLastSelectedGrip(true);
            _customizationHandler.SetLastSelectedLazer(false);
            _customizationHandler.SetLastSelectedMuzzle(false);
            _customizationHandler.SetLastSelectedScope(false);
        }
        
        if (CurrentCustomizationType == CustomizationType.Lazer)
        {
            _customizationHandler.SetLastSelectedGrip(false);
            _customizationHandler.SetLastSelectedMuzzle(false);
            _customizationHandler.SetLastSelectedScope(false);
            _customizationHandler.SetLastSelectedLazer(true);
        }
        
         if (CurrentCustomizationType == CustomizationType.Muzzle)
        {
            _customizationHandler.SetLastSelectedMuzzle(true);
            _customizationHandler.SetLastSelectedLazer(false);
            _customizationHandler.SetLastSelectedGrip(false);
            _customizationHandler.SetLastSelectedScope(false); 
        }
         
         if (CurrentCustomizationType == CustomizationType.Scope)
        {
            _customizationHandler.SetLastSelectedScope(true);
            _customizationHandler.SetLastSelectedLazer(false);
            _customizationHandler.SetLastSelectedMuzzle(false);
            _customizationHandler.SetLastSelectedGrip(false);
        }
    }

    public void SelectCustomizationItem(int i)
    {
        if(m_CurrentItem)
            m_CurrentItem.SetActive(false);
        
        tempSelectedItem = i;
        
        ResetCustomization();

        m_CurrentItem = CurrentCustomizationCategory.m_Items[tempSelectedItem];

        m_CurrentItem.SetActive(true);

        for (i = 0; i < customizationItemButtonsLength; i++)
        {
            m_InventoryItemBtns[i].selectedImage.SetActive(false);
        }
        m_InventoryItemBtns[tempSelectedItem].selectedImage.SetActive(true);
        
        if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,ApplicationController.SelectedInventoryItem,(int)CurrentCustomizationType,tempSelectedItem))
        {
            SetSelectedItem();
            Buy.gameObject.SetActive(false);
        }
        else
        {
            Buy.gameObject.SetActive(true);
            PriceText.text= CurrentCustomizationCategory.m_CustomizationItemInfo[tempSelectedItem].price.ToString();
        }
    }


    public void SetSelectedItem()
    {
        if (CurrentCustomizationType == CustomizationType.Grip)
        {
           ApplicationController.SelectedGripItem= tempSelectedItem;
        }
        if (CurrentCustomizationType == CustomizationType.Lazer)
        {
            ApplicationController.SelectedLazerItem= tempSelectedItem;
        }
        if (CurrentCustomizationType == CustomizationType.Muzzle)
        {
            ApplicationController.SelectedMuzzleItem= tempSelectedItem;
        }
        if (CurrentCustomizationType == CustomizationType.Scope)
        {
           ApplicationController.SelectedScopeItem= tempSelectedItem;
        }
    }


    public void ResetSelection()
    {
        if(_customizationHandler)
        _customizationHandler.SetLastSelectedCustomization();
    }

}


[Serializable]

public struct CustomizationCategory
{
    public string m_Name;
    public GameObject selectedImage;
    public GameObject LockImage;
    public List<CustomizationItemInfo> m_CustomizationItemInfo;
    public List<GameObject> m_Items;
} 