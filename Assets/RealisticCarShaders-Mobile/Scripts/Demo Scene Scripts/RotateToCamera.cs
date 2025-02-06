//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    public GameObject mainCamera;
    private Vector3 gameobjectRotate;
    private Vector3 mainCameraVector;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        gameobjectRotate = gameObject.transform.rotation.eulerAngles;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            mainCameraVector = mainCamera.transform.rotation.eulerAngles;
            gameObject.transform.rotation = Quaternion.Euler(gameobjectRotate.x, mainCameraVector.y, gameobjectRotate.z);
        }
        else
        {
            Debug.LogError("Could not find Main Camra. Please add a Main Camera or set it manually for -Reflection Camera- gameobject.", mainCamera);
        }
    }
}