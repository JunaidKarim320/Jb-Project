using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoliceTrigger : MonoBehaviour
{
    public PolicePointController PolicePointController;
    public GameObject Checkpoints;

    //private JUCharacterController _juCharacterController;
    private RewardedPopController _rewardedPopController;

    void Start()
    {
        PolicePointController = GetComponentInParent<PolicePointController>();
        _rewardedPopController = RewardedPopController.instance;
        Checkpoints = PolicePointController.Checkpoint();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //_juCharacterController = other.GetComponent<JUCharacterController>();
            _rewardedPopController.ShowPop();
            RewardedPopController.m_CarIn += CarInReward;
        }
    }
    public void CarInReward()
    {
        /*if (_juCharacterController)
        {
            _juCharacterController.ToEnterVehicle = true;
            _juCharacterController.VehicleInAreaHelper = null;
                _juCharacterController.DriveVehicleAbility.VehicleToDrive =PolicePointController.Vehicle;
                if ( _juCharacterController.DriveVehicleAbility.VehicleToDrive != null)
                {
                    _juCharacterController.DriveVehicleAbility.VehicleDrivableNearby = true;
                }
                _juCharacterController.VehicleInArea = PolicePointController.Vehicle;
                _juCharacterController.VehicleInAreaHelper = PolicePointController.VehicleAlignmentHelper;
                _juCharacterController.DriveVehicleAbility.ManualEnterExit();
                RewardedPopController.m_CarIn -= CarInReward;
                _rewardedPopController.HidePopup();
                Checkpoints.SetActive(true);


        }*/
    }
}
