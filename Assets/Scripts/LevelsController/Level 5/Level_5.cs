using System;
using System.Collections;
//using JUTPS.InventorySystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Level_5 : Level
{
    public Transform HelicopterRoofPoint;
    public Transform LandSpawnPoint;
    public GameObject Helicopter;
    public GameObject Vincent;
    public Transform VincentRoofPoint;

    [Header("Characters")] 
    public GameObject Max;

    public HelicopterPointController helicopterPointController;
    public const string LevelName = "Vincent Down";


    public Steps[] Step;
    

    private int stepNumber;
    private GameController _gameController;
    //private JUInventory juInventory;

    void Start()
    {
        _gameController = GameController.instance;
        _ingameController = InGameController.instance;
        //juInventory = _gameController.Player.GetComponent<JUInventory>();
//        stepNumber = DataController.lastSelectedLevel;
        stepNumber = 0;
        
        if(_gameController != null)
       { 
           //_gameController.Player.transform.position = PlayerSpawnPoint.transform.position;
           //_gameController.Player.transform.rotation = PlayerSpawnPoint.transform.rotation;
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
    public void VincentSpawn()
        {
           GameObject t = Instantiate(Vincent, VincentRoofPoint);
            t.transform.position = VincentRoofPoint.transform.position;
            t.transform.rotation = VincentRoofPoint.transform.rotation;
        }
    public void LandSpawn()
    {
        //_gameController.Player.transform.position = LandSpawnPoint.transform.position;
        //_gameController.Player.transform.rotation = LandSpawnPoint.transform.rotation;
    }
    public void LandHelicopter()
    {
       /* helicopterPointController.Vehicle.transform.position = HelicopterRoofPoint.transform.position;
        helicopterPointController.Vehicle.transform.rotation = HelicopterRoofPoint.transform.rotation;
        helicopterPointController.Vehicle.rb.drag = 50f;
        helicopterPointController.Vehicle.rb.isKinematic = true;*/
    }

    public void HelicopterOut()
    {
      /*helicopterPointController.Vehicle.transform.position = HelicopterRoofPoint.transform.position;
       helicopterPointController.Vehicle.transform.rotation = HelicopterRoofPoint.transform.rotation;
        helicopterPointController.HelicopterTrigger.CarOut();
        Destroy(helicopterPointController.Vehicle.gameObject);
        Helicopter.SetActive(true);
        _gameController.Player.IsHelicopter = false;*/
    }

    public void ForceDisableEnterVehicleBtn(bool b)
    {
        _gameController.ForceDisableEnterVehicleBtn(b);
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

    public override void NextStep()
    {
        stepNumber = stepNumber + 1;
        StepStart();
    }

   
}


