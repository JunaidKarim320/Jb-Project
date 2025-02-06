using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS.AI;
using UnityEngine;

public class EnemyCountID : MonoBehaviour
{

    public EnemySpawner EnemySpawner;

    public void Start()
    {
        EnemySpawner = FindObjectOfType<EnemySpawner>();
      
    }

    void OnDisable()
    {
        //EnemySpawner.Enemies.Remove(GetComponent<PatrolAI>());
        EnemySpawner.CountEnemies();
    }

}
