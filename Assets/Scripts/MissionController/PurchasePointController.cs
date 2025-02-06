using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;

public class PurchasePointController : MonoBehaviour
{
    public string MessagePopup;
    private AlertMessageConroller AlertMessageConroller;
    //private JUCharacterController _juCharacterController;

    void Start()
    {
          AlertMessageConroller = AlertMessageConroller.instance;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /*_juCharacterController = other.GetComponent<JUCharacterController>();
            if (_juCharacterController)*/
            {
                AlertMessageConroller.ShowPop(MessagePopup,5f);
            }
        }
    }
}
