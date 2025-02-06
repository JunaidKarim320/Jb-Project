using Spirit604.DotsCity.Simulation.Car;
using Spirit604.Extensions;
using UnityEngine;

public class RCC_VehicleInputAdapter : MonoBehaviour, IVehicleInput
{
    [SerializeField] private RCC_CarControllerV3 controller;

    private RCC_Inputs inputs = new RCC_Inputs();
    private bool handbrakeLock;

    private void Awake()
    {
        if (controller)
        {
            controller.OverrideInputs(inputs);
        }
        else
        {
            Debug.LogError($"RCC_VehicleInputAdapter. Vehicle {name} RCC_CarControllerV3 not assigned");
        }
    }

    public float Throttle
    {
        get => (float)inputs.throttleInput;
        set
        {
            handbrakeLock = false;

            if (value > 0)
            {
                inputs.throttleInput = value;
                inputs.brakeInput = 0;
            }
            else
            {
                if (value == 0)
                {
                    inputs.throttleInput = 0;
                    inputs.brakeInput = 0;
                    handbrakeLock = true;
                    inputs.handbrakeInput = 1;
                }
                else
                {
                    inputs.throttleInput = 0;
                    inputs.brakeInput = Mathf.Abs(value);
                }
            }
        }
    }

    public float Steering { get => inputs.steerInput; set => inputs.steerInput = value; }

    public bool Handbrake
    {
        get => inputs.handbrakeInput == 1;
        set
        {
            if (!handbrakeLock)
            {
                inputs.handbrakeInput = value ? 1 : 0;
            }
        }
    }

    public void SwitchEnabledState(bool isEnabled)
    {
        controller.canControl = isEnabled;
    }

    private void Reset()
    {
        controller = GetComponent<RCC_CarControllerV3>();

        if (controller)
        {
            controller.useSteeringLimiter = false;
            controller.steeringType = RCC_CarControllerV3.SteeringType.Constant;
            EditorSaver.SetObjectDirty(controller);
            EditorSaver.SetObjectDirty(this);
        }
    }
}
