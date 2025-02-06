using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS.CameraSystems;
//using JUTPS.VehicleSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class AirplanePointController : MonoBehaviour
{
    
    public GameObject AirplanePrefab;

    public Transform SpawnPoint;
    
    public Transform ExitPoint;
    public GameObject[] Checkpoints;
    [HideInInspector]
    public GameObject SelectedCheckpoint;

    //public Vehicle Vehicle;
    //public VehicleAlignmentHelper VehicleAlignmentHelper;

    public static AirplanePointController instance;

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
        if(!AirplanePrefab)
            return;
        
        GameObject g= Instantiate(AirplanePrefab, SpawnPoint.position, SpawnPoint.rotation);

       // g.GetComponent<CarController>().ExitTransform = ExitPoint;
        /*g.GetComponent<SimpleAirPlaneControllerHelper>()._spawnTransform = SpawnPoint;
        Vehicle = g.GetComponent<Vehicle>();
        VehicleAlignmentHelper = Vehicle.GetComponent<VehicleAlignmentHelper>();*/ 
    }
    public GameObject Checkpoint()
    {
        SelectedCheckpoint = Checkpoints[Random.Range(0, Checkpoints.Length)];
        return SelectedCheckpoint;
    }
}
