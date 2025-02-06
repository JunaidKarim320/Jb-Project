using Spirit604.DotsCity.Samples.PlayerInteract;
using UnityEngine;

public class RCC_PlayerCustomInteractCarSwitchService : PlayerCustomInteractCarExampleService2
{
    [SerializeField] private Canvas carCanvas;
    [SerializeField] private Canvas InvectorCanvas;

    protected override void Awake()
    {
        base.Awake();
        SwitchCarCanvasState(false);
        SwitchInvectorCanvasState(true);
    }

    public override GameObject ConvertCarBeforeEnter(GameObject enteredTrafficCar, GameObject enteredNPCObj)
    {
        var enteredCar = base.ConvertCarBeforeEnter(enteredTrafficCar, enteredNPCObj);
        RCC_SceneManager.Instance.activePlayerVehicle = enteredCar.GetComponent<RCC_CarControllerV3>();
        SwitchCarCanvasState(true);
        SwitchInvectorCanvasState(false);

        return enteredCar;
    }

    public override void ExitCar(GameObject exitPlayerCar, GameObject npcObj)
    {
        base.ExitCar(exitPlayerCar, npcObj);
        SwitchCarCanvasState(false);
        SwitchInvectorCanvasState(true);
    }

    private void SwitchCarCanvasState(bool isEnabled)
    {
        carCanvas.enabled = isEnabled;
    }
    private void SwitchInvectorCanvasState(bool isEnabled)
    {
        InvectorCanvas.enabled = isEnabled;
    }
}
