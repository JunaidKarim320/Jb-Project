using System.Collections;
using System.Collections.Generic;
//using JUTPS.AI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    public GameObject Gang1Prefab, Gang2Prefab;
    public Transform[] Gang1, Gang2;

    public int TotalEnemies;

    public UnityEvent MissionCompleted;

    //public List<PatrolAI> Enemies;
    public Level_4 currentLevel;
    
     public void Start()
        {
       // GangSpawn();
        }
    public void GangSpawn()
    {
        for (int i = 0; i < Gang1.Length; i++)
        {
            GameObject Gangmember= Instantiate(Gang1Prefab, Gang1[i]);
            Gangmember.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
            TotalEnemies = TotalEnemies + 1;
            //Enemies.Add(Gangmember.GetComponent<PatrolAI>());
        }
        for (int i = 0; i < Gang2.Length; i++)
        {
            GameObject Gangmember= Instantiate(Gang2Prefab, Gang2[i]);
            Gangmember.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
            TotalEnemies = TotalEnemies + 1;
            //Enemies.Add(Gangmember.GetComponent<PatrolAI>());
        }
    }
    

    public void CountEnemies()
    {
        TotalEnemies = TotalEnemies - 1;

        if (TotalEnemies == 7)
        {
            EnableAI();
        }

        if (TotalEnemies <= 0)
        {
            currentLevel.NoSniping();
          Invoke(nameof(InvokeComplete),1f);  //InvokeComplete()
           // ApplicationController.AllMissionPoint = 1;
        }
    }
    [ContextMenu(" Kill All Enemies")]
    public void KillAllEnemies()
    {
        /*for (int i = 0; i < Enemies.Count; i++)
        {
            Destroy(Enemies[i]);
        }*/
    }
    public void EnableAI()
    {
        /*for (int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].enabled = true;
        }*/
    }

    public void InvokeComplete()
    {
        MissionCompleted?.Invoke();
    }

}
