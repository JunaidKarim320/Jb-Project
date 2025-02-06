using System.Collections;
using System.Collections.Generic;
/*using JUTPS.CameraSystems;
using JUTPS.VehicleSystem;*/
using UnityEngine;

public class BoatPointController : MonoBehaviour
{ 
    public GameObject BoatPrefab;

    public Transform SpawnPoint;

    public Transform ExitPoint;
    public GameObject[] Checkpoints;
    [HideInInspector]
    public GameObject SelectedCheckpoint;

    /*public Vehicle Vehicle;
    public VehicleAlignmentHelper VehicleAlignmentHelper;*/
    // Start is called before the first frame update
    void Start()
    {
        if(!BoatPrefab)
            return;
        
       GameObject g= Instantiate(BoatPrefab, SpawnPoint.position, SpawnPoint.rotation);
       /*g.GetComponent<CarController>().ExitTransform = ExitPoint;
       g.GetComponent<BoatHelper>()._spawnTransform = SpawnPoint;
       Vehicle = g.GetComponent<Vehicle>();
       VehicleAlignmentHelper = Vehicle.GetComponent<VehicleAlignmentHelper>();*/
    }

    public GameObject Checkpoint()
    {
        SelectedCheckpoint = Checkpoints[Random.Range(0, Checkpoints.Length)];
        //Vehicle.GetComponent<BoatHelper>().Checkpoint = SelectedCheckpoint;
        return SelectedCheckpoint;
    }
}
