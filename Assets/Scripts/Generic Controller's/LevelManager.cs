using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public GameObject[] Levels;
    public int CustomLevel;
    public bool IsCustomLevel,IsStart;
    
    // Start is called before the first frame update

    public static LevelManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        if (IsCustomLevel && IsStart)
        {
            if (CustomLevel < Levels.Length)
                Levels[CustomLevel].SetActive(true);
        }
        else
        {
            //if (ApplicationController.LastSelectedLevel < Levels.Length)
            //    Levels[ApplicationController.LastSelectedLevel].SetActive(true);
        }
    }

    public void StartLevel()
    {
        if (IsCustomLevel)
        {
            if (CustomLevel < Levels.Length)
                Levels[CustomLevel].SetActive(true);
        }
        else
        {
            if (ApplicationController.LastSelectedLevel < Levels.Length)
                Levels[ApplicationController.LastSelectedLevel].SetActive(true);
        }
    }

}
