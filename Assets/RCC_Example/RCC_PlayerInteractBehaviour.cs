using Spirit604.DotsCity.Samples.PlayerInteract;
using UnityEngine;

public class RCC_PlayerInteractBehaviour : PlayerInteractorConvertExample
{
    protected override GameObject GetCarRootFromCollider(GameObject car)
    {
        return car.GetComponentInParent<RCC_CarControllerV3>().gameObject;
    }
}
