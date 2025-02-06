using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;
using Invector;
using Invector.vCharacterController;

public class InGameShopController : MonoBehaviour
{
    #region Instance
 
    private static InGameShopController _instance;

    public static InGameShopController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InGameShopController>();
            }

            return _instance;
        }
    }
 
    #endregion
    
    public GameObject WeaponParent;
    public GameObject CharacterParent;
    public GameObject ClothShop;
    public GameObject FreelookCamera;
    public GameObject ClothShopCamera;
    
    public Canvas[] Panels;
    
    //[HideInInspector]
    public Transform ExitPosition;
    
     [HideInInspector]
       public Transform ExitClothShopPosition;

    InventoryController InventoryController;
    CustomizationController _customizationController;
    CharacterCustomizationController _characterCustomizationController;
    InGameController m_inGameController;
    //JUCharacterController _juCharacterController;
    vThirdPersonController _vThirdPersonController;

    private void Awake()
    {
        _instance = this;
        
    }
    private void Start()
    {
        InventoryController = InventoryController.instance;
        _characterCustomizationController = CharacterCustomizationController.instance;
        m_inGameController = InGameController.instance;
        _customizationController= CustomizationController.instance;
        //_juCharacterController = FindObjectOfType<JUCharacterController>();
        _vThirdPersonController = FindObjectOfType<vThirdPersonController>();

    }

    public void InventoryPlay()
    {
        ExitShop();
        InventoryManagerbyJunaid.instance.AssignAllUnlockedItemsToPlayer();
    }

    public void GunCustomization()
    {
        Panels[0].enabled = false;
        Panels[1].enabled = true;
        _customizationController.DisplayGameShop();
    }

    public void GunCustomizationBack()
    {
        Panels[0].enabled = true;
        Panels[1].enabled = false;
        _customizationController.ResetSelection();
    }
    // Start is called before the first frame update
    public void EnterShop()
    {
        //print("Enter Shop");
        GameController.instance.Camera.gameObject.SetActive(false);
        _vThirdPersonController.enabled = false;
        Panels[0].enabled = true;
        FreelookCamera.SetActive(true);
        WeaponParent.SetActive(true);
        m_inGameController.HideAllHUD();
        InventoryController.DisplayGameShop();
        print("Enter Shop 2");

        /*if(AdmobAdsManager.Instance)
            AdmobAdsManager.Instance.HideLargeRightBanner();*/

    }

    public void ExitShop()
    {
        Panels[0].enabled = false;
        GameController.instance.Camera.gameObject.SetActive(true);
        _vThirdPersonController.enabled = true;
        FreelookCamera.SetActive(false);
        WeaponParent.SetActive(false);
        m_inGameController.m_ControllerUI.SetActive(true);
        m_inGameController.m_HudNavigationSystem.EnableSystem(true);
        _vThirdPersonController.transform.SetPositionAndRotation(ExitPosition.position , ExitPosition.rotation);  
        m_inGameController.ShowAllHUD();
        //InventoryManagerbyJunaid.instance.AssignAllUnlockedItemsToPlayer();
        /*if(AdmobAdsManager.Instance)
            AdmobAdsManager.Instance.ShowLargeRightBanner();*/
    }
    
    public void EnterClothShop()
    {
        GameController.instance.Camera.gameObject.SetActive(false);
        _vThirdPersonController.enabled = false;
        CharacterParent.SetActive(true);
        Panels[2].enabled = true;
        ClothShopCamera.SetActive(true);
        ClothShop.SetActive(true);
        m_inGameController.HideAllHUD();
        _characterCustomizationController.DisplayGameShop();
        
        /*if(AdmobAdsManager.Instance)
            AdmobAdsManager.Instance.HideLargeRightBanner();*/

    }
    
      public void ExitClothShop()
        {
            Debug.Log("Exit Cloth Shop 1");
            GameController.instance.Camera.gameObject.SetActive(true);
            _vThirdPersonController.enabled = true;
            Panels[2].enabled = false;
            ClothShopCamera.SetActive(false);
            ClothShop.SetActive(false);
            CharacterParent.SetActive(false);
            m_inGameController.m_ControllerUI.SetActive(true);
            m_inGameController.m_HudNavigationSystem.EnableSystem(true);
            _vThirdPersonController.transform.SetPositionAndRotation(ExitClothShopPosition.position , ExitClothShopPosition.rotation);  
            m_inGameController.ShowAllHUD();
            _vThirdPersonController.GetComponent<CharacterCustomizationHandler>().SetLastSelectedCustomization();
            Debug.Log("Exit Cloth Shop 2");
        /*if (AdmobAdsManager.Instance)
            AdmobAdsManager.Instance.ShowLargeRightBanner();*/
    }
}
