using Spirit604.DotsCity.Simulation.Level.Props;
using Spirit604.DotsCity.Simulation.Level.Streaming.Authoring;
using Spirit604.Gameplay.Road;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Road.Authoring
{
    [DisallowMultipleComponent]
    public class TrafficLightObjectAuthoring : MonoBehaviour, IRelatedObjectProvider
    {
        [SerializeField] private TrafficLightObject trafficLightObject;

        public GameObject RelatedObject
        {
            get
            {
                if (trafficLightObject && trafficLightObject.TrafficLightCrossroad)
                {
                    return trafficLightObject.TrafficLightCrossroad.gameObject;
                }

                return null;
            }
        }

        private void OnEnable()
        {
            if (TrafficLightHybridService.Instance)
                RegisterFrames();
        }

        private void OnDisable()
        {
            if (TrafficLightHybridService.Instance)
                RemoveFrames();
        }

        private void RegisterFrames()
        {
            var frames = trafficLightObject.TrafficLightFrames;

            foreach (var frameData in frames)
            {
                int frameID = frameData.Key + trafficLightObject.ConnectedId;

                var localFrames = frameData.Value.TrafficLightFrames;

                for (int i = 0; i < localFrames.Count; i++)
                {
                    var localFrame = localFrames[i];

                    TrafficLightHybridService.Instance.AddListener(localFrame, frameID);
                }
            }
        }

        private void RemoveFrames()
        {
            var frames = trafficLightObject.TrafficLightFrames;

            foreach (var frameData in frames)
            {
                int frameID = frameData.Key + trafficLightObject.ConnectedId;

                var localFrames = frameData.Value.TrafficLightFrames;

                for (int i = 0; i < localFrames.Count; i++)
                {
                    var localFrame = localFrames[i];

                    TrafficLightHybridService.Instance.RemoveListener(localFrame, frameID);
                }
            }
        }

        class TrafficLightObjectAuthoringBaker : Baker<TrafficLightObjectAuthoring>
        {
            public override void Bake(TrafficLightObjectAuthoring authoring)
            {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

                AddComponent(entity, typeof(LightPropTag));

                var frameBuffer = AddBuffer<LightFrameEntityHolderElement>(entity);

                if (!authoring.trafficLightObject || authoring.trafficLightObject.TrafficLightFrames == null)
                    return;

                var frames = authoring.trafficLightObject.TrafficLightFrames;

                foreach (var frameData in frames)
                {
                    if (frameData.Value == null)
                        continue;

                    var tempFrameBakingEntity = CreateAdditionalEntity(TransformUsageFlags.None, true);

                    Entity relatedHandlerEntity = Entity.Null;

                    if (authoring.trafficLightObject.TrafficLightCrossroad)
                    {
                        var handler = authoring.trafficLightObject.TrafficLightCrossroad.GetTrafficLightHandler(frameData.Key);

                        if (handler != null)
                        {
                            relatedHandlerEntity = GetEntity(handler, TransformUsageFlags.Dynamic);
                        }
                        else
                        {
                            Debug.Log($"Light {authoring.name} InstanceID {authoring.GetInstanceID()} TrafficLightHandler not found. Make sure that TrafficLightHandler is assigned to TrafficLightCrossroad.");
                        }
                    }
                    else
                    {
                        Debug.Log($"Light '{authoring.name}' InstanceID {authoring.GetInstanceID()} doens't have link to 'TrafficLightCrossroad'");
                    }

                    var localFrames = frameData.Value.TrafficLightFrames;

                    int frameCount = localFrames?.Count ?? 0;

                    var frameEntities = new NativeList<LightEntityElementTemp>(frameCount, Allocator.TempJob);

                    if (frameCount == 0)
                    {
                        Debug.Log($"Light '{authoring.name}' InstanceID {authoring.GetInstanceID()} index {frameData.Key} has 0 assigned frames");
                    }

                    for (int i = 0; i < frameCount; i++)
                    {
                        var frameBase = localFrames[i];

                        if (frameBase == null)
                            continue;

                        var frameEntity = GetEntity(frameBase.gameObject, TransformUsageFlags.Dynamic);

                        Entity redEntity = Entity.Null;
                        Entity yellowEntity = Entity.Null;
                        Entity greenEntity = Entity.Null;

                        var frame = frameBase as TrafficLightFrame;

                        if (frame != null)
                        {
                            if (frame.RedLight != null)
                            {
                                redEntity = GetEntity(frame.RedLight, TransformUsageFlags.Dynamic);
                            }

                            if (frame.YellowLight != null)
                            {
                                yellowEntity = GetEntity(frame.YellowLight, TransformUsageFlags.Dynamic);
                            }

                            if (frame.GreenLight != null)
                            {
                                greenEntity = GetEntity(frame.GreenLight, TransformUsageFlags.Dynamic);
                            }
                        }

                        frameEntities.Add(new LightEntityElementTemp()
                        {
                            FrameEntity = frameEntity,
                            IndexPosition = frameBase.GetIndexPosition(),
                            RedEntity = redEntity,
                            YellowEntity = yellowEntity,
                            GreenEntity = greenEntity
                        });

                        frameBuffer.Add(new LightFrameEntityHolderElement()
                        {
                            FrameEntity = frameEntity
                        });
                    }

                    AddComponent(tempFrameBakingEntity, new TrafficLightObjectBakingData()
                    {
                        RelatedEntityHandler = relatedHandlerEntity,
                        FrameEntities = frameEntities.ToArray(Allocator.Temp)
                    });

                    frameEntities.Dispose();
                }
            }
        }
    }
}