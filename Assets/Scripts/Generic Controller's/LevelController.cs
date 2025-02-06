using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;
using UnityEngine.Events;

public class LevelController : MonoBehaviour
{
    
    public UnityEvent Action;
     LevelManager levelManager;

    public void Start()
    {
        levelManager = LevelManager.instance;
        if (ApplicationController.LastSelectedLevel == 0)
        {
            levelManager.StartLevel();
            transform.parent.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider Check)
    {
        if (Check.CompareTag("Player")  )
        {
            levelManager.StartLevel();
            Action?.Invoke();
            transform.parent.gameObject.SetActive(false);

        }
    }

   
}
