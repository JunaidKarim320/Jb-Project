using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Level_1 : Level
{
    [Header("Characters")] 
    public GameObject Max;
    public GameObject Emily;
    [Space] 
    
    public Transform PlayerCarSpawnPoint;
    public GameObject PlayerCar;
    public GameObject TargetDestination;

    public const string LevelName = "Max_Arrival";


    private Animator EmilyAnimator;

    public Steps[] Step;
    

    private int stepNumber;
    private GameController _gameController;

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
           GameObject car= Instantiate(PlayerCar, PlayerCarSpawnPoint);
           car.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
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
    
    public void StepStart()
    {
        Step[stepNumber].OnStepStartAction.Invoke();
       
        HideHUD();
        _gameController.ManualBrakeOn();

    }


   [ContextMenu("Skip")]
    public override void StepComplete()
    {
        Step[stepNumber].OnStepCompleteAction.Invoke();
        //ApplicationController.NoTrafficVehicle = !Step[stepNumber].IsTrafficOn;
        if (_gameController)
        {
            _gameController.ResetCamera();
            _gameController.ManualBrakeOff();
        }

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

    public void PlayerSpawn()
    {
         
    }
   
}


