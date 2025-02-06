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

public class SideScroller : MonoBehaviour {

    public float carSpeed;
    public Material reflectiveMaterials;

    void Update()
    {
        float offset = Time.time * carSpeed;
        reflectiveMaterials.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
