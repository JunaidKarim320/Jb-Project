using Unity.Mathematics;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Car
{
    public static class ObstacleLayoutHelper
    {
        public static Bounds GetRotatedBound(float3 position, quaternion rotation, float3 extents, float targetOffset = 0)
        {
            var obs = new ObstacleLayout(position, rotation, extents);

            float3 size = obs.GetCurrentSize();
            size += new float3(targetOffset, 0, targetOffset);

            var bounds = new Bounds()
            {
                center = position,
                size = new float3(size.x, extents.y * 2, size.z)
            };

            return bounds;
        }
    }
}