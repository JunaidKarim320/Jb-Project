using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlySpawner : MonoBehaviour
{
    public GameObject[] FriendlyPrefab;

    public Transform[] SpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        SpawnFriends();
    }

    public void SpawnFriends()
    {
        for (int i = 0; i < SpawnPosition.Length; i++)
        {
            GameObject g = Instantiate(FriendlyPrefab[Random.Range(0, FriendlyPrefab.Length)], SpawnPosition[i].position,
                SpawnPosition[i].rotation);
            
        }
    }
}
