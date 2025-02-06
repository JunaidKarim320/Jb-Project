
using System.Collections;
//using JUTPS;
using UnityEngine;

public class BoatPointTrigger : MonoBehaviour
{
    public BoatPointController BoatPointController;
    public GameObject Player;
    //public JUCharacterController juCharacterController;
    private RewardedPopController _rewardedPopController;
    public GameObject Checkpoints;

    IEnumerator Start()
    {
        BoatPointController = GetComponentInParent<BoatPointController>();
        _rewardedPopController = RewardedPopController.instance;
        yield return new WaitForSeconds(1f);
        Checkpoints = BoatPointController.Checkpoint();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player = other.gameObject;
            //juCharacterController = Player.GetComponent<JUCharacterController>();
            _rewardedPopController.ShowPop();
            RewardedPopController.m_CarIn += CarInReward;
        }
    }

    public void CarInReward()
    {
        /*if (juCharacterController)
        {
            juCharacterController.ToEnterVehicle = true;
            juCharacterController.VehicleInAreaHelper = null;
            juCharacterController.DriveVehicleAbility.VehicleToDrive = BoatPointController.Vehicle;
            if ( juCharacterController.DriveVehicleAbility.VehicleToDrive != null)
            {
                juCharacterController.DriveVehicleAbility.VehicleDrivableNearby = true;
            }
            juCharacterController.VehicleInArea = BoatPointController.Vehicle;
            juCharacterController.VehicleInAreaHelper = BoatPointController.VehicleAlignmentHelper;
            juCharacterController.DriveVehicleAbility.ManualEnterExit();
            RewardedPopController.m_CarIn -= CarInReward;
            _rewardedPopController.HidePopup();
            Checkpoints.SetActive(true);


        }*/
    }
}
