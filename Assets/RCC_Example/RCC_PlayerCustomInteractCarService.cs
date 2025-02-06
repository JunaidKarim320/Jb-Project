using Spirit604.DotsCity.Samples.PlayerInteract;
using UnityEngine;

public class RCC_PlayerCustomInteractCarService : PlayerCustomInteractCarServiceBase
{
    protected override void InitCustomComponents(GameObject enteredCar)
    {
        var controller = enteredCar.GetComponent<RCC_CarControllerV3>();
        controller.DisableOverrideInputs(true);
        controller.steeringType = RCC_CarControllerV3.SteeringType.Curve;
        controller.useSteeringLimiter = true;
        controller.canControl = true;

        var playerCarBehaviour = enteredCar.AddComponent<RCC_PlayerCarBehaviour>();
        playerCarBehaviour.Init();
    }
}
