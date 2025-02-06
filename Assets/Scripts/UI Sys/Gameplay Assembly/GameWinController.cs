
using TMPro;
using UnityEngine;

public class GameWinController : MonoBehaviour
{
    #region Instance
 
    private static GameWinController _instance;

    public static GameWinController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameWinController>();
            }

            return _instance;
        }
    }
 
    #endregion


    private InventoryController _inventoryController;
    private CharacterCustomizationHandler _characterCustomizationHandler;
    private LocationController _locationController;
    private GameController _gameController;
    private void OnEnable()
    {
        GameController.gameEnded += endedClicked;
    }

    private void OnDisable()
    {
        GameController.gameEnded -= endedClicked;
    }
    
    private void Awake()
    {
        _instance = this;

        _gameController = FindObjectOfType<GameController>();
        _locationController = FindObjectOfType<LocationController>();
        _inventoryController = FindObjectOfType<InventoryController>();
        _characterCustomizationHandler = FindObjectOfType<CharacterCustomizationHandler>();
    }
    
    public void endedClicked()
    {
        
    }  
    public void ShowAd()
    {
        /*if (AdmobAdsManager.Instance)
        {
            AdmobAdsManager.Instance.ShowInterstitial();
            AdmobAdsManager.Instance.ShowLargeLeftBanner();
            AdmobAdsManager.Instance.HideLargeRightBanner();

        }   */
    }
    
    public void display()
    {
        _gameController.PlayerState(false);
        MissionPassed.SetActive(true);
        PassedMoneyText.text=MoneyText.text = $"+ ${winAmount}";
        Invoke(nameof(ShowAd),1f);

        if (WeaponItemUnlocked || CharacterCustomizationUnlocked)
        {
            Invoke(nameof(UnlockItemsPanelDisplay),2f);
        }
        else if (LocationUnlocked)
        {
            Invoke(nameof(UnlockLocationPanelDisplay),2f);
        }
        else
        {
            Invoke(nameof(GameWinPanelDisplay),2f);
        }
    }
    public void UnlockItemsPanelDisplay()
    {
        MissionPassed.SetActive(false);
        UnlockItemsPanel.SetActive(true);
        if (Cashobj)
        {
            Destroy(Cashobj);
        }
        Cashobj= Instantiate(Cash ,UnlockItemsParent);
        Cashobj.GetComponent<UnlockItem>().TitleText.text = $"+ ${winAmount}";
        
        if (WeaponItemUnlocked)
        {
            if (Weaponobj)
            {
                Destroy(Weaponobj);
            }
            Weaponobj= Instantiate(Weapon ,UnlockItemsParent);
            InventoryItemInfo itemInfo = _inventoryController.ItemsCategories[(int) WeaponCategoryIndex]
                .m_InventoryItemInfo[WeaponIndex];
            Weaponobj.GetComponent<UnlockItem>().TitleText.text = itemInfo.Name;
            Weaponobj.GetComponent<UnlockItem>().Icon.sprite = itemInfo.Icon;
            DataController.instance.SetUnlockInventoryItem((int)WeaponCategoryIndex , WeaponIndex);
            //unlock Code
        }
        if (CharacterCustomizationUnlocked)
        {
            if (Customizationobj)
            {
                Destroy(Customizationobj);
            }
            Customizationobj= Instantiate(Customization ,UnlockItemsParent);
            CustomizationItemInfo itemInfo = _characterCustomizationHandler.CustomizationCategories[(int) CharacterCustomizationPart].m_CustomizationCategory[CharacterCustomizationSubPart].
                m_CustomizationItemInfo[CharacterCustomizationIndex];
            
            Customizationobj.GetComponent<UnlockItem>().TitleText.text =itemInfo.Name; 
            Customizationobj.GetComponent<UnlockItem>().Icon.sprite =itemInfo.Icon; 
            //unlock Code
            DataController.instance.SetUnlockInventoryItem((int) CharacterCustomizationPart, CharacterCustomizationSubPart, CharacterCustomizationIndex);
        }
    }
    public void UnlockLocationPanelDisplay()
    {
        UnlockItemsPanel.SetActive(false);
        UnlockLocationPanel.SetActive(true);
        LocationDetails.text = _locationController.LocationsName[LocationIndex];
        _locationController.UnlockLocation(LocationIndex,false);
    } 
    
    public void GameWinPanelDisplay()
    {
        UnlockLocationPanel.SetActive(false);
        containerGO.enabled = true;  
        DataController.instance.AddCoins(winAmount);

    }

    public void hide()
    {
        containerGO.enabled = false;

    }
 
    [Header("Panel Container")]
    public Canvas containerGO; 
    public TextMeshProUGUI MoneyText;
    public GameObject MissionPassed;
    public TextMeshProUGUI PassedMoneyText;
    public GameObject UnlockItemsPanel;
    public Transform UnlockItemsParent;
    public GameObject Cash, Weapon, Customization;
    public GameObject UnlockLocationPanel;
    public TextMeshProUGUI LocationDetails;
    GameObject Cashobj, Weaponobj, Customizationobj;
    
   [HideInInspector] public int winAmount;
   [HideInInspector] public int LocationIndex;
   [HideInInspector] public bool LocationUnlocked;
   [HideInInspector] public InventoryController.Categories WeaponCategoryIndex;
   [HideInInspector] public int WeaponIndex;
   [HideInInspector] public bool WeaponItemUnlocked;
   [HideInInspector] public CharacterCustomizationController.CharacterCustomizationPart CharacterCustomizationPart;
   [HideInInspector] public int CharacterCustomizationSubPart;
   [HideInInspector] public int CharacterCustomizationIndex;
   [HideInInspector] public bool CharacterCustomizationUnlocked;

}
