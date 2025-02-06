using Spirit604.Attributes;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Traffic.Authoring
{
    public class TrafficRailConfigAuthoring : MonoBehaviour
    {
        [DocLinker("https://dotstrafficcity.readthedocs.io/en/latest/trafficCarConfigs.html#traffic-rail-config")]
        [SerializeField] private string link;

        [Header("Rail Settings")]

        [Tooltip("Maximum distance between the rail and the vehicle")]
        [SerializeField][Range(0.005f, 2f)] private float maxDistanceToRailLine = 0.06f;

        [Tooltip("Lateral speed of the vehicle to align with the rail")]
        [SerializeField][Range(0.01f, 4f)] private float lateralSpeed = 0.4f;

        [Tooltip("Rotation lerp speed")]
        [SerializeField][Range(0.1f, 20f)] private float rotationLerpSpeed = 1f;

        [Tooltip("On/off rotating lerp for default traffic")]
        [SerializeField] private bool lerpRotationTraffic = true;

        [Header("Train Settings")]

        [Tooltip("On/off rotating lerp for train")]
        [SerializeField] private bool lerpRotationTram = true;

        [Tooltip("Rotation lerp speed for train")]
        [SerializeField][Range(0.1f, 20f)] private float trainRotationLerpSpeed = 10f;

        [Tooltip("Relative speed of deceleration/acceleration of the wagon when approaching/departing from the connected wagon")]
        [SerializeField][Range(0f, 5f)] private float convergenceSpeedRate = 0.1f;

        class TrafficRailConfigAuthoringBaker : Baker<TrafficRailConfigAuthoring>
        {
            public override void Bake(TrafficRailConfigAuthoring authoring)
            {
                var entity = CreateAdditionalEntity(TransformUsageFlags.None);

                using (var builder = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var root = ref builder.ConstructRoot<TrafficRailConfig>();

                    root.MaxDistanceToRailLine = authoring.maxDistanceToRailLine;
                    root.LateralSpeed = authoring.lateralSpeed;
                    root.RotationLerpSpeed = authoring.rotationLerpSpeed;
                    root.TrainRotationLerpSpeed = authoring.trainRotationLerpSpeed;
                    root.LerpTram = authoring.lerpRotationTram;
                    root.LerpTraffic = authoring.lerpRotationTraffic;
                    root.ConvergenceSpeedRate.x = Mathf.Clamp(1 - authoring.convergenceSpeedRate, 0, float.MaxValue);
                    root.ConvergenceSpeedRate.y = Mathf.Clamp(1 + authoring.convergenceSpeedRate, 0, float.MaxValue);

                    var blobRef = builder.CreateBlobAssetReference<TrafficRailConfig>(Unity.Collections.Allocator.Persistent);

                    AddBlobAsset(ref blobRef, out var hash);

                    AddComponent(entity, new TrafficRailConfigReference() { Config = blobRef });
                }
            }
        }
    }
}