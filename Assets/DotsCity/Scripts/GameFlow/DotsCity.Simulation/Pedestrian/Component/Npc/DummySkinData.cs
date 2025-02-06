using Unity.Entities;

namespace Spirit604.DotsCity.Simulation.Pedestrian
{
    public struct DummySkinData : IComponentData
    {
        public bool UnloadRenderBounds;
        public Entity DummyEntity;
    }

    public struct LoadDummySkinInViewTag : IComponentData { }

    public struct LoadDummySkinIfOutOfCameraTag : IComponentData { }

    public struct DummySkinEnabledTag : IComponentData, IEnableableComponent { }
}
