using System;
using UnityEngine;
using System.Collections;
using SickscoreGames.HUDNavigationSystem;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{

	public bool ExcludeVehicle;

	public UnityEvent CheckpointActivated;
    
    private CheckpointManager checkpointManager;
    private HUDNavigationElement hudNavigationElement;
	// Use this for initialization
	void Start () {
		
			checkpointManager = GetComponentInParent<CheckpointManager>();
			hudNavigationElement = GetComponentInChildren<HUDNavigationElement>();

		if (checkpointManager)
		{
			if (checkpointManager.HUD)
			{
				if (hudNavigationElement)
				{
					hudNavigationElement.gameObject.SetActive(true);
				}
			}
			else
			{
				if (hudNavigationElement)
				{
					hudNavigationElement.gameObject.SetActive(false);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerEnter(Collider Check)
    {
	    if (Check.CompareTag("Player")  )
	    {
		    Debug.Log("Activate");
		   // if (CheckpointActivated != null)
			    CheckpointActivated?.Invoke();
	    }
	    if ( Check.gameObject.CompareTag("Vehicle") && !ExcludeVehicle )
	    {
		    if(Check.gameObject.GetComponentInParent<RCC_CarControllerV3>().enabled)
			    CheckpointActivated?.Invoke();
	    }
    }

    private void OnCollisionEnter(Collision Check)
    {
	    if ( Check.gameObject.CompareTag("Vehicle") && !ExcludeVehicle )
	    {
		    if(Check.gameObject.GetComponentInParent<RCC_CarControllerV3>().enabled)
			    CheckpointActivated?.Invoke();
	    }
    }
}
