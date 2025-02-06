using Spirit604.DotsCity.Core;
using Spirit604.Gameplay.Services;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.TestScene
{
    public class VehicleCustomStressSceneInstaller : MonoBehaviour
    {
        [SerializeField]
        private VehicleCustomStressUI vehicleCustomStressUI;

        [SerializeField]
        private EntityWorldService entityWorldService;

        [SerializeField]
        private SceneService sceneService;

        private void Start()
        {
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<VehicleLineTracker>().Initialize(vehicleCustomStressUI);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<VehicleStressSpawner>().Initialize(vehicleCustomStressUI);
            sceneService.Construct(entityWorldService);
        }
    }
}
