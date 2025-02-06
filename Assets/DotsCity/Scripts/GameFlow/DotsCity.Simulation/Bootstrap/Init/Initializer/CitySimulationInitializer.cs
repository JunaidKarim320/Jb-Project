using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Config;
using Spirit604.DotsCity.Simulation.Level.Props;
using Spirit604.DotsCity.Simulation.VFX;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Initialization
{
    public class CitySimulationInitializer : InitializerBase
    {
        private GeneralSettingDataSimulation generalSettingDataSimulation;
        private VFXFactory vfxFactory;

        [InjectWrapper]
        public void Construct(
            GeneralSettingDataSimulation generalSettingDataSimulation,
            VFXFactory vfxFactory)
        {
            this.generalSettingDataSimulation = generalSettingDataSimulation;
            this.vfxFactory = vfxFactory;
        }

        public override void Initialize()
        {
            base.Initialize();

            var world = World.DefaultGameObjectInjectionWorld;

            if (generalSettingDataSimulation.CullStaticPhysics)
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CullStaticPhysicsSystem>(true);
            }
            else
            {
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CleanStaticPhysicsSystem>(true);
            }

            world.GetOrCreateSystemManaged<HydrantPropDamageSystem>().Initialize(vfxFactory);
        }
    }
}