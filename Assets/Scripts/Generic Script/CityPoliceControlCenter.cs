using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPoliceControlCenter : MonoBehaviour
{
    //private CreateAI AICScript;
    public int MaxPersonKilled;
    public int currentPersonKilled;

    
    
    public static CityPoliceControlCenter instance;
    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        //AICScript = CreateAI.instance;
    }


    public void AlertPoliceCenter()
    {
        ApplicationController.currentPersonKilled = ApplicationController.currentPersonKilled +1;
        currentPersonKilled = ApplicationController.currentPersonKilled;
        
        if (currentPersonKilled >= MaxPersonKilled)
        {
           // CreatePoliceMan();
        }
    }

    public void CreatePoliceMan()
    {
        //AICScript.CreatePoliceMan();
        ApplicationController.currentPersonKilled = 0;
    }

    
    
    
}
