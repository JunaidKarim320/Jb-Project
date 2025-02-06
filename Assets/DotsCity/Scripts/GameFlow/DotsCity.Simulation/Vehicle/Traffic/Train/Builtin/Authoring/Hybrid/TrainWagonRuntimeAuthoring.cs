using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Car;
using Spirit604.DotsCity.Simulation.Traffic;
using Spirit604.Extensions;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Train
{
    public class TrainWagonRuntimeAuthoring : MonoBehaviour, IRuntimeEntityComponentSetProvider, IRuntimeInitEntity
    {
        [SerializeField] private Rigidbody rb;

        public Rigidbody Rb => rb;

        ComponentType[] IRuntimeEntityComponentSetProvider.GetComponentSet()
        {
            return new ComponentType[] {
                ComponentType.ReadOnly<TrafficCustomApproachTag>(),
                ComponentType.ReadOnly<TrafficCustomInputTag>(),
                ComponentType.ReadOnly<HasDriverTag>(),
                ComponentType.ReadOnly<TrainTag>(),
                ComponentType.ReadOnly<TrainComponent>(),
                ComponentType.ReadOnly<TrafficWagonComponent>(),
                ComponentType.ReadOnly<TrafficCustomMovementTag>(),
            };
        }

        void IRuntimeInitEntity.Initialize(EntityManager entityManager, GameObject root, Entity entity)
        {
            entityManager.SetComponentEnabled<TrafficCustomMovementTag>(entity, true);
        }

        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
            EditorSaver.SetObjectDirty(this);
        }
    }
}