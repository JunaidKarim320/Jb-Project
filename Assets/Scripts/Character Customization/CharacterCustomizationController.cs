using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizationController : MonoBehaviour
{
    
    #region Instance
 
    private static CharacterCustomizationController _instance;

    public static CharacterCustomizationController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterCustomizationController>();
            }

            return _instance;
        }
    }
    
    #endregion
    
    public enum CharacterCustomizationPart
    {
        Head,
        Face,
        Body,
        Torso
    }

    public CharacterCustomizationPart currentPart;
    public int currentsubPart;
    
    public CharacterCustomizationCategory CurrentCustomizationCategory;
    
    public List<CharacterCustomizationCategory> CustomizationCategories;

    public Transform m_ItemBtnParent;
    public GameObject m_ItemBtnPrefab;

    public Transform m_CategoryBtnParent;
    public GameObject m_CategoryBtnPrefab;

    public TextMeshProUGUI PriceText;
    public Button Buy;
    private List<InventoryItemBtn> m_InventoryItemBtns;
    private List<CustomizationCategoryBtn> m_InventoryCategoryBtns;
    private GameObject m_CurrentItem;
    private int tempSelectedItem;
    private int j,itemsLength,customizationItemButtonsLength;
    private int tempcustomtype;

    private MainMenuController _mainMenuController;
    private CharacterCustomizationHandler _customizationHandler;
    private DataController _dataController;
    private LoadingController _loadingController;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _mainMenuController = MainMenuController.instance;
        _dataController = DataController.instance;
        _loadingController = LoadingController.instance;

        m_InventoryItemBtns = new List<InventoryItemBtn>();
        m_InventoryCategoryBtns = new List<CustomizationCategoryBtn>();

    }

    public void Display()
    {
        
        _mainMenuController.AddPanelToStackAndLoad(10);
        //Character.SetActive(true);
        _customizationHandler = _mainMenuController.Character.GetComponentInChildren<CharacterCustomizationHandler>();
        
        if (_customizationHandler)
        {
            InitiateCustomization();
            SelectCategory(0,true);
        }
    }
    
    public void DisplayGameShop()
        {
          Invoke("GameShop",.25f);
        }

    void GameShop()
    {
        _customizationHandler = InGameShopController.instance.CharacterParent.GetComponent<CharacterCustomizationHandler>();
            
        if (_customizationHandler)
        {
            InitiateCustomization();
            SelectCategory(0,true);
        }
    }

    public void PlayGameShop()
        {
            InGameShopController.instance.ExitClothShop();
        }
    public void InitiateCustomization()
    {
        
        for (int k = 0; k < CustomizationCategories.Count; k++)
        {
            CustomizationCategories[k].m_CustomizationCategory = new List<CharacterCustomizationSubCategory>();
            
            CustomizationCategories[k].m_CustomizationCategory = _customizationHandler.CustomizationCategories[k].m_CustomizationCategory;
            
            for (int z = 0; z < CustomizationCategories[k].m_CustomizationCategory.Count; z++)
            {
                // CustomizationCategories[k].m_CustomizationCategory[z].m_CustomizationItemInfo = new List<CustomizationItemInfo>();
                // CustomizationCategories[k].m_CustomizationCategory[z].m_Items = new List<GameObject>();
                
                CustomizationCategories[k].m_CustomizationCategory[z].m_CustomizationItemInfo = _customizationHandler.CustomizationCategories[k].m_CustomizationCategory[z].m_CustomizationItemInfo;
                CustomizationCategories[k].m_CustomizationCategory[z].m_Items = _customizationHandler.CustomizationCategories[k].m_CustomizationCategory[z].m_Items;
                CustomizationCategories[k].m_CustomizationCategory[z].Icon = _customizationHandler.CustomizationCategories[k].m_CustomizationCategory[z].Icon;
                
                if (CustomizationCategories[k].m_CustomizationCategory[z].m_CustomizationItemInfo.Count > 0)
                {
                    CustomizationCategories[k].selectedImage.transform.parent.GetComponent<Button>().interactable = true;
                }
                else
                {
                    CustomizationCategories[k].selectedImage.transform.parent.GetComponent<Button>().interactable = false;
                }
            }
        } 
    }

    public void SelectCategoryBtn(int i)
    {
        SelectCategory(i,false);
    }
 
    
    public void SelectCategory(int i, bool shouldCheckInit)
    {
        currentPart = (CharacterCustomizationPart)i;
        //ApplicationController.SelectedCustomizationType =  (int)CurrentCustomizationType;
       
        CurrentCustomizationCategory = CustomizationCategories[i];

        tempcustomtype = i;

        if (shouldCheckInit)
        {
            if (CurrentCustomizationCategory.m_CustomizationCategory[i].m_Items.Count > 0)
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
        for (int k = 0; k < CustomizationCategories.Count; k++)
        {
            CustomizationCategories[k].selectedImage.SetActive(false);
        }
        
        CustomizationCategories[i].selectedImage.SetActive(true);
        
        if (m_InventoryCategoryBtns.Count > 0)
        {
            for (int k = 0; k < m_InventoryCategoryBtns.Count; k++)
            {
                Destroy(m_InventoryCategoryBtns[k].gameObject);
            }
            m_InventoryCategoryBtns.Clear();
        }

        
        for (int x = 0; x < CurrentCustomizationCategory.m_CustomizationCategory.Count; x++)
        {
            GameObject Category = Instantiate(m_CategoryBtnPrefab, m_CategoryBtnParent);
            CustomizationCategoryBtn Categorybtn = Category.GetComponent<CustomizationCategoryBtn>();
            
                Categorybtn.Icon.sprite = CurrentCustomizationCategory.m_CustomizationCategory[x].Icon;
                Categorybtn.SelectedIcon.sprite = CurrentCustomizationCategory.m_CustomizationCategory[x].Icon;
                Categorybtn.selectedImage.SetActive(false);
                Categorybtn.lockButton.SetActive(false);
                Categorybtn.CategoryText.text = CurrentCustomizationCategory.m_CustomizationCategory[x].m_Name;
                
            
            Categorybtn.Id = x;
            
            Category.GetComponent<Button>().onClick.AddListener(() => SelectSubCategory(Categorybtn.Id));
            m_InventoryCategoryBtns.Add(Categorybtn);
        }

        SelectSubCategory(0);
    }

    public void SelectSubCategory(int i)
    {
        for (int j = 0; j < m_InventoryCategoryBtns.Count; j++)
        {
            m_InventoryCategoryBtns[j].selectedImage.SetActive(false);
        }
        
        m_InventoryCategoryBtns[i].selectedImage.SetActive(true);
        currentsubPart = i;
        InitializeCategoryItem(i);
    }

    
    
    public void InitializeCategoryItem(int i)
    {
        if (m_InventoryItemBtns.Count > 0)
        {
            for (int k = 0; k < m_InventoryItemBtns.Count; k++)
            {
                Destroy(m_InventoryItemBtns[k].gameObject);
            }

            m_InventoryItemBtns.Clear();
        }

        for (int k = 0; k < CurrentCustomizationCategory.m_CustomizationCategory[i].m_Items.Count; k++)
        {
            GameObject item = Instantiate(m_ItemBtnPrefab, m_ItemBtnParent);
            InventoryItemBtn inventoryItemBtn = item.GetComponent<InventoryItemBtn>();
        
            if (CurrentCustomizationCategory.m_CustomizationCategory[i].m_CustomizationItemInfo[k].Icon)
                inventoryItemBtn.Icon.sprite = CurrentCustomizationCategory.m_CustomizationCategory[i]
                    .m_CustomizationItemInfo[k].Icon;
        
            inventoryItemBtn.Id = k;
            item.GetComponent<Button>().onClick.AddListener(() => SelectCustomizationItem(inventoryItemBtn.Id));
            m_InventoryItemBtns.Add(inventoryItemBtn);
        
        }
        
        
        UnlockingInventoryItem(i);
        
        if (currentPart == CharacterCustomizationPart.Head)
        {
            SortIndexForLastSelected(ApplicationController.SelectedHeadItem);
        }
         if (currentPart == CharacterCustomizationPart.Body)
        {
            SortIndexForLastSelected(ApplicationController.SelectedBodyItem);
        }
         if (currentPart == CharacterCustomizationPart.Torso)
        {
            SortIndexForLastSelected(ApplicationController.SelectedTorsoItem);
        }
         if (currentPart == CharacterCustomizationPart.Face)
        {
            if (currentsubPart == 0)
            {
                SelectCustomizationItem(ApplicationController.SelectedGlassesItem);
            }
             if (currentsubPart == 1)
            {
                SelectCustomizationItem(ApplicationController.SelectedMostacheItem);
            }
             if (currentsubPart == 2)
            {
                SelectCustomizationItem(ApplicationController.SelectedBeardItem);
            }
        }
       
    }


    public void SortIndexForLastSelected(int i )
    {
        if (i > m_InventoryItemBtns.Count)
        {
            SelectCustomizationItem(0);
        }
        else
        {
            SelectCustomizationItem(i);
        }
    }

    public void UnlockInventoryItem()
    {
        _dataController.SetUnlockInventoryItem((int) currentPart,currentsubPart, tempSelectedItem);
        _dataController.RemoveCoins(CurrentCustomizationCategory.m_CustomizationCategory[currentsubPart].m_CustomizationItemInfo[tempSelectedItem].price);
        m_InventoryItemBtns[tempSelectedItem].lockButton.SetActive(false);
       
        SetSelectedItem();
    }
    public void UnlockInventoryItemRewarded()
    {
        _dataController.SetUnlockInventoryItem((int) currentPart,currentsubPart, tempSelectedItem);
        m_InventoryItemBtns[tempSelectedItem].lockButton.SetActive(false);
        SetSelectedItem();
    }
    public void BuyInventoryItem()
    {
        if (_dataController.coins >= CurrentCustomizationCategory.m_CustomizationCategory[currentsubPart].m_CustomizationItemInfo[tempSelectedItem].price)
        {
            UnlockInventoryItem();
            Buy.gameObject.SetActive(false);
        }
        else
        {
            //ApplicationController._RewardBtnValue = RewardBtn.CharacterCustomization;
            _mainMenuController.AddPanelToStackAndLoad(8);
        }
    }
    
    public void UnlockingInventoryItem(int j)
    {
        customizationItemButtonsLength = m_InventoryItemBtns.Count;

        if (customizationItemButtonsLength <= 0) return;

        for (int i = 0; i < customizationItemButtonsLength; i++)
        {
            if (_dataController.GetUnlockInventoryItem((int)currentPart,currentsubPart,i))
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
        if (currentPart == CharacterCustomizationPart.Head)
        {
            _customizationHandler.SetLastSelectedHead(true);
            _customizationHandler.SetLastSelectedBody(false);
            _customizationHandler.SetLastSelectedGlasses(false);
            _customizationHandler.SetLastSelectedMostaches(false);
            _customizationHandler.SetLastSelectedBeard(false);
            _customizationHandler.SetLastSelectedTorso(false);
        }
         if (currentPart == CharacterCustomizationPart.Body)
        {
            _customizationHandler.SetLastSelectedBody(true);
            _customizationHandler.SetLastSelectedHead(false);
            _customizationHandler.SetLastSelectedGlasses(false);
            _customizationHandler.SetLastSelectedMostaches(false);
            _customizationHandler.SetLastSelectedBeard(false);
            _customizationHandler.SetLastSelectedTorso(false);
        }
         if (currentPart == CharacterCustomizationPart.Torso)
        {
            _customizationHandler.SetLastSelectedTorso(true);
            _customizationHandler.SetLastSelectedBody(false);
            _customizationHandler.SetLastSelectedGlasses(false);
            _customizationHandler.SetLastSelectedMostaches(false);
            _customizationHandler.SetLastSelectedBeard(false);
            _customizationHandler.SetLastSelectedHead(false);
        }
         
         if (currentPart == CharacterCustomizationPart.Face)
        {
            if (currentsubPart == 0)
            {
                _customizationHandler.SetLastSelectedGlasses(true);
            }
            if (currentsubPart == 1)
            {
                _customizationHandler.SetLastSelectedMostaches(true);
            }
            if (currentsubPart == 2)
            {
                _customizationHandler.SetLastSelectedBeard(true);
            }
            _customizationHandler.SetLastSelectedHead(false);
            _customizationHandler.SetLastSelectedBody(false);
            _customizationHandler.SetLastSelectedTorso(false);
        }
         
    }
    public void SelectCustomizationItem(int i)
    {
       
       // if (m_CurrentItem)
          //  m_CurrentItem.SetActive(false);
        
        
        
        ResetCustomization();
        
        tempSelectedItem = i;
        
       // print($"{currentsubPart}{tempSelectedItem}");
        m_CurrentItem = CurrentCustomizationCategory.m_CustomizationCategory[currentsubPart].m_Items[tempSelectedItem];

        if (m_CurrentItem)
        m_CurrentItem.SetActive(true);

        for (int k = 0; k < customizationItemButtonsLength; k++)
        {
            m_InventoryItemBtns[k].selectedImage.SetActive(false);
        }
        m_InventoryItemBtns[tempSelectedItem].selectedImage.SetActive(true);
         
        
        if (_dataController.GetUnlockInventoryItem((int)currentPart,currentsubPart,tempSelectedItem))
        {
            Buy.gameObject.SetActive(false);

            SetSelectedItem();

        }
        else
        {
            Buy.gameObject.SetActive(true);
            PriceText.text= CurrentCustomizationCategory.m_CustomizationCategory[currentsubPart].m_CustomizationItemInfo[tempSelectedItem].price.ToString();
        }
        
    }


    public void SetSelectedItem()
    {
        // if(CurrentCustomizationCategory.m_CustomizationCategory[currentsubPart].m_CustomizationItemInfo[tempSelectedItem].Disabled)
        //     return;
        
        if (currentPart == CharacterCustomizationPart.Head)
        {
            ApplicationController.SelectedHeadItem = tempSelectedItem;
        }
         if (currentPart == CharacterCustomizationPart.Body)
        {
            ApplicationController.SelectedBodyItem = tempSelectedItem;
        }
         if (currentPart == CharacterCustomizationPart.Torso)
        {
            ApplicationController.SelectedTorsoItem = tempSelectedItem;
        }
         if (currentPart == CharacterCustomizationPart.Face)
        {
            if (currentsubPart == 0)
            {
                ApplicationController.SelectedGlassesItem = tempSelectedItem;
            }
             if (currentsubPart == 1)
            {
                ApplicationController.SelectedMostacheItem = tempSelectedItem;
            }
             if (currentsubPart == 2)
            {
                ApplicationController.SelectedBeardItem = tempSelectedItem;
            }
        }
    }
    public void ResetSelection()
    {
        _customizationHandler.SetLastSelectedCustomization();
    }
    
    public void Play()
    {
        _mainMenuController.Character.SetActive(false);
        _loadingController.display(Scenes.Gameplay);
    }
}

[Serializable]
public class CharacterCustomizationCategory
{
    public string m_Name;
    public GameObject selectedImage;
    public List<CharacterCustomizationSubCategory> m_CustomizationCategory ;

}


[Serializable]
public class CharacterCustomizationSubCategory
{
    public string m_Name;
    public GameObject selectedImage;
    public Sprite Icon;
    public List<CustomizationItemInfo> m_CustomizationItemInfo;
    public List<GameObject> m_Items;
}
