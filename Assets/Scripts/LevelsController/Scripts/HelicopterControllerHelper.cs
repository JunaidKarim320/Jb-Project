 using System;
using Cinemachine;
 //using JUTPS;
 using UnityEngine;
using UnityEngine.UI;

public class HelicopterControllerHelper : MonoBehaviour
{
    public Transform _spawnTransform;
    [SerializeField] private CinemachineFreeLook freeLook;
    private HelicopterController HelicopterController;
    //private JUCharacterController _juCharacterController;
    private FailedReason _failedReason;
    [HideInInspector]
    public GameObject Checkpoint;

    public bool Reset = true;
    public void Start()
    {
        HelicopterController = GetComponent<HelicopterController>();
        //_juCharacterController = GameObject.FindObjectOfType<JUCharacterController>();
        if (TryGetComponent(out FailedReason failedReason)) 
        {  _failedReason = failedReason;}
    }

    public void StartHelicopter()
    {
        HelicopterController.enabled = true;
       // freeLook.gameObject.SetActive(true);
        ApplicationController.NoTrafficVehicle = true;

    }

    /*public void StopHelicopter()
    {
        HelicopterController.enabled = false;
        //freeLook.gameObject.SetActive(false);
        if(Reset)
        transform.SetPositionAndRotation(_spawnTransform.position, _spawnTransform.rotation);
        if (_juCharacterController)
        {
            _juCharacterController.VehicleInAreaHelper = null;
         if (_juCharacterController.DriveVehicleAbility)  
             _juCharacterController.DriveVehicleAbility.VehicleDrivableNearby = false;
         
            _juCharacterController.VehicleInArea = null;
            _juCharacterController.VehicleInAreaHelper = null;
            _juCharacterController.ToEnterVehicle = false;
            
            if (_juCharacterController.DriveVehicleAbility)  
                _juCharacterController.DriveVehicleAbility.VehicleToDrive = null;
        }
        ApplicationController.NoTrafficVehicle = false;
        if (_failedReason) _failedReason.QuitMission();
        if (Checkpoint)
        {
             checkpointManager = Checkpoint.GetComponent<CheckpointManager>();
            if (checkpointManager.PathTracker)
            {
                if (checkpointManager._pathTracker) checkpointManager._pathTracker.StopTarget();
            }
            Checkpoint.SetActive(false);

        }

    }*/

    private CheckpointManager checkpointManager;
}