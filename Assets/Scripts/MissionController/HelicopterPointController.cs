using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS.CameraSystems;
//using JUTPS.VehicleSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterPointController : MonoBehaviour
{
    
    public GameObject HelicopterPrefab;

    public Transform SpawnPoint;
    
    public Transform ExitPoint;
    public GameObject[] Checkpoints;
    [HideInInspector]
    public GameObject SelectedCheckpoint;

    //public Vehicle Vehicle;
    //public VehicleAlignmentHelper VehicleAlignmentHelper;
    public HelicopterTrigger HelicopterTrigger;

    public static HelicopterPointController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InstantiateAirplane();
    }



    public void InstantiateAirplane()
    {
        if(!HelicopterPrefab)
            return;
        
        GameObject g= Instantiate(HelicopterPrefab, SpawnPoint.position, SpawnPoint.rotation);

     //   g.GetComponent<CarController>().ExitTransform = ExitPoint;
        /*g.GetComponent<HelicopterControllerHelper>()._spawnTransform = SpawnPoint;
        Vehicle = g.GetComponent<Vehicle>();
        VehicleAlignmentHelper = Vehicle.GetComponent<VehicleAlignmentHelper>();*/ 
    }
    public GameObject Checkpoint()
    {
        SelectedCheckpoint = Checkpoints[Random.Range(0, Checkpoints.Length)];
        //Vehicle.GetComponent<HelicopterControllerHelper>().Checkpoint = SelectedCheckpoint;
        return SelectedCheckpoint;
    }
}
