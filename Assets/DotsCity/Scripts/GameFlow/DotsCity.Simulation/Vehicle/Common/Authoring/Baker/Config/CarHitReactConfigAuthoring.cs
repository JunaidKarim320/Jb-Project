using Spirit604.Attributes;
using System;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Car.Authoring
{
    public struct HitReactionMaterialData : ISharedComponentData, IEquatable<HitReactionMaterialData>
    {
        public Material Material;

        public bool Equals(HitReactionMaterialData other)
        {
            return Material == other.Material;
        }

        public override int GetHashCode()
        {
            return Material?.GetHashCode() ?? 0;
        }
    }

    public class CarHitReactConfigAuthoring : MonoBehaviour
    {
        [DocLinker("https://dotstrafficcity.readthedocs.io/en/latest/carCommonConfigs.html")]
        [SerializeField] private string link;

        [SerializeField] private Material hitReactionMaterial;

        [Range(1, 100)]
        [SerializeField] private int poolSize = 3;

        [Range(0, 5f)]
        [SerializeField] private float effectDuration = 0.2f;

        [Range(1, 100f)]
        [SerializeField] private float lerpSpeed = 10f;

        [Range(0f, 2f)]
        [SerializeField] private float maxLerp = 1f;

        [Range(0, 0.5f)]
        [SerializeField] private float divHorizontalRate = 0.05F;

        [Range(0, 0.5f)]
        [SerializeField] private float divVerticalRate = 0.0125f;

        public Material HitReactionMaterial { get => hitReactionMaterial; set => hitReactionMaterial = value; }

        class CarHitReactAuthoringBaker : Baker<CarHitReactConfigAuthoring>
        {
            public override void Bake(CarHitReactConfigAuthoring authoring)
            {
                DependsOn(authoring.hitReactionMaterial);

                var entity = CreateAdditionalEntity(TransformUsageFlags.None);

                using (var builder = new BlobBuilder(Unity.Collections.Allocator.Temp))
                {
                    ref var root = ref builder.ConstructRoot<CarHitReactionConfig>();

                    root.PoolSize = authoring.poolSize;
                    root.EffectDuration = authoring.effectDuration;
                    root.LerpSpeed = authoring.lerpSpeed;
                    root.MaxLerp = authoring.maxLerp;
                    root.DivHorizontalRate = authoring.divHorizontalRate;
                    root.DivVerticalRate = authoring.divVerticalRate;

                    var blobRef = builder.CreateBlobAssetReference<CarHitReactionConfig>(Unity.Collections.Allocator.Persistent);

                    AddBlobAsset(ref blobRef, out var hash);

                    AddComponent(entity, new CarHitReactionConfigReference() { Config = blobRef });
                }

                AddSharedComponentManaged(entity, new HitReactionMaterialData()
                {
                    Material = authoring.hitReactionMaterial
                });
            }
        }
    }
}