using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Core.Bootstrap;
using Spirit604.DotsCity.Simulation.Level.Streaming;
using Spirit604.DotsCity.Simulation.Road;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Bootstrap
{
    public class InitialGraphResolveCommand : BootstrapCoroutineCommandBase
    {
        private EntityManager entityManager;
        private EntityQuery graphQuery;

        public InitialGraphResolveCommand(EntityManager entityManager, MonoBehaviour source) : base(source)
        {
            this.entityManager = entityManager;
            InitQuery();
        }

        protected override IEnumerator InternalRoutine()
        {
            yield return new WaitWhile(() => graphQuery.CalculateEntityCount() == 0);
            DefaultWorldUtils.SwitchActiveUnmanagedSystem<TrafficNodeResolverSystem>(true);
        }

        private void InitQuery()
        {
            graphQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PathGraphSystem.Singleton>()
                .Build(entityManager);
        }
    }
}