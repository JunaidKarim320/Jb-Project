using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimationEvents : MonoBehaviour
{
    public Level myLevel; 
    
    public void OnCutSceneCompletion()
    {
        myLevel.StepComplete();
    }
}
