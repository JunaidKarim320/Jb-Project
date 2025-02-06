using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;
using Random = UnityEngine.Random;

public class AmbulanceTrigger : MonoBehaviour
{
    public HospitalPointController HospitalPointController;
    public GameObject Checkpoints;

    //private JUCharacterController _juCharacterController;
    private RewardedPopController _rewardedPopController;

    IEnumerator Start()
    {
        HospitalPointController = GetComponentInParent<HospitalPointController>();
        _rewardedPopController = RewardedPopController.instance;
        yield return new WaitForSeconds(1f);

        Checkpoints = HospitalPointController.Checkpoint();
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
        //if (_juCharacterController)
        {
            /*_juCharacterController.ToEnterVehicle = true;
            _juCharacterController.VehicleInAreaHelper = null;
            _juCharacterController.DriveVehicleAbility.VehicleToDrive = HospitalPointController.Vehicle;
            if (_juCharacterController.DriveVehicleAbility.VehicleToDrive != null)
            {
                _juCharacterController.DriveVehicleAbility.VehicleDrivableNearby = true;
            }
            _juCharacterController.VehicleInArea = HospitalPointController.Vehicle;
            _juCharacterController.VehicleInAreaHelper = HospitalPointController.VehicleAlignmentHelper;
            _juCharacterController.DriveVehicleAbility.ManualEnterExit();*/
            RewardedPopController.m_CarIn -= CarInReward;
            _rewardedPopController.HidePopup();
                Checkpoints.SetActive(true);

        }
    }
}
