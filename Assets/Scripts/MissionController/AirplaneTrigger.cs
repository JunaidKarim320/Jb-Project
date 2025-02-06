using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;
using Random = UnityEngine.Random;

public class AirplaneTrigger : MonoBehaviour
{
    public AirplanePointController AirplanePointController;
    public GameObject Checkpoints;

    //private JUCharacterController _juCharacterController;
    private RewardedPopController _rewardedPopController;

    void Start()
    {
        AirplanePointController = GetComponentInParent<AirplanePointController>();
        _rewardedPopController = RewardedPopController.instance;
        Checkpoints = AirplanePointController.Checkpoint();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //_juCharacterController = other.GetComponent<JUCharacterController>();
            _rewardedPopController.ShowPop();
            RewardedPopController.m_CarIn += CarInReward;
            _rewardedPopController.ShowPop();

        }
    }
    
    public void CarInReward()
    {
        //if (_juCharacterController)
        {
            /*_juCharacterController.ToEnterVehicle = true;
            _juCharacterController.VehicleInAreaHelper = null;
            _juCharacterController.DriveVehicleAbility.VehicleToDrive = AirplanePointController.Vehicle;
            if (_juCharacterController.DriveVehicleAbility.VehicleToDrive != null)
            {
                _juCharacterController.DriveVehicleAbility.VehicleDrivableNearby = true;
            }
            _juCharacterController.VehicleInArea = AirplanePointController.Vehicle;
            _juCharacterController.VehicleInAreaHelper = AirplanePointController.VehicleAlignmentHelper;
            _juCharacterController.DriveVehicleAbility.ManualEnterExit();*/
            RewardedPopController.m_CarIn -= CarInReward;
            _rewardedPopController.HidePopup();
            Checkpoints.SetActive(true);
        }
    }
}
