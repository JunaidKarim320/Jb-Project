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

[RequireComponent(typeof(Camera))]
public class ReflectionCamLimiter : MonoBehaviour
{
    public float FPS = 5f;
    private Camera renderCam;

    void Start()
    {
        renderCam = gameObject.GetComponent<Camera>();
        InvokeRepeating("Render", 0f, 1f / FPS);
    }
    void OnDestroy()
    {
        CancelInvoke ();
    }
    void Render()
    {
        renderCam.enabled = true;
    }
    void OnPostRender()
    {
        renderCam.enabled = false;
    }
}
