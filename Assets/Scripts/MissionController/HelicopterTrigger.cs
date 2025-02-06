using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterTrigger : MonoBehaviour
{
    public HelicopterPointController HelicopterController;
    public GameObject Checkpoints;

    public bool NoRewardedRequired;
    //private JUCharacterController _juCharacterController;
    private RewardedPopController _rewardedPopController;

    IEnumerator Start()
    {
        HelicopterController = GetComponentInParent<HelicopterPointController>();
        _rewardedPopController = RewardedPopController.instance;
        yield return new WaitForSeconds(1f);

        Checkpoints = HelicopterController.Checkpoint();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //_juCharacterController = other.GetComponent<JUCharacterController>();
            if (NoRewardedRequired)
            {
                _rewardedPopController.ShowPop();
                RewardedPopController.m_CarIn += CarInReward;
            }
            else
            {
                CarInReward();
            }
        }
    }
    public void CarInReward()
    {
        /*if (_juCharacterController)
        {
            _juCharacterController.ToEnterVehicle = true;
            _juCharacterController.VehicleInAreaHelper = null;
                _juCharacterController.DriveVehicleAbility.VehicleToDrive =HelicopterController.Vehicle;
                if ( _juCharacterController.DriveVehicleAbility.VehicleToDrive != null)
                {
                    _juCharacterController.DriveVehicleAbility.VehicleDrivableNearby = true;
                }
                _juCharacterController.VehicleInArea = HelicopterController.Vehicle;
                _juCharacterController.VehicleInAreaHelper = HelicopterController.VehicleAlignmentHelper;
                _juCharacterController.DriveVehicleAbility.ManualEnterExit();
                RewardedPopController.m_CarIn -= CarInReward;
                _rewardedPopController.HidePopup();
                Checkpoints.SetActive(true);


        }*/
    }
    [ContextMenu("Car Out")]
    public void CarOut()
    {
        /*if (_juCharacterController)
        {
            if (_juCharacterController.DriveVehicleAbility)
                _juCharacterController.DriveVehicleAbility.VehicleToDrive.IsHelicopter = false;
            _juCharacterController.DriveVehicleAbility.ManualEnterExit();
            _juCharacterController.VehicleInAreaHelper = null;
                if (_juCharacterController.DriveVehicleAbility)  
                    _juCharacterController.DriveVehicleAbility.VehicleDrivableNearby = false;
                _juCharacterController.VehicleInArea = null;
                _juCharacterController.VehicleInAreaHelper = null;
                _juCharacterController.ToEnterVehicle = false;
                _juCharacterController.IsDriving = false;
                if (_juCharacterController.DriveVehicleAbility)  
                    _juCharacterController.DriveVehicleAbility.VehicleToDrive = null;
            
        }*/
    }
}
