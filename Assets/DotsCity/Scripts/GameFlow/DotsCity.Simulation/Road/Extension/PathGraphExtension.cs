using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Spirit604.DotsCity.Simulation.Road
{
    public static class PathGraphExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(in this PathGraphSystem.Singleton graph)
        {
            return graph.allPaths.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasOption(in this PathGraphSystem.PathData pathData, PathOptions option)
        {
            return DotsEnumExtension.HasFlagUnsafe(pathData.Options, option);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvailable(in this PathGraphSystem.Singleton graph, int pathIndex, in TrafficTypeComponent trafficTypeComponent)
        {
            ref readonly var pathData = ref graph.GetPathData(pathIndex);
            return IsAvailable(in pathData, trafficTypeComponent.TrafficGroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvailable(in this PathGraphSystem.PathData pathData, in TrafficTypeComponent trafficTypeComponent)
        {
            return IsAvailable(in pathData, trafficTypeComponent.TrafficGroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvailable(in this PathGraphSystem.PathData pathData, TrafficGroupType trafficGroupType)
        {
            return DotsEnumExtension.HasFlagUnsafe(pathData.TrafficGroup, trafficGroupType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvailable(in this PathGraphSystem.Singleton graph, int pathIndex, int nodeIndex, in TrafficTypeComponent trafficTypeComponent)
        {
            ref readonly var routeNode = ref graph.GetPathNodeData(pathIndex, nodeIndex);
            return IsAvailable(in routeNode, trafficTypeComponent.TrafficGroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvailable(in this PathGraphSystem.Singleton graph, in PathGraphSystem.PathData pathData, int nodeIndex, in TrafficTypeComponent trafficTypeComponent)
        {
            ref readonly var routeNode = ref graph.GetPathNodeData(in pathData, nodeIndex);
            return IsAvailable(in routeNode, trafficTypeComponent.TrafficGroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvailable(in this RouteNodeData routeNode, in TrafficTypeComponent trafficTypeComponent)
        {
            return IsAvailable(in routeNode, trafficTypeComponent.TrafficGroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAvailable(in this RouteNodeData routeNode, TrafficGroupType trafficGroupType)
        {
            return DotsEnumExtension.HasFlagUnsafe(routeNode.TrafficGroup, trafficGroupType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetPositionOnRoad(in this PathGraphSystem.Singleton graph, int pathIndex, float targetPathLength)
        {
            GetPositionOnRoad(in graph, pathIndex, targetPathLength, out var spawnPosition, out var spawnDirection, out var pathNodeIndex);
            return spawnPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetPositionOnRoad(in this PathGraphSystem.Singleton graph, int pathIndex, float targetPathLength, out float3 spawnPosition, out float3 spawnDirection, out int pathNodeIndex)
        {
            var pathNodes = graph.GetRouteNodes(pathIndex);

            spawnPosition = default;
            spawnDirection = default;
            pathNodeIndex = -1;

            float currentDistance = 0;
            float prevCurrentDistance = 0;

            var minIndex = 0;
            var maxIndex = pathNodes.Length - 1;

            for (int index = minIndex; index < maxIndex; index++)
            {
                float3 nodePosition = pathNodes[index].Position;
                float3 nextNodePosition = pathNodes[index + 1].Position;

                float distance = math.distance(nodePosition, nextNodePosition);

                currentDistance += distance;

                if (currentDistance >= targetPathLength)
                {
                    var spawnOffset = targetPathLength - prevCurrentDistance;
                    spawnDirection = math.normalize(nextNodePosition - nodePosition);
                    spawnPosition = nodePosition + spawnDirection * spawnOffset;
                    pathNodeIndex = index;
                    return;
                }

                prevCurrentDistance = currentDistance;
            }

            if (pathNodeIndex == -1 && pathNodes.Length >= 2)
            {
                pathNodeIndex = maxIndex;
                spawnPosition = pathNodes[maxIndex].Position;
                spawnDirection = math.normalize(pathNodes[maxIndex].Position - pathNodes[maxIndex - 1].Position);
            }
        }
    }
}
