using System.Collections;
using System.Collections.Generic;
/*using JUTPS.CameraSystems;
using JUTPS.VehicleSystem;*/
using UnityEngine;

public class HospitalPointController : MonoBehaviour
{
    public GameObject AmbulancePrefab;

    public Transform[] SpawnPoint;
    public GameObject[] Checkpoints;
    [HideInInspector]
    public GameObject SelectedCheckpoint;
    private List<GameObject> spawnedObjects;
    
    /*public Vehicle Vehicle;
    public VehicleAlignmentHelper VehicleAlignmentHelper;*/
    // Start is called before the first frame update
    void Start()
    {
        spawnedObjects = new List<GameObject>();
        
        for (int i = 0; i < SpawnPoint.Length; i++)
        {
          /*GameObject g = Instantiate(AmbulancePrefab, SpawnPoint[i].position, SpawnPoint[i].rotation);
          g.GetComponent<CarControllerHelper>()._spawnTransform = SpawnPoint[i];
          spawnedObjects.Add(g);
          Vehicle = g.GetComponent<Vehicle>();
          VehicleAlignmentHelper = Vehicle.GetComponent<VehicleAlignmentHelper>(); */
          
        }     
    }

    public GameObject Checkpoint()
    {
        SelectedCheckpoint = Checkpoints[Random.Range(0, Checkpoints.Length)];
        //Vehicle.GetComponent<CarControllerHelper>().Checkpoint = SelectedCheckpoint;
        return SelectedCheckpoint;
    }

}
