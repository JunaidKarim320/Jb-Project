using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Invector.vItemManager;

//using JUTPS.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{

    #region Instance

    private static InventoryController _instance;

    public static InventoryController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryController>();
            }

            return _instance;
        }
    }

    #endregion

    public InventoryItem[] InventoryItems;
    public enum Categories
    {
        Melee,
        HandGun,
        SemiAuto,
        Sniper,
        Grenade
    }

    public Categories currentCategory;
    //[HideInInspector]
    public InventoryCategory CurrentInventoryCategory;

    public InventoryCategory[] ItemsCategories;

    public TextMeshProUGUI NameofWeapon;

    public Image Damage;
    public Image Range;
    public Image Recoil;
    public Image Magazine;
    public Image RateOfFire;

    public Button Buy;

    public TextMeshProUGUI PriceText;

    public Transform m_ItemBtnParent;
    public GameObject m_ItemBtnPrefab;
    public GameObject m_CustomizationBtn;
    public Scrollbar _InventorySlider;
    int i, tempSelectedItem;

    int inventoryItemButtonsLength;

    private List<InventoryItemBtn> m_InventoryItemBtns;

    [HideInInspector]
    public GameObject m_CurrentItem;


    //public JUInventory _juInventory;
    public vItemManager _vItemManager;

    private MainMenuController _mainMenuController;
    private DataController _dataController;
    private LoadingController _loadingController;
    private CharacterCustomizationController _characterCustomizationController;
    private InGameShopController _inGameShopController;

    private void Awake()
    {
        _instance = this;
    }


    private void Start()
    {
        m_InventoryItemBtns = new List<InventoryItemBtn>();
        _mainMenuController = MainMenuController.instance;
        _dataController = DataController.instance;
        _loadingController = LoadingController.instance;
        _characterCustomizationController = CharacterCustomizationController.instance;
        _inGameShopController = FindObjectOfType<InGameShopController>();
        //StoreItemIDs();

    }

    public void cancelInventoryItem()
    {
        SelectInventoryItem(ApplicationController.SelectedInventoryItem);
    }

    public void HideItem()
    {
        //    --------------- For 3D Items -----------------
        //    Items[tempSelectedItem].SetActive(false);
        //    Items[ApplicationController.SelectedInventoryItem].SetActive(false);
        if (m_CurrentItem)
            m_CurrentItem.SetActive(false);

    }
    public void ShowItem()
    {
        //    --------------- For 3D Items -----------------
        // Items[tempSelectedItem].SetActive(true);
        SelectCategory(0);
    }



    public void UnlockInventoryItem()
    {
        _dataController.SetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory, tempSelectedItem);
        _dataController.RemoveCoins(CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].price);
        m_InventoryItemBtns[tempSelectedItem].lockButton.SetActive(false);
        ApplicationController.SelectedInventoryItem = tempSelectedItem;
        m_CustomizationBtn.SetActive(CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].Customization);
        InitiateUnlocking();
    }
    public void UnlockInventoryItemRewarded()
    {
        _dataController.SetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory, tempSelectedItem);
        m_InventoryItemBtns[tempSelectedItem].lockButton.SetActive(false);
        ApplicationController.SelectedInventoryItem = tempSelectedItem;
        m_CustomizationBtn.SetActive(CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].Customization);
        InitiateUnlocking();
    }
    public void UnlockAllInventoryItem()
    {
        unlockCategoryItem(Categories.Melee);
        unlockCategoryItem(Categories.HandGun);
        unlockCategoryItem(Categories.SemiAuto);
        unlockCategoryItem(Categories.Sniper);
        unlockCategoryItem(Categories.Grenade);
    }

    public void unlockCategoryItem(Categories categories)
    {
        for (int k = 0; k < ItemsCategories[(int)categories].m_InventoryItemInfo.Length; k++)
        {
            _dataController.SetUnlockInventoryItem((int)categories, k);
        }
    }

    public void InitiateUnlocking()
    {
        if (currentCategory == Categories.Melee)
        {
            //_juInventory.UnlockMeleeItem(tempSelectedItem);
        }
        if (currentCategory == Categories.HandGun)
        {
            //_juInventory.UnlockHandGunItems(tempSelectedItem);
        }
        if (currentCategory == Categories.SemiAuto)
        {
            //_juInventory.UnlockSemiAutoItems(tempSelectedItem);
        }
        if (currentCategory == Categories.Sniper)
        {
            //_juInventory.UnlockSniperItem(tempSelectedItem);
        }
    }

    public void BuyInventoryItem()
    {
        if (_dataController.coins >= CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].price)
        {
            UnlockInventoryItem();
            Buy.gameObject.SetActive(false);

        }
        else
        {
            //ApplicationController._RewardBtnValue = RewardBtn.Gun;
            _mainMenuController.AddPanelToStackAndLoad(8);
        }
    }
    public void BuyInventoryItemGame()
    {
        if (_dataController.coins >= CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].price)
        {
            UnlockInventoryItem();
            Buy.gameObject.SetActive(false);
        }
        else
        {
            //ApplicationController._RewardBtnValue = RewardBtn.Gun;
            _inGameShopController.Panels[3].enabled = true;
        }
    }

    public void Display()
    {
        // _mainMenuController.AddPanelToStackAndLoad(3);
        // SelectCategory(0);
        Play();
    }
    [ContextMenu("Enter Shop")]
    public void DisplayGameShop()
    {
        SelectCategory(0);
    }

    public void Play()
    {
        HideItem();
        // _loadingController.display(Scenes.Gameplay);
        _characterCustomizationController.Display();
    }

    public void PlayBtnGameShop()
    {
        InGameShopController.instance.InventoryPlay();
    }

    public void SelectCategory(int i)
    {
        currentCategory = (Categories)i;
        ApplicationController.SelectedInventoryCategory = (int)currentCategory;
        CurrentInventoryCategory = ItemsCategories[i];
        InitializeCategory(i);
    }

    public void InitializeCategory(int i)
    {
        for (int k = 0; k < ItemsCategories.Length; k++)
        {
            ItemsCategories[k].selectedImage.SetActive(false);
        }

        ItemsCategories[i].selectedImage.SetActive(true);

        for (int k = 0; k < m_InventoryItemBtns.Count; k++)
        {
            Destroy(m_InventoryItemBtns[k].gameObject);
        }

        if (m_InventoryItemBtns.Count > 0)
            m_InventoryItemBtns.Clear();


        for (int k = 0; k < CurrentInventoryCategory.m_InventoryItemInfo.Length; k++)
        {
            GameObject item = Instantiate(m_ItemBtnPrefab, m_ItemBtnParent);
            InventoryItemBtn inventoryItemBtn = item.GetComponent<InventoryItemBtn>();
            if (CurrentInventoryCategory.m_InventoryItemInfo[k].Icon)
                inventoryItemBtn.Icon.sprite = CurrentInventoryCategory.m_InventoryItemInfo[k].Icon;
             //CurrentInventoryCategory.m_InventoryItemInfo[k].id = k;

            inventoryItemBtn.Id = k;
            print("inventoryItemBtn.Id" + inventoryItemBtn.Id);
            item.GetComponent<Button>().onClick.AddListener(() => SelectInventoryItem(inventoryItemBtn.Id));
            m_InventoryItemBtns.Add(inventoryItemBtn);

        }

        _InventorySlider.value = 1f;
        UnlockingInventoryItem(i);
        SelectInventoryItem(ApplicationController.SelectedInventoryItem);
    }



    public void UnlockingInventoryItem(int j)
    {
        inventoryItemButtonsLength = m_InventoryItemBtns.Count;

        if (inventoryItemButtonsLength <= 0) return;

        for (i = 0; i < inventoryItemButtonsLength; i++)
        {
            if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory, i))
            {
                m_InventoryItemBtns[i].lockButton.SetActive(false);
            }
            else
            {
                m_InventoryItemBtns[i].lockButton.SetActive(true);
            }
        }
    }

    int j, itemsLength;

    public void SelectInventoryItem(int i)
    {
        // print(i);
        tempSelectedItem = i;
        // for (i = 0; i < CurrentInventoryCategory.m_Items.Length; i++)
        // {
        //     CurrentInventoryCategory.m_Items[i].SetActive(false);
        // }

        if (m_CurrentItem)
            m_CurrentItem.SetActive(false);

        m_CurrentItem = CurrentInventoryCategory.m_Items[tempSelectedItem];

        m_CurrentItem.SetActive(true);

        for (i = 0; i < inventoryItemButtonsLength; i++)
        {
            m_InventoryItemBtns[i].selectedImage.SetActive(false);
        }
        m_InventoryItemBtns[tempSelectedItem].selectedImage.SetActive(true);


        if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory, tempSelectedItem))
        {
            //ApplicationController.SelectedInventoryItem = tempSelectedItem;
            ApplicationController.SelectedInventoryItem = CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].id;

            print("Category: " + ApplicationController.SelectedInventoryCategory + "Item Index :" + ApplicationController.SelectedInventoryItem);
            Buy.gameObject.SetActive(false);
            m_CustomizationBtn.SetActive(CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].Customization);
            SelectItem();
        }
        else
        {
            Buy.gameObject.SetActive(true);
            PriceText.text = CurrentInventoryCategory.m_InventoryItemInfo[tempSelectedItem].price.ToString();
            m_CustomizationBtn.SetActive(false);
        }

        Stats(tempSelectedItem);

    }

    // 

    public void SelectItem()
    {
        if (currentCategory == Categories.Melee)
        {
            //_juInventory.SelectMeleeItem(tempSelectedItem);
        }
        if (currentCategory == Categories.HandGun)
        {
            //_juInventory.SelectHandGunItems(tempSelectedItem);
        }
        if (currentCategory == Categories.SemiAuto)
        {
            //_juInventory.SelectSemiAutoItems(tempSelectedItem);
        }
        if (currentCategory == Categories.Sniper)
        {
            //_juInventory.SelectSniperItem(tempSelectedItem);
        }
    }


    public void Stats(int i)
    {
        NameofWeapon.text = CurrentInventoryCategory.m_InventoryItemInfo[i].Name;


        Damage.DOFillAmount(CurrentInventoryCategory.m_InventoryItemInfo[i].Damage / 100f, 1f);
        Recoil.DOFillAmount(CurrentInventoryCategory.m_InventoryItemInfo[i].Recoil / 100f, 1f);
        Magazine.DOFillAmount(CurrentInventoryCategory.m_InventoryItemInfo[i].Magazine / 100f, 1f);
        Range.DOFillAmount(CurrentInventoryCategory.m_InventoryItemInfo[i].Range / 100f, 1f);
        RateOfFire.DOFillAmount(CurrentInventoryCategory.m_InventoryItemInfo[i].RateOfFire / 100f, 1f);

    }
    //customFunction to store all Item IDs so i can assign these items to player inventory

    /*void StoreItemIDs()
        {
        Debug.Log(CurrentInventoryCategory.m_InventoryItemInfo.Length + "YES");
            for (int i = 0; i < CurrentInventoryCategory.m_InventoryItemInfo.Length; i++)
            {
            //int i = 42;
            PlayerPrefs.SetInt("ItemID_" + i,i);
            Debug.Log("ItemID_" + i);
        }

            PlayerPrefs.SetInt("TotalItems", CurrentInventoryCategory.m_InventoryItemInfo.Length);
        PlayerPrefs.SetInt("TotalItems", 1);

        PlayerPrefs.Save();
        }*/
    }
[Serializable]
public struct InventoryCategory
{
    public string m_Name;
    public GameObject selectedImage;
    public InventoryItemInfo[] m_InventoryItemInfo;
    public GameObject[] m_Items;
}

// this is my copy paste script for custom items integrating into the inventory system of Invector.
[Serializable]
public class InventoryItem
{
    public int ItemID;
    public GameObject selectedImage;
    public GameObject inventoryItemlockButtons;
    public int price;

}


