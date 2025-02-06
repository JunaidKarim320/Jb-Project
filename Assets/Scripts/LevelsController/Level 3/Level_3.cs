using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Level_3 : Level
{
    [Header("Characters")] 
    public GameObject Max;
    public GameObject Emily;
    [Space] 
    
    public Transform PlayerCarSpawnPoint;
    public GameObject PlayerCar;
    public Transform JackCarSpawnPoint;
    public GameObject JackCar;
    public RCC_AIWaypointsContainer waypoint;
    public GameObject TargetDestination;

    public const string LevelName = "Max_Arrival";


    private Animator EmilyAnimator;

    public Steps[] Step;
    

    private int stepNumber;
    private GameController _gameController;
    [HideInInspector]
    public GameObject rivalcar;

    public static Level_3 instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _gameController = GameController.instance;
        _ingameController = InGameController.instance;

//        stepNumber = DataController.lastSelectedLevel;
        stepNumber = 0;
        
        if(_gameController != null)
       { 
           _gameController.Player.transform.position = PlayerSpawnPoint.transform.position;
           _gameController.Player.transform.rotation = PlayerSpawnPoint.transform.rotation;
           
         //  GameObject car= Instantiate(PlayerCar, PlayerCarSpawnPoint);
          // car.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
       }
        
        /*if (Step[stepNumber].IsTrafficOn)
        {
            ApplicationController.NoTrafficVehicle = !Step[stepNumber].IsTrafficOn;
        }
        else
        {
            ApplicationController.NoTrafficVehicle = !Step[stepNumber].IsTrafficOn;
        }*/
        
        
        StepStart();
        
    }
    public void SpawnJackCar()
    {
        if(rivalcar){
            Destroy(rivalcar);
        }
        rivalcar= Instantiate(JackCar, JackCarSpawnPoint);
        rivalcar.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
        rivalcar.GetComponent<RCC_AICarController>().waypointsContainer = waypoint;
    }
    public void StartRivalCar()
    {
      //  _gameController.NoTraffic();
        rivalcar.GetComponent<RCC_CarControllerV3>().canControl = true;
        rivalcar.GetComponent<RCC_CarControllerV3>().engineRunning = true;
        rivalcar.GetComponent<RCC_CarControllerV3>().enabled = true;
    }
    public void StepStart()
    {
        Step[stepNumber].OnStepStartAction.Invoke();
       
        HideHUD();
        _gameController.ManualBrakeOn();

    }


   [ContextMenu("Skip")]
    public override void StepComplete()
    {
        Step[stepNumber].OnStepCompleteAction?.Invoke();
        ApplicationController.NoTrafficVehicle = !Step[stepNumber].IsTrafficOn;
        if (_gameController)
        {
            _gameController.ResetCamera();
            _gameController.ManualBrakeOff();
        }
///        print("here complete");
/// 
if (Step[stepNumber].Ad)
{
    /*if (AdmobAdsManager.Instance)
    {
        AdmobAdsManager.Instance.ShowInterstitial();
    }*/
}
    }


  

    public void NextStep()
    {
        stepNumber = stepNumber + 1;
        StepStart();
    }

    public void NextStep(float f)
    {
        stepNumber = stepNumber + 1;
       Invoke(nameof(StepStart),f);
    }
}


