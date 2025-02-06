using Spirit604.Attributes;
using Spirit604.DotsCity.Gameplay.Player;
using Spirit604.Gameplay.Services;
using System;
using UnityEngine;

#if CINEMACHINE
using Spirit604.Collections.Dictionary;
using System.Collections;
#if !CINEMACHINE_V3
using Cinemachine;
#else
using Unity.Cinemachine;
#endif
#endif

namespace Spirit604.DotsCity.Gameplay.CameraService
{
    public class CameraController : CameraVirtualBase
    {
#if CINEMACHINE
#if !CINEMACHINE_V3
        [Serializable]
        public class CameraDictionary : AbstractSerializableDictionary<CameraType, CinemachineVirtualCamera> { }
#else
        [Serializable]
        public class CameraDictionary : AbstractSerializableDictionary<CameraType, CinemachineCamera> { }
#endif
#endif

        [SerializeField] private Canvas canvas;
        [SerializeField] private float noiseAmplitude;
        [SerializeField] private float noiseFrequency;
        [SerializeField] private float noiseLength;

#if CINEMACHINE
        [SerializeField] private CameraDictionary cameras;
        private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
#endif

        private float shakeDisableTime;
        private Coroutine shakeCoroutine;

        private ISceneService sceneService;

        [InjectWrapper]
        public void Construct(ISceneService sceneService, PlayerActorTracker playerActorTracker)
        {
            base.Construct(playerActorTracker);

            this.sceneService = sceneService;

            sceneService.OnSceneUnloaded += SceneService_OnSceneUnloaded;
        }

        public event Action OnCameraWarped = delegate { };

        public void SwitchCanvasState(bool isActive)
        {
            canvas.enabled = isActive;
        }

#if CINEMACHINE

        public void ActivateShake()
        {
            shakeDisableTime = Time.time + noiseLength;

            if (virtualCameraNoise)
            {
#if !CINEMACHINE_V3

                virtualCameraNoise.m_AmplitudeGain = noiseAmplitude;
                virtualCameraNoise.m_FrequencyGain = noiseFrequency;
#else
                virtualCameraNoise.AmplitudeGain = noiseAmplitude;
                virtualCameraNoise.FrequencyGain = noiseFrequency;
#endif
            }

            if (shakeCoroutine == null)
            {
                shakeCoroutine = StartCoroutine(ShakeCoroutine());
            }
        }

#if !CINEMACHINE_V3
        public override void ChangeCamera(CinemachineVirtualCamera newCamera)
        {
            base.ChangeCamera(newCamera);

            virtualCameraNoise = newCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (virtualCameraNoise)
                virtualCameraNoise.m_AmplitudeGain = 0;
        }
#else
        public override void ChangeCamera(CinemachineCamera newCamera)
        {
            base.ChangeCamera(newCamera);

            virtualCameraNoise = newCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (virtualCameraNoise)
                virtualCameraNoise.AmplitudeGain = 0;
        }
#endif

        public IEnumerator WarpCamera()
        {
            yield return new WaitForEndOfFrame();

            if (ActiveCamera)
            {
                ActiveCamera.PreviousStateIsValid = false;
                ActiveCamera.enabled = false;
                ActiveCamera.enabled = true;

                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                UnityEngine.Debug.Log("CameraController. Camera can't be warped, camera is not set up. Make sure the PlayerActor is spawned.");
            }

            OnCameraWarped();
        }

#if !CINEMACHINE_V3
        private CinemachineVirtualCamera GetCamera(PlayerActorType actorType)
#else
        private CinemachineCamera GetCamera(PlayerActorType actorType)
#endif
        {
            switch (actorType)
            {
                case PlayerActorType.Car:
                    {
                        return cameras[CameraType.TopDownPersonCar];
                    }
                case PlayerActorType.Npc:
                    {
                        return cameras[CameraType.TopDownPersonPlayer];
                    }
                case PlayerActorType.FreeFly:
                    {
                        return cameras[CameraType.FreeFly];
                    }
                case PlayerActorType.TrackingCamera:
                    {
                        return cameras[CameraType.TrackingCamera];
                    }
            }

            return null;
        }

        private IEnumerator ShakeCoroutine()
        {
            yield return new WaitWhile(() => Time.time < shakeDisableTime);

            if (virtualCameraNoise)
            {
#if !CINEMACHINE_V3
                virtualCameraNoise.m_AmplitudeGain = 0;
#else
                virtualCameraNoise.AmplitudeGain = 0;
#endif
            }

            shakeCoroutine = null;
        }

        private IEnumerator SwitchCamera(Transform target, PlayerActorType newActorType)
        {
            ActiveCamera.Follow = null;
            ActiveCamera.LookAt = null;

            yield return new WaitForSeconds(0.05f);

            var newCamera = GetCamera(newActorType);

            ChangeCamera(newCamera);

            SetTarget(target);
        }
#endif

        protected override void PlayerActorTracker_OnSwitchActor(Transform newTarget)
        {
            PlayerActorType actorType = PlayerActorType.Npc;

            var newPlayerActor = newTarget.GetComponent<PlayerActor>();

            if (!newPlayerActor)
            {
                UnityEngine.Debug.Log("CameraController. The camera can't be switched. Event 'PlayerActorTracker_OnSwitchActor' PlayerActor is null. Npc camera type is taken");
            }
            else
            {
                actorType = newPlayerActor.CurrentActorType;
            }

#if CINEMACHINE
            bool firstRun = false;

            if (ActiveCamera == null)
            {
                ActiveCamera = GetCamera(actorType);
                firstRun = true;
            }

            if (!firstRun)
            {
                StartCoroutine(SwitchCamera(newTarget, actorType));
            }
            else
            {
                var newCamera = GetCamera(actorType);
                ChangeCamera(newCamera);
                SetTarget(newTarget.transform);
            }

#else
            UnityEngine.Debug.Log("CameraController. The camera can't be switched. Event 'PlayerActorTracker_OnSwitchActor' Cinemachine package not installed. Use your own camera solution or install Cinemachine plugin.");
#endif
        }

        private void SceneService_OnSceneUnloaded()
        {
            SwitchCanvasState(false);
        }
    }
}
