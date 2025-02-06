using Spirit604.Extensions;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Spirit604.DotsCity.Core
{
    [DisallowMultipleComponent]
    public abstract class RuntimeEntityAuthoringBase : MonoBehaviour
    {
        [SerializeField]
        private bool addTransform = true;

        private ComponentTypeSet componentSet;
        private IRuntimeInitEntity[] runTimeComponents;
        private bool initComponent;

        protected Entity entity;

        public Entity Entity
        {
            get
            {
                if (!HasEntity)
                {
                    entity = CreateInitialEntity();
                }

                return entity;
            }
        }

        public bool HasEntity => entity != Entity.Null;

        public EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

        protected virtual void Awake()
        {
            InitComponents();
        }

        public bool DestroyEntity()
        {
            if (HasEntity)
            {
                EntityManager.DestroyEntity(entity);
                ResetEntity();
                return true;
            }

            return false;
        }

        public void ResetEntity()
        {
            entity = Entity.Null;
        }

        public void ReinitEntity()
        {
            DestroyEntity();
            InitEntity(true);
        }

        protected virtual List<ComponentType> GetCustomTypeList() => null;

        protected virtual IRuntimeInitEntity[] GetInitializers() => GetComponents<IRuntimeInitEntity>();

        protected virtual Entity CreateInitialEntity()
        {
            entity = Entity.Null;

            if (!addTransform)
            {
                entity = EntityManager.CreateEntity();
            }
            else
            {
                entity = EntityManager.CreateEntity(typeof(LocalToWorld), typeof(LocalTransform));
            }

            return entity;
        }

        protected virtual void PostEntityInit(Entity entity) { }

        protected void InitEntity(bool force = false)
        {
            InitComponents(force);

            if (componentSet.Length > 0)
                EntityManager.AddComponent(Entity, componentSet);

            for (int i = 0; i < runTimeComponents?.Length; i++)
            {
                runTimeComponents[i].Initialize(EntityManager, gameObject, Entity);
            }

            PostEntityInit(Entity);
        }

        private void InitComponents(bool force = false)
        {
            if (initComponent && !force)
                return;

            initComponent = true;

            List<ComponentType> types = null;
            var runTimeSets = GetComponents<IRuntimeEntityComponentSetProvider>();

            for (int i = 0; i < runTimeSets?.Length; i++)
            {
                if (types == null)
                {
                    types = new List<ComponentType>();
                }

                IRuntimeEntityComponentSetProvider set = runTimeSets[i];
                var componentSet = set.GetComponentSet();

                for (int j = 0; j < componentSet?.Length; j++)
                {
                    types.TryToAdd(componentSet[j]);
                }

                var customTypes = GetCustomTypeList();

                if (customTypes != null && customTypes.Count > 0)
                {
                    if (types == null)
                    {
                        types = new List<ComponentType>();
                    }

                    for (int j = 0; j < customTypes.Count; j++)
                    {
                        types.TryToAdd(customTypes[j]);
                    }
                }
            }

            if (types?.Count > 0)
            {
                componentSet = new ComponentTypeSet(types.ToArray());
            }

            runTimeComponents = GetInitializers();
        }
    }
}
