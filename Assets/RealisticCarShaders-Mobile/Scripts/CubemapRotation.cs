//______________________________________________//
//________Realistic Car Shaders - Mobile________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//
using UnityEngine;
using System.Collections;

public class CubemapRotation : MonoBehaviour {

    //
    // This script rotate the cubemap as fast as your car goes. Modify it to work with your car controller script. Drag and drop this script on a 3D model that have RCS-Mobile material.
    //

    //private CarController player; // car controller script that have your car's current speed value
    public GameObject car; // car gameobject

    private float rotation = 1;
    Matrix4x4 m4 = new Matrix4x4();
    Quaternion rot;
    void Start()
    {
        //player = car.GetComponent<CarController>();
    }
    void FixedUpdate()
    {
        //rotation = rotation + player.speed / 100;
        rot = Quaternion.Euler(rotation, 0, 0);
        m4.SetTRS(Vector3.zero, rot, new Vector3(1, 1, 1));
        GetComponent<Renderer>().material.SetMatrix("_Rotation", m4); // set rotation value in RCS-Mobile material
    }
}
