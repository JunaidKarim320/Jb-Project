#if UNITY_EDITOR
using Spirit604.DotsCity.Simulation.Road;
using Spirit604.Gameplay.Road;
using Spirit604.Gameplay.Road.Debug;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Spirit604.DotsCity.Debug
{
    public class TrafficNodeLightStateDebugger : ITrafficNodeDebugger
    {
        private EntityManager entityManager;

        public TrafficNodeLightStateDebugger(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public void Tick(Entity entity)
        {
            var trafficNode = entityManager.GetComponentData<TrafficNodeComponent>(entity);
            var pos = entityManager.GetComponentData<LocalToWorld>(entity).Position;

            var lightState = LightState.Uninitialized;

            if (trafficNode.LightEntity != Entity.Null && entityManager.HasComponent<LightHandlerComponent>(trafficNode.LightEntity))
            {
                var trafficLightHandler = entityManager.GetComponentData<LightHandlerComponent>(trafficNode.LightEntity);
                lightState = trafficLightHandler.State;
            }

            Gizmos.color = TrafficLightSceneColor.StateToColor(lightState);
            Gizmos.DrawWireSphere(pos, 1f);
        }
    }
}
#endif