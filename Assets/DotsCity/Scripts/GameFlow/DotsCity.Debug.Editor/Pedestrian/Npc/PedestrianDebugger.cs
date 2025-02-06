using Spirit604.Attributes;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Debug
{
    public class PedestrianDebugger : MonoBehaviour
    {
        private enum PedestrianDebuggerType { Disabled, Default, Destination, DestinationIndex, Navigation, Talk, Antistuck }

#pragma warning disable 0414

        [DocLinker("https://dotstrafficcity.readthedocs.io/en/latest/pedestrianDebug.html")]
        [SerializeField] private string link;

        [SerializeField] private bool enableDebug;

        [ShowIf(nameof(enableDebug))]
        [SerializeField] private Color fontColor = Color.white;

        [ShowIf(nameof(enableDebug))]
        [SerializeField] private PedestrianDebuggerType pedestrianDebuggerType;

        [ShowIf(nameof(enableDebug))]
        [SerializeField] private bool drawDefaultGizmos = true;

        [ShowIf(nameof(enableDebug))]
        [SerializeField] private int customSelectedEntityIndex = -1;

        [ShowIf(nameof(DrawDefaultGizmos))]
        [SerializeField] private Color pedestrianColorGizmos = Color.magenta;

#pragma warning restore 0414

        private bool DrawDefaultGizmos => enableDebug && drawDefaultGizmos;

#if UNITY_EDITOR
        private PedestrianDebuggerSystem pedestrianDebuggerSystem;

        private EntityManager entityManager;

        private Dictionary<PedestrianDebuggerType, IEntityDebugger> debuggers = new Dictionary<PedestrianDebuggerType, IEntityDebugger>();

        private void Awake()
        {
            pedestrianDebuggerSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PedestrianDebuggerSystem>();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            debuggers.Add(PedestrianDebuggerType.Default, new DefaultPedestrianDebugger(entityManager));
            debuggers.Add(PedestrianDebuggerType.Destination, new PedestrianDestinationDebugger(entityManager));
            debuggers.Add(PedestrianDebuggerType.DestinationIndex, new PedestrianDestinationIndexDebugger(entityManager));
            debuggers.Add(PedestrianDebuggerType.Navigation, new PedestrianNavigationDebugger(entityManager));
            debuggers.Add(PedestrianDebuggerType.Talk, new PedestrianTalkDebugger(entityManager));
            debuggers.Add(PedestrianDebuggerType.Antistuck, new PedestrianAntistuckDebugger(entityManager));
        }

        public bool ShouldShowDebug(Entity entity)
        {
            if (customSelectedEntityIndex <= 0)
            {
                return true;
            }

            return entity.Index == customSelectedEntityIndex;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enableDebug)
            {
                return;
            }

            pedestrianDebuggerSystem.GetDependency().Complete();
            var pedestrians = pedestrianDebuggerSystem.Pedestrians;

            if (pedestrians.IsCreated)
            {
                for (int i = 0; i < pedestrians.Length; i++)
                {
                    var position = pedestrians[i].Position;

                    if (EntityDebuggerBase.OutOfCamera(position))
                    {
                        continue;
                    }

                    if (ShouldShowDebug(pedestrians[i].Entity))
                    {
                        if (pedestrianDebuggerType != PedestrianDebuggerType.Disabled)
                        {
                            debuggers[pedestrianDebuggerType].Tick(pedestrians[i].Entity, fontColor);
                        }

                        if (drawDefaultGizmos)
                        {
                            var color = pedestrianColorGizmos;

                            if (pedestrianDebuggerType != PedestrianDebuggerType.Disabled && debuggers[pedestrianDebuggerType].HasCustomColor())
                            {
                                color = debuggers[pedestrianDebuggerType].GetBoundsColor(pedestrians[i].Entity);
                            }

                            var oldColor = Gizmos.color;
                            Gizmos.color = color;
                            Gizmos.DrawWireSphere(pedestrians[i].Position, pedestrians[i].Radius);
                            Gizmos.color = oldColor;
                        }
                    }
                }
            }
        }
#endif
    }
}
