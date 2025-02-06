
using TMPro;
using UnityEngine;

public class GameOverController : MonoBehaviour
{  
    #region Instance
 
    private static GameOverController _instance;

    public static GameOverController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameOverController>();
            }

            return _instance;
        }
    }
 
    #endregion

    private InGameController _inGameController;
    public void Start()
    {
        _inGameController = InGameController.instance;
    }

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
  
    }
    
    public void endedClicked()
    {
        
    }
    
    public void display(string text)
    {
        MissionFailed.SetActive(true);
        MissionFailedReasonText.text = text;
        Invoke(nameof(ShowAd),1f);
        Invoke(nameof(hide),2f);
    }

    public void GameFailPanelDisplay()
    {
        containerGO.enabled = true;  
    }
    public void hide()
    {
        MissionFailed.SetActive(false);
        _inGameController.ShowAllHUD();
        
        // GameController.instance.GameRestart();
    }
    public void ShowAd()
    {
        /*if (AdmobAdsManager.Instance)
        {
            AdmobAdsManager.Instance.ShowInterstitial();
            AdmobAdsManager.Instance.ShowLargeLeftBanner();
            AdmobAdsManager.Instance.HideLargeRightBanner();
        }*/
    }
 
    [Header("Panel Container")]
    public Canvas containerGO; 
    public GameObject MissionFailed; 
    public TextMeshProUGUI MissionFailedReasonText; 
}


