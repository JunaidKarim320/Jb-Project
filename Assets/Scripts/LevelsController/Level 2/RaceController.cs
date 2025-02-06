using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaceController : MonoBehaviour
{
    public UnityEvent WinEvent,LoseEvent;
    
    private void OnTriggerEnter(Collider Check)
    {
        if (Check.CompareTag("Player")  )
        {
            WinEvent?.Invoke();
        }
        if (Check.gameObject.CompareTag("Vehicle") )
        {
            WinEvent?.Invoke();
        }
        if (Check.gameObject.CompareTag("Enemy") )
        {
            LoseEvent?.Invoke();
        }
    }
}
