
//using JUTPS;
//using JUTPS.CrossPlataform;
using SickscoreGames.HUDNavigationSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    #region Instance
 
    private static InGameController _instance;

    public static InGameController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InGameController>();
            }

            return _instance;
        }
    }
 
    #endregion

    public TextMeshProUGUI LevelText;
    public HUDNavigationSystem m_HudNavigationSystem;
    public HUDNavigationCanvas _hudNavigationCanvas;

    public GameObject m_ControllerUI;
    public Canvas m_CarControllerUI;
    public GameObject m_GameplayCanvas;
    public Button m_ExitVehicleBtn;

    //private MobileRig m_mobileRig;

    private void OnEnable()
    {
        GameController.gamePaused += pauseClicked;
        GameController.gameResumed += resumeClicked;
        GameController.gameStarted += startedClicked;
        GameController.gameEnded += endedClicked;
    }

    private void OnDisable()
    {
        
        GameController.gamePaused -= pauseClicked;
        GameController.gameResumed -= resumeClicked;
        GameController.gameStarted -= startedClicked;
        GameController.gameEnded -= endedClicked;
    }
    
    private void Awake()
    {
        _instance = this;
    } 
    
    public void Start()
    {
        //m_mobileRig = MobileRig.instance;
        LevelText.text = ScriptLocalization.LevelTitle + (ApplicationController.SelectedLevel+1).ToString();
    }

    
    public void pauseClicked()
    {
        
    }
    public void resumeClicked()
    {
        
    }
    public void startedClicked()
    {
        
    }
    public void endedClicked()
    {
       HideAllHUD();
    }
    
    public void ForceDisableEnterVehicleBtn(bool b)
    {
        //m_mobileRig.ForceDisableEnterVehicle = b;
    }

    public void ShowMovementControl()
    {
        //m_mobileRig.DisableMovement(true);
    }
    
    public void HideMovementControl()
    {
        //m_mobileRig.DisableMovement(false);
    }

    public void CarHUD(bool b)
    {
        if (!b)
        {
            m_CarControllerUI.GetComponent<RCC_UIDashboardDisplay>().displayType =
                RCC_UIDashboardDisplay.DisplayType.Off;
        }
        else
        {
            m_CarControllerUI.GetComponent<RCC_UIDashboardDisplay>().displayType =
            RCC_UIDashboardDisplay.DisplayType.Full;
        }
    }

    public void HideAllHUD()
    {
        m_HudNavigationSystem.useIndicators = false;
        m_HudNavigationSystem.useRadar = false;
        m_HudNavigationSystem.useCompassBar = false;
        _hudNavigationCanvas.Radar.Panel.gameObject.SetActive(false);
        _hudNavigationCanvas.Indicator.Panel.gameObject.SetActive(false);
        _hudNavigationCanvas.CompassBar.Panel.gameObject.SetActive(false);
        m_ControllerUI.SetActive(false);
        m_GameplayCanvas.gameObject.SetActive(false);
        m_CarControllerUI.gameObject.SetActive(false);
        //JUPauseGame.Paused =  true;

    }
    public void ShowAllHUD()
    {
        m_HudNavigationSystem.useIndicators = true;
        m_HudNavigationSystem.useRadar = true;
        m_HudNavigationSystem.useCompassBar = true;
        _hudNavigationCanvas.Radar.Panel.gameObject.SetActive(true);
        _hudNavigationCanvas.Indicator.Panel.gameObject.SetActive(true);
        _hudNavigationCanvas.CompassBar.Panel.gameObject.SetActive(true);
        m_ControllerUI.SetActive(true);
        m_GameplayCanvas.gameObject.SetActive(true);
        if (ApplicationController.ExternalVehicle)
        {
            m_CarControllerUI.gameObject.SetActive(true);
        }
        //JUPauseGame.Paused =  false;

    }
}
