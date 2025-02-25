﻿using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.DotsCity.Simulation.Binding;
using Spirit604.DotsCity.Simulation.Binding.Authoring;
using Spirit604.DotsCity.Simulation.Level.Streaming.Authoring;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Initialization
{
    public class CoreSimulationSystemsInitializer : InitializerBase
    {
        [SerializeField] private RoadStreamingConfigAuthoring roadStreamingConfig;
        [SerializeField] private EntityBindingConfigAuthoring entityBindingConfigAuthoring;

        private EntityBindingService entityBindingService;

        [InjectWrapper]
        public void Construct(
            EntityBindingService entityBindingService)
        {
            this.entityBindingService = entityBindingService;
        }

        public override void Initialize()
        {
            base.Initialize();

            var world = World.DefaultGameObjectInjectionWorld;

            if (entityBindingConfigAuthoring.BindingAvailable)
            {
                world.GetOrCreateSystemManaged<EntityBindingSystem>().Initialize(entityBindingService);

                if (roadStreamingConfig && roadStreamingConfig.StreamingIsEnabled)
                    world.GetOrCreateSystemManaged<EntityBindingCleanupSystem>().Initialize(entityBindingService);
            }
        }
    }
}