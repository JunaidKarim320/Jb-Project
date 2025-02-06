using Unity.Entities;

namespace Spirit604.DotsCity.Core.Initialization
{
    [UpdateInGroup(typeof(CullSimulationGroup))]
    public partial class CullInitializerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnUpdate() { }

        public void Launch()
        {
            var cullSystemConfig = SystemAPI.GetSingleton<CullSystemConfigReference>();

            var hasCull = cullSystemConfig.Config.Value.HasCull;

            if (hasCull)
            {
                var method = cullSystemConfig.Config.Value.CullMethod;
                DefaultWorldUtils.SwitchActiveManagedSystem<InitCameraCullingSystem>(true);

                switch (method)
                {
                    case CullMethod.CalculateDistance:
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCullingSystem>(true);
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCullingPreinitSystem>(true);
                        break;
                    case CullMethod.CameraView:
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCameraCullingSystem>(true);
                        DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCameraCullingPreinitSystem>(true);
                        break;
                }

                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCustomCullingSystem>(true);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCustomCullingPreinitSystem>(true);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCameraCustomCullingPreinitSystem>(true);
                DefaultWorldUtils.SwitchActiveUnmanagedSystem<CalcCameraCustomCullingSystem>(true);
            }
        }
    }
}
