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
using UnityEngine.UI;

public class DragDemoSceneUIScript : MonoBehaviour {

    public Camera reflectionCam;
    public Slider slider;
    public void UpdateSliderValue()
    {
        reflectionCam.fieldOfView = slider.value;
    }
}
