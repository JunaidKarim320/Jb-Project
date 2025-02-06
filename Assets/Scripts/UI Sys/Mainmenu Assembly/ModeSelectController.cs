
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectController : MonoBehaviour
{
    #region Instance
 
    private static ModeSelectController _instance;

    public static ModeSelectController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ModeSelectController>();
            }

            return _instance;
        }
    }
 
    #endregion

    private MainMenuController _mainMenuController;
    private LevelMenuController _levelMenuController;
    private InventoryController _inventoryController;
    public Image[] ModeImages;
    private void Awake()
    {
  
        _instance = this;
        //SelectedMode();
    }


    private void Start()
    {
        _mainMenuController = MainMenuController.instance;
        _levelMenuController = LevelMenuController.instance;
        _inventoryController = InventoryController.instance;
    }
 
    public void PlayBtnClick()
    {
       // _mainMenuController.AddPanelToStackAndLoad(2);
      //  _levelMenuController.ExecuteLevel();
      // if (ApplicationController.SelectedGameMode == 0)
      // {
      //      _mainMenuController.AddPanelToStackAndLoad(2);
      //       _levelMenuController.ExecuteLevel();  
      // }
      // else
      // {
          _inventoryController.Display();
      //}
      
    }
    
    public void ModeSelectOnBtnClick(int n)
    {
        ApplicationController.SelectedGameMode = n;
        // PlayBtnClick();
        SelectedMode();
    }


    public void SelectedMode()
    {
        // for (int i = 0; i < ModeImages.Length; i++)
        // {
        //     ModeImages[i].gameObject.SetActive(false);
        // }
        // ModeImages[ApplicationController.SelectedGameMode].gameObject.SetActive(true);
        _inventoryController.Display();
    }
}


