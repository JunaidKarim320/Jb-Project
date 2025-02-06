using Spirit604.DotsCity.Simulation.Road;
using Unity.Burst;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.TrafficArea
{
    [UpdateInGroup(typeof(TrafficAreaSimulationGroup))]
    [BurstCompile]
    public partial struct TrafficAreaLockSystem : ISystem
    {
        private EntityQuery updateQuery;

        [BurstCompile]
        void ISystem.OnCreate(ref SystemState state)
        {
            updateQuery = SystemAPI.QueryBuilder()
                .WithAll<TrafficAreaUpdateLockStateTag>()
                .Build();

            state.RequireForUpdate(updateQuery);
        }

        [BurstCompile]
        void ISystem.OnUpdate(ref SystemState state)
        {
            var lockJob = new LockJob()
            {
                CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                NodeSettingsLookup = SystemAPI.GetComponentLookup<TrafficNodeSettingsComponent>(false),
                NodeCapacityLookup = SystemAPI.GetComponentLookup<TrafficNodeCapacityComponent>(false),
            };

            lockJob.Schedule();
        }

        [WithAll(typeof(TrafficAreaUpdateLockStateTag))]
        [BurstCompile]
        public partial struct LockJob : IJobEntity
        {
            public EntityCommandBuffer CommandBuffer;
            public ComponentLookup<TrafficNodeSettingsComponent> NodeSettingsLookup;
            public ComponentLookup<TrafficNodeCapacityComponent> NodeCapacityLookup;

            void Execute(
                Entity entity,
                ref DynamicBuffer<TrafficAreaEnterCarQueueElement> enterQueue,
                ref DynamicBuffer<TrafficAreaEnterNodeElement> enterNodes,
                ref TrafficAreaComponent trafficAreaComponent)
            {
                int queueLength = enterQueue.Length;

                bool locked = queueLength >= trafficAreaComponent.MaxEntryQueueCount;

                if (trafficAreaComponent.Locked != locked)
                {
                    if (locked)
                    {
                        for (int i = 0; i < enterNodes.Length; i++)
                        {
                            var enterNodeEntity = enterNodes[i].NodeEntity;

                            var enterNodeSettings = NodeSettingsLookup[enterNodeEntity];
                            enterNodeSettings.IsAvailableForSpawn = false;
                            enterNodeSettings.IsAvailableForSpawnTarget = false;
                            NodeSettingsLookup[enterNodeEntity] = enterNodeSettings;

                            var enterNodeCapacity = NodeCapacityLookup[enterNodeEntity];
                            enterNodeCapacity.Capacity = 0;
                            NodeCapacityLookup[enterNodeEntity] = enterNodeCapacity;
                        }

                        trafficAreaComponent.Locked = true;
                    }
                    else
                    {
                        for (int i = 0; i < enterNodes.Length; i++)
                        {
                            var enterNodeEntity = enterNodes[i].NodeEntity;

                            var enterNodeSettings = NodeSettingsLookup[enterNodeEntity];
                            enterNodeSettings.IsAvailableForSpawn = true;
                            enterNodeSettings.IsAvailableForSpawnTarget = true;

                            NodeSettingsLookup[enterNodeEntity] = enterNodeSettings;

                            var enterNodeCapacity = NodeCapacityLookup[enterNodeEntity];
                            enterNodeCapacity.Capacity = -1;
                            NodeCapacityLookup[enterNodeEntity] = enterNodeCapacity;
                        }

                        trafficAreaComponent.Locked = false;
                    }
                }

                CommandBuffer.SetComponentEnabled<TrafficAreaUpdateLockStateTag>(entity, false);
            }
        }
    }
}