using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    public string[] Objectives;

     int currentObjective;
    
    private ObjectiveController _objectiveController;
    // Start is called before the first frame update
    void Start()
    {
        _objectiveController = FindObjectOfType<ObjectiveController>();
        _objectiveController.ShowObjective(Objectives[currentObjective]);
        
    }

 
}
