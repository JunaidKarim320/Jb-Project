using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

public class QuailtyManager : MonoBehaviour
{
    public GameObject DynamicObjects;
  //  public Volume volume;
    public Camera camera;
    public GameObject[] wantedObjects;
     //AIContoller AiContoller;
    private void Awake()
    {
       
    }

    private void Start()
    {
        
        //AiContoller = FindObjectOfType<AIContoller>();

#if !UNITY_EDITOR
        setScreenResolution();
        setQuality();
#endif
        
  
    }

    public void setScreenResolution()
    {
        
        if (SystemInfo.systemMemorySize <=512)
        {
            int x = (int)(Screen.width * 0.2);
            int y = (int)(Screen.height * 0.2);
            Screen.SetResolution(x,y,true);
        }
        if (SystemInfo.systemMemorySize > 512 && SystemInfo.systemMemorySize <= 1024)
        {
            int x = (int)(Screen.width * 0.5);
            int y = (int)(Screen.height * 0.5);
            Screen.SetResolution(x,y,true);
        }
        if (SystemInfo.systemMemorySize > 1024 && SystemInfo.systemMemorySize <= 4096)
        {
            int x = (int)(Screen.width *  0.8);
            int y = (int)(Screen.height * 0.8);
            Screen.SetResolution(x,y,true);
        }
    }

    public void setQuality()
    {

        
        if (SystemInfo.systemMemorySize <=4096)
        {
            /*if (AiContoller)
            {
                AiContoller.maxVehicles = 3;
                AiContoller.maxHumans = 3;
            }*/

            DynamicObjects.SetActive(false);
            RenderSettings.fog = false;
            RenderSettings.reflectionIntensity = 0f;
           // volume.weight = 0;
           
            for (int i = 0; i < wantedObjects.Length; i++)
            {
                wantedObjects[i].SetActive(false);
            }
            camera.farClipPlane = 350f;
          //  camera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;
            Application.targetFrameRate = 30;
       }
        if (SystemInfo.systemMemorySize > 4096 && SystemInfo.systemMemorySize <= 5120)
        {
            /*if (AiContoller)
            {
                AiContoller.maxVehicles = 5;
                AiContoller.maxHumans = 5;
            }*/
        
            DynamicObjects.SetActive(true);
            RenderSettings.fog = true;
         //   volume.weight = 1;
            camera.farClipPlane = 500f;
            Application.targetFrameRate = 60;
        }
        if (SystemInfo.systemMemorySize > 5120 )
        {
            /*if (AiContoller)
            {
                AiContoller.maxVehicles = 7;
                AiContoller.maxHumans = 10;
            }*/
        
            DynamicObjects.SetActive(true);
            RenderSettings.fog = true;
          //  volume.weight = 1;
            camera.farClipPlane = 700f;
             Application.targetFrameRate = 60;
         }
    }

}
