using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Sound;
using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Car.Sound
{
    [WithAll(typeof(HasSoundTag))]
    [BurstCompile]
    public partial struct DisableSoundJob : IJobEntity
    {
        public EntityCommandBuffer CommandBuffer;

        void Execute(
            ref CarSoundData carSoundData,
            EnabledRefRW<HasSoundTag> hasSoundTagRW)
        {
            if (carSoundData.WaitForInit)
            {
                return;
            }

            if (carSoundData.SoundEntity != Entity.Null)
            {
                PoolEntityUtils.DestroyEntity(ref CommandBuffer, carSoundData.SoundEntity);
                carSoundData.SoundEntity = Entity.Null;
            }

            hasSoundTagRW.ValueRW = false;
        }
    }
}
