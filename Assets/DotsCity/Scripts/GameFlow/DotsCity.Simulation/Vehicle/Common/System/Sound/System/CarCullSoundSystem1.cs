using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Sound;
using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Car.Sound
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct CarCullSoundSystem1 : ISystem
    {
        private EntityQuery removeSoundQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            removeSoundQuery = SystemAPI.QueryBuilder()
                .WithNone<InViewOfCameraTag, CulledEventTag, CarCustomEnginePitchTag>()
                .WithAll<HasSoundTag>()
                .WithAllRW<CarSoundData>()
                .Build();

            state.RequireForUpdate(removeSoundQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var removeSoundJob = new DisableSoundJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
            };

            removeSoundJob.Schedule(removeSoundQuery);
        }
    }
}
