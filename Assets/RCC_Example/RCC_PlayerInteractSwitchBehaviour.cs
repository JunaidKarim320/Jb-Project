using Spirit604.DotsCity.Samples.PlayerInteract;
using Spirit604.DotsCity.Simulation.Mono;
using UnityEngine;

public class RCC_PlayerInteractSwitchBehaviour : PlayerInteractorSwitchExample
{
    protected override GameObject GetCarRootFromCollider(GameObject car)
    {
        if (car.TryGetComponent<ArcadeVehicleController>(out var arcade))
        {
            return arcade.gameObject;
        }

        return car.GetComponentInParent<RCC_CarControllerV3>().gameObject;
    }
}
