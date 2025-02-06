#if FMOD
using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Sound
{
    [UpdateInGroup(typeof(LateInitGroup))]
    [BurstCompile]
    public partial struct FMODCleanupSoundSystem : ISystem
    {
        private EntityQuery cleanupQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            cleanupQuery = SystemAPI.QueryBuilder()
                .WithNone<SoundComponent>()
                .WithAll<FMODSound>()
                .Build();

            state.RequireForUpdate(cleanupQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var stopSoundJob = new StopSoundJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                DestroyEntity = false,
            };

            stopSoundJob.Schedule(cleanupQuery);
        }
    }
}
#endif