using Spirit604.DotsCity.Samples.PlayerInteract;
using Spirit604.Extensions;
using UnityEngine;

public class RCC_PlayerCarBehaviour : PlayerCarBehaviourExample
{
    [SerializeField] private RCC_CarControllerV3 controller;

    protected override void EnableInput()
    {
        controller.canControl = true;
    }

    protected override void DisableInput()
    {
        controller.canControl = false;
    }

    public override void Init()
    {
        Reset();
    }

    private void Reset()
    {
        controller = GetComponent<RCC_CarControllerV3>();
        EditorSaver.SetObjectDirty(this);
    }
}
