#if FMOD
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Sound
{
    [UpdateInGroup(typeof(InitGroup))]
    public partial class FMODCleanupOnDestroySoundSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Entities
            .WithoutBurst()
            .ForEach((
                Entity entity,
                in FMODSound sound) =>
            {
                sound.Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                sound.Event.release();
            }).WithStructuralChanges().Run();
        }

        protected override void OnUpdate()
        {
        }
    }
}
#endif