using Spirit604.Gameplay.Services;
using UnityEngine;

namespace Spirit604.DotsCity.TestScene
{
    public class VehicleCustomStressManager : MonoBehaviour
    {
        [SerializeField]
        private VehicleCustomStressUI customStressUI;

        [SerializeField]
        private SceneService sceneService;

        [SerializeField]
        private VehicleCustomCameraGroupTracker vehicleCustomCameraGroupTracker;

        private void Awake()
        {
            customStressUI.OnExitClicked += CustomStressUI_OnExitClicked;
            vehicleCustomCameraGroupTracker.Initialize();
        }

        private void CustomStressUI_OnExitClicked()
        {
            vehicleCustomCameraGroupTracker.SwitchEnabledState(false);
            sceneService.LoadScene(0);
        }
    }
}
