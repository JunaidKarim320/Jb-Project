using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarCollision : MonoBehaviour
{
    public GameObject ParentVehicle;
    public GameObject explodedVehicle;
    GameObject deadVehicle;

    public UnityEvent WinEvent;
    public Level_3 level;

    public int currentHit, MaxHit;
    public void Start()
    {
        level = Level_3.instance;
    }

    public void Dead()
  {
      deadVehicle =   Instantiate(explodedVehicle,transform.parent);
      deadVehicle.transform.SetPositionAndRotation(transform.position,transform.rotation);
      deadVehicle.transform.SetParent(null);
      level.NextStep(1f);
      Destroy(ParentVehicle);
  }
    
   
  private void OnTriggerEnter(Collider Check)
    {
        if (Check.gameObject.CompareTag("Vehicle") )
        {
            if (!Check.gameObject.GetComponentInParent<RCC_AICarController>())
            {
                currentHit++;
                if (currentHit >= MaxHit)
                {
                    Dead();
                    WinEvent?.Invoke();
                }
            }
        }
    }
    
     private void OnCollisionEnter(Collision Check)
        {
            if (Check.gameObject.CompareTag("Vehicle"))
            {
                if (!Check.gameObject.GetComponentInParent<RCC_AICarController>())
                {
                    currentHit++;
                    if (currentHit >= MaxHit)
                    {
                        Dead();
                        WinEvent?.Invoke();
                    }
                }
            }
        }
}
