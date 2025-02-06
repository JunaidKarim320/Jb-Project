using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Core.Bootstrap;
using Spirit604.DotsCity.Simulation.Config;
using Spirit604.DotsCity.Simulation.Level.Props;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Bootstrap
{
    public class EnableSceneSystemsCommand : IBootstrapCommand
    {
        private EntityQuery commonGeneralSettingsQuery;

        private readonly World world;
        private readonly EntityManager entityManager;

        public EnableSceneSystemsCommand(World world, EntityManager entityManager)
        {
            this.world = world;
            this.entityManager = entityManager;

            InitQuery();
        }

        public Task Execute()
        {
            ref var revertCulledPhysicsSystem = ref world.Unmanaged.GetExistingSystemState<RevertCulledPhysicsSystem>();
            revertCulledPhysicsSystem.Enabled = true;

            var generalConfig = commonGeneralSettingsQuery.GetSingleton<CommonGeneralSettingsReference>();

            if (generalConfig.Config.Value.PropsPhysics)
            {
                ref var revertCulledPropsPhysicsSystem = ref world.Unmanaged.GetExistingSystemState<RevertCulledPropsPhysicsSystem>();
                revertCulledPropsPhysicsSystem.Enabled = true;
            }

            return Task.CompletedTask;
        }

        private void InitQuery()
        {
            commonGeneralSettingsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<CommonGeneralSettingsReference>()
                .Build(entityManager);
        }
    }
}