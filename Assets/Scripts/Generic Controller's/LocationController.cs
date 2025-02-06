using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LocationController : MonoBehaviour
{

    public GameObject[] Locations;
    public string[] LocationsName;
    public CinemachineVirtualCamera[] LocationUnlockCamera;
    public static LocationController instance;

    public void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        // if (ApplicationController.LastSelectedLevel > 2)
        // {
        //     UnlockLocation(3);
        // }
        // if (ApplicationController.LastSelectedLevel > 3)
        // {
        //     UnlockLocation(4);
        //     UnlockLocation(5);
        // }
        // if (ApplicationController.LastSelectedLevel > 4)
        // {
        //     UnlockLocation(6);
        //     UnlockLocation(7);
        // }

        if (ApplicationController.LastSelectedLevel >= 5)
        {
            ApplicationController.AllMissionPoint = 1;
        }
        if (ApplicationController.AllMissionPoint ==1)
        {
            for (int i = 0; i < Locations.Length; i++)
            {
                UnlockLocation(i);
            }
        }
    }
    public void UnlockAllLocation()
    {
        for (int i = 0; i < Locations.Length; i++)
        {
            UnlockLocation(i);
        }
    }

    // Update is called once per frame
    public void UnlockLocation(int index)
    {
        Locations[index].SetActive(true);
    }
    
    public void UnlockLocation(int index,bool Camera)
    {
        Locations[index].SetActive(true);
        if (Camera)
        {
            if(LocationUnlockCamera[index])
            LocationUnlockCamera[index].gameObject.SetActive(true);
        }
    }
}
