using System;
using System.Collections;
//using JUTPS.InventorySystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Level_4 : Level
{
    public Transform PlayerSpawnRoofPoint;
    public Transform PlayerSpawnfloorPoint;

    [Header("Characters")] 
    public GameObject Max;

   

    public const string LevelName = "Gang War";


    public Steps[] Step;
    

    private int stepNumber;
    private GameController _gameController;
   // private JUInventory juInventory;

    void Start()
    {
        _gameController = GameController.instance;
        _ingameController = InGameController.instance;
        //juInventory = _gameController.Player.GetComponent<JUInventory>();
//        stepNumber = DataController.lastSelectedLevel;
        stepNumber = 0;
        
        if(_gameController != null)
       { 
           _gameController.Player.transform.position = PlayerSpawnPoint.transform.position;
           _gameController.Player.transform.rotation = PlayerSpawnPoint.transform.rotation;
          // GameObject car= Instantiate(PlayerCar, PlayerCarSpawnPoint);
         //  car.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
       }
        
        if (Step[stepNumber].IsTrafficOn)
        {
            ApplicationController.NoTrafficVehicle = !Step[stepNumber].IsTrafficOn;
        }
        else
        {
            ApplicationController.NoTrafficVehicle = !Step[stepNumber].IsTrafficOn;
        }
        
        
      //  StepStart();
        
    }

    [ContextMenu("Sniper")]
    public void Sniping()
    {
        //_gameController.Player._Crouch();
        //juInventory.SelectSniperItem(0);
    }
    [ContextMenu("No Sniper")]
    public void NoSniping()
    {
        //_gameController.Player._GetUp();
       
    }
    public void RoofSpawn()
    {
        _gameController.Player.transform.position = PlayerSpawnRoofPoint.transform.position;
        _gameController.Player.transform.rotation = PlayerSpawnRoofPoint.transform.rotation;
        HideMovementHUD();
        //_gameController.Player._Crouch();
        //juInventory.SelectSniperItem(0);
        _gameController.NoTraffic();
    }
    
    public void Traffic()
    {
        ApplicationController.NoTrafficVehicle = false;
    }
    public void FloorSpawn()
    {
        //_gameController.Player._GetUp();
        _gameController.Player.transform.position = PlayerSpawnfloorPoint.transform.position;
        _gameController.Player.transform.rotation = PlayerSpawnfloorPoint.transform.rotation;
        ShowMovementHUD();
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
        ApplicationController.NoTrafficVehicle = !Step[stepNumber].IsTrafficOn;
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

   
}


