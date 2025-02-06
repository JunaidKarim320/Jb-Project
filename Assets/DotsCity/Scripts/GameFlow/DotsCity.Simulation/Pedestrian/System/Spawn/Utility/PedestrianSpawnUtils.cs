using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    public static class PedestrianSpawnUtils
    {
        public struct SpawnRequestParams
        {
            public Entity PedestrianEntity;
            public Entity SourceEntity;
            public Entity DstEntity;
            public uint BaseSeed;
            public bool GroupSpawn;
            public bool HasTarget;
            public ComponentLookup<LocalToWorld> WorldTransformLookup;
            public ComponentLookup<NodeSettingsComponent> NodeSettingsLookup;
            public ComponentLookup<NodeLightSettingsComponent> NodeLightSettingsLookup;
            public NativeParallelHashMap<Entity, int> CapacityNodeHashMap;

            public float3 SourcePosition => WorldTransformLookup[SourceEntity].Position;

            public float3 TargetPosition => WorldTransformLookup[DstEntity].Position;

            public NodeSettingsComponent SourceNodeSettings => NodeSettingsLookup[SourceEntity];
            public NodeSettingsComponent TargetNodeSettings => NodeSettingsLookup[DstEntity];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpawnParams GetSpawnParams(
            ref EntityCommandBuffer commandBuffer,
            ref SpawnRequestParams spawnRequestParams)
        {
            var sourceEntity = spawnRequestParams.SourceEntity;
            var dstEntity = spawnRequestParams.DstEntity;

            var seed = MathUtilMethods.ModifySeed(spawnRequestParams.BaseSeed, sourceEntity.Index * dstEntity.Index);
            var randomGen = new Random(seed);
            int side = randomGen.NextInt(0, 2);

            if (side == -1)
            {
                var temp = sourceEntity;
                sourceEntity = dstEntity;
                dstEntity = temp;

                spawnRequestParams.DstEntity = dstEntity;
                spawnRequestParams.SourceEntity = sourceEntity;
            }

            var sourcePosition = spawnRequestParams.SourcePosition;
            var targetPosition = spawnRequestParams.TargetPosition;

            float3 direction = math.normalize(targetPosition - sourcePosition);
            quaternion spawnRotation = quaternion.LookRotationSafe(direction, math.up());

            sourceEntity = spawnRequestParams.SourceEntity;
            dstEntity = spawnRequestParams.DstEntity;

            var sourceLightEntity = spawnRequestParams.NodeLightSettingsLookup[sourceEntity].LightEntity;
            var targetLightEntity = spawnRequestParams.NodeLightSettingsLookup[dstEntity].LightEntity;

            var sourceNodeType = PedestrianNodeType.Default;

            if (!spawnRequestParams.GroupSpawn)
            {
                sourceNodeType = spawnRequestParams.NodeSettingsLookup[sourceEntity].NodeType;
            }

            float3 spawnPosition = default;

            PedestrianEntitySpawnerSystem.CaptureNodeInfo captureNodeInfo = default;

            #region Internal type helpers

            seed = MathUtilMethods.ModifySeed(spawnRequestParams.BaseSeed, spawnRequestParams.PedestrianEntity.Index);
            randomGen.InitState(seed);

            switch (sourceNodeType)
            {
                case PedestrianNodeType.Default:
                    {
                        spawnPosition = DefaultNodeSpawnHelper.GetSpawnPositionAndSetCustomParams(ref commandBuffer, ref spawnRequestParams, randomGen);
                        break;
                    }
                case PedestrianNodeType.House:
                    {
                        spawnPosition = HouseNodeSpawnHelper.GetSpawnPositionAndSetCustomParams(ref commandBuffer, ref spawnRequestParams);
                        break;
                    }
                case PedestrianNodeType.Sit:
                    {
                        spawnPosition = SeatNodeSpawnHelper.GetSpawnSpawnPositionAndSetCustomParams(ref spawnRequestParams, ref captureNodeInfo);
                        break;
                    }
                default:
                    {
                        spawnPosition = DefaultNodeSpawnHelper.GetSpawnPositionAndSetCustomParams(ref commandBuffer, ref spawnRequestParams, randomGen);
                        break;
                    }
            }

            #endregion

            RigidTransform rigidTransform = new RigidTransform(spawnRotation, spawnPosition);

            DestinationComponent destinationComponent = default;

            bool hasTarget = spawnRequestParams.HasTarget;

            if (captureNodeInfo.CapturedNodeEntity != Entity.Null)
            {
                destinationComponent = new DestinationComponent
                {
                    Value = sourcePosition,
                    PreviousDestination = targetPosition,
                    DestinationNode = sourceEntity,
                    PreviuosDestinationNode = dstEntity,
                    DestinationLightEntity = sourceLightEntity,
                    PreviousLightEntity = targetLightEntity,
                };
            }
            else
            {
                destinationComponent = new DestinationComponent
                {
                    Value = targetPosition,
                    PreviousDestination = sourcePosition,
                    DestinationNode = dstEntity,
                    PreviuosDestinationNode = sourceEntity,
                    DestinationLightEntity = targetLightEntity,
                    PreviousLightEntity = sourceLightEntity,
                };
            }

            if (hasTarget)
            {
                commandBuffer.SetComponentEnabled<HasTargetTag>(spawnRequestParams.PedestrianEntity, true);
            }

            return new SpawnParams(seed, destinationComponent, rigidTransform, captureNodeInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetRandomSpawnPosition(float3 sourcePosition, float3 targetPosition, Random random)
        {
            float lerpValue = random.NextFloat(0f, 1f);
            float3 spawnPosition = math.lerp(sourcePosition, targetPosition, lerpValue);
            return spawnPosition;
        }
    }
}
