using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
//using JUTPS;
using UnityEngine;
using Invector;

public class ShopTrigger : MonoBehaviour
{
    public bool ClothShop;
    public Transform PlayerPosition;
    public Transform ExitPosition;
    private InGameShopController _InGameShopController;
    //private JUCharacterController _juCharacterController;
    private vThirdPersonController _vThirdPersonController;


    // Start is called before the first frame update
    void Start()
    {
        _InGameShopController = InGameShopController.instance;
    }

    public void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered");
            _vThirdPersonController = other.GetComponent<vThirdPersonController>();
            if (_vThirdPersonController)
            {
                _vThirdPersonController.transform.SetPositionAndRotation(PlayerPosition.position, PlayerPosition.rotation);
                if (ClothShop)
                {
                    _InGameShopController.EnterClothShop();
                    _InGameShopController.ExitClothShopPosition = ExitPosition;
                }
                else
                {
                    _InGameShopController.EnterShop();
                    _InGameShopController.ExitPosition = ExitPosition;
                }
            }
            Debug.Log("Player Exited");
        }
    }
}
