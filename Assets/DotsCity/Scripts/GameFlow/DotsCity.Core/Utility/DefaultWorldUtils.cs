using Unity.Entities;

namespace Spirit604.DotsCity.Core
{
    public static class DefaultWorldUtils
    {
        private static World DefaultWorld => World.DefaultGameObjectInjectionWorld;
        private static WorldUnmanaged WorldUnmanaged => World.DefaultGameObjectInjectionWorld.Unmanaged;

        public static void SwitchActiveManagedSystem<T>(bool isEnabled) where T : SystemBase
        {
            var system = DefaultWorld.GetOrCreateSystemManaged<T>();
            system.Enabled = isEnabled;
        }

        public static void SwitchActiveUnmanagedSystem<T>(bool isEnabled) where T : unmanaged, ISystem
        {
            SwitchActiveUnmanagedSystem<T>(WorldUnmanaged, isEnabled);
        }

        public static void SwitchActiveUnmanagedSystem<T>(WorldUnmanaged world, bool isEnabled) where T : unmanaged, ISystem
        {
            ref var system = ref world.GetExistingSystemState<T>();
            system.Enabled = isEnabled;
        }
    }
}
