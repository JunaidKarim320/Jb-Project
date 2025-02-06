using Spirit604.DotsCity.Core;
using Spirit604.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spirit604.DotsCity.Hybrid.Core
{
    [DefaultExecutionOrder(-10000)]
    public class RuntimeHybridEntityService : SingletonMonoBehaviour<RuntimeHybridEntityService>
    {
        [SerializeField] private EntityWorldService entityWorldService;

        public event Action<HybridEntityRuntimeAuthoring> OnDisposeRequested = delegate { };

        private HashSet<HybridEntityRuntimeAuthoring> entities = new HashSet<HybridEntityRuntimeAuthoring>();

        protected override void Awake()
        {
            base.Awake();
            entityWorldService.OnEntitySceneUnload += EntityWorldService_OnEntitySceneUnload;
        }

        private void OnDisable()
        {
            DestroyEntities(false);
        }

        public void RegisterEntity(HybridEntityRuntimeAuthoring hybridEntity)
        {
            entities.Add(hybridEntity);
        }

        public bool UnregisterEntity(HybridEntityRuntimeAuthoring hybridEntity)
        {
            if (entities.Contains(hybridEntity))
            {
                entities.Remove(hybridEntity);
                return true;
            }

            return false;
        }

        public void DestroyEntity(HybridEntityRuntimeAuthoring hybridEntity)
        {
            UnregisterEntity(hybridEntity);

            if (hybridEntity.CustomDispose)
            {
                OnDisposeRequested(hybridEntity);
            }
            else
            {
                hybridEntity.gameObject.ReturnToPool();
            }
        }

        private void DestroyEntities(bool destroy = true)
        {
            foreach (var entity in entities)
            {
                entity.Destroyed = true;

                if (destroy)
                    entity.DestroyEntity();
            }

            entities.Clear();
        }

        private void EntityWorldService_OnEntitySceneUnload()
        {
            DestroyEntities();
        }
    }
}
