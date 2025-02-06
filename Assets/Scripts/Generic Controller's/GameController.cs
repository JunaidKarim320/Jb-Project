
using Invector.vCharacterController;
using Spirit604.DotsCity.Core;
using System.Collections;

using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Instance
 
    private static GameController _instance;

    public static GameController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
            }

            return _instance;
        }
    }
 
    #endregion
     
    private GameOverController _gameOverController;

    private GameWinController _gameWinController;
    private InGameController _inGameController;
    private ObjectiveController _ObjectiveController;
    private LocationController _locationController;


    
    private LoadingController _loadingController;
    //private CreateAI _createAi;

    public delegate void GameStatusHandler();

    public static event GameStatusHandler gamePaused;

    public static event GameStatusHandler gameResumed;

    public static event GameStatusHandler gameStarted;

    public static event GameStatusHandler gameEnded;

    public static event GameStatusHandler gameRestarted;

    public delegate void Cinematic();

    public static event Cinematic cinematicEvent;

    public static GameStatus _gameStatus = GameStatus.INGAME;
    
    public static GameStatus gameStatus
    {
        get
        {
            return _gameStatus;
        }
        set
        {
            _gameStatus = value;
        }
    }
    
    public vThirdPersonController Player;
    public GameObject LevelController;
    public GameObject Camera;

    //private DriveVehicle driveVehicle;
    private void Awake()
    {
        _instance = this;
  
    }
    
   

    IEnumerator Start()
    {
        _gameOverController=GameOverController.instance;
        _gameWinController=GameWinController.instance;
        _loadingController=LoadingController.instance;
        _inGameController = InGameController.instance;
        _locationController = LocationController.instance;
        _ObjectiveController= FindObjectOfType<ObjectiveController>();

        /*_createAi = CreateAI.instance;
        driveVehicle = Player.GetComponent<DriveVehicle>();*/
        GameStart();
        

        AllMissionComplete();
        yield return new WaitForSeconds(1f);
        /*if (AdmobAdsManager.Instance)
        {
            AdmobAdsManager.Instance.HideLargeLeftBanner();
            AdmobAdsManager.Instance.ShowLargeRightBanner();
            
            print("Hide Top Banner");
        }*/
    }

   

    public void AllMissionComplete()
    {
        if (ApplicationController.LastSelectedLevel >= 5)
        {
            ApplicationController.AllMissionPoint = 1;
            LevelController.gameObject.SetActive(false);
            _locationController.UnlockAllLocation();
        }
        else
        {
            LevelController.gameObject.SetActive(true);
        }
    }

    public void NoTraffic()
    {
        /*if(_createAi)
            _createAi.NoTraffic();*/
    }
    public void PlayerState(bool b)
    {
        /*Player.DisableAllMove = !b;
        JUPauseGame.Paused = !b;
        Player.enabled = b;
        Player.anim.SetFloat(Player.AnimatorParameters.Speed, 0f);
        Player.anim.SetFloat(Player.AnimatorParameters.MovingTurn, 0f);*/

    }

    public void ForceDisableEnterVehicleBtn(bool b)
    {
        _inGameController.ForceDisableEnterVehicleBtn(b);
    }
    
   

    public void ResetCamera()
    {
        if(Camera)
        Camera.GetComponent<ResetCamera>().ResetCameraPos();
    }
    public void ManualBrakeOn()
    {
    /*if(driveVehicle)
        driveVehicle.ManualStop();*/
    }
    public void ManualBrakeOff()
    {
        /*if(driveVehicle)
        driveVehicle.ManualStart();*/
    }
    public IEnumerator CarIn(Transform t)
    {
        yield return new WaitForSeconds(.1f);
        /*if(driveVehicle)
            driveVehicle.GetInCar(t);*/
    }
    public void CarOut()
    {
        /*if(driveVehicle)
        driveVehicle.GetOffCar();*/
    }
    public void displayGameOver()
    {
        gameStatus = GameStatus.GAMEOVER;
        _gameOverController.display(null);
        gameEnded?.Invoke();
        AllMissionComplete();
        _ObjectiveController.HidePopup();
        
    }

    public void displayGameWin()
    {
        gameStatus = GameStatus.GAMEOVER;
        DataController.instance.SetUnlockLevel(ApplicationController.LastSelectedLevel);
        ApplicationController.LastSelectedLevel += 1;
        _gameWinController.display();
        gameEnded?.Invoke();
        AllMissionComplete();
        _ObjectiveController.HidePopup();
    }

    
    public void GameStart()
    {
        gameStatus = GameStatus.INGAME;
		
        if (gameStatus == GameStatus.PAUSED)
        {
            gameResumed?.Invoke();

        }
        else
        {
            gameStarted?.Invoke();
        }
    }

    public void GameResumed()
    {
        if (gameStatus == GameStatus.PAUSED)
        {
            gameStatus = GameStatus.INGAME;
            gameResumed?.Invoke();
        }
        /*if (AdmobAdsManager.Instance)
        {
            AdmobAdsManager.Instance.HideLargeLeftBanner();
            AdmobAdsManager.Instance.ShowLargeRightBanner();
        }*/
    }
    
    public void GamePaused()
    {
        gameStatus = GameStatus.PAUSED;
        gamePaused?.Invoke();
        /*if (AdmobAdsManager.Instance)
        {
            AdmobAdsManager.Instance.ShowInterstitial();
            AdmobAdsManager.Instance.ShowLargeLeftBanner();
            AdmobAdsManager.Instance.HideLargeRightBanner();
        }*/
    }

    public  void GameRestart()
    {
        gameRestarted?.Invoke();
        _loadingController.display(Scenes.Gameplay);
    }

    public void Home()
    {
        _loadingController.display(Scenes.Mainmenu);
    }

    public void Next()
    {
        Dispose();
        _loadingController.display(Scenes.Gameplay);
         PlayerState(true);
       _gameWinController.hide();
       _inGameController.ShowAllHUD();
      
    }
    [SerializeField] private EntityWorldService entityWorldService;

    private void Dispose()
    {
        entityWorldService.DisposeWorld();
    }
}

