using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvents : MonoBehaviour
{
    public Level myLevel; 
    
     

        void Start()
        {
            myLevel = transform.GetComponentInParent<Level>();
        }
    public void OnCompletion()
    {
       myLevel.NextStep();
    }
}
