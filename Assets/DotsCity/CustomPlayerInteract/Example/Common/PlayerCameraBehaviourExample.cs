using Spirit604.Extensions;
using UnityEngine;
using Invector.vCamera;

#if !CINEMACHINE_V3
using Cinemachine;
#else
using Unity.Cinemachine;
#endif

namespace Spirit604.DotsCity.Samples.PlayerInteract
{
    public class PlayerCameraBehaviourExample : SingletonMonoBehaviour<PlayerCameraBehaviourExample>
    {
        [SerializeField] private GameObject _playerCameraObject;
        [SerializeField] private GameObject _carCameraObject;

        private vThirdPersonCamera _camera;
        private RCC_Camera _carCamera;

        private bool HasCarCamera => _carCamera != null;

        private void Start()
        {
            // Fetch components from the assigned GameObjects
            if (_playerCameraObject != null)
            {
                _camera = _playerCameraObject.GetComponent<vThirdPersonCamera>();
                if (_camera == null)
                {
                    //Debug.LogError("vThirdPersonCamera component not found on _playerCameraObject.");
                }
            }

            if (_carCameraObject != null)
            {
                _carCamera = _carCameraObject.GetComponent<RCC_Camera>();
                if (_carCamera == null)
                {
                    //Debug.LogError("RCC_Camera component not found on _carCameraObject.");
                }
            }
        }

        public void SetTarget(Transform target, bool playerNpcCamera = true)
        {
            if (!HasCarCamera)
            {
                DisableGameObject(_carCameraObject);
                EnableGameObject(_playerCameraObject);

                if (_camera != null)
                {
                    _camera.mainTarget = target;
                }
            }
            else
            {
                if (playerNpcCamera)
                {
                    EnableGameObject(_playerCameraObject);
                    DisableGameObject(_carCameraObject);

                    if (_camera != null)
                    {
                        _camera.mainTarget = target;
                    }
                }
                else
                {
                    EnableGameObject(_carCameraObject);
                    DisableGameObject(_playerCameraObject);

                    if (_carCamera != null)
                    {
                        var carController = target.GetComponent<RCC_CarControllerV3>();
                        if (carController != null)
                        {
                            _carCamera.cameraTarget.playerVehicle = carController;
                        }
                        else
                        {
                            //Debug.LogWarning("Target does not have an RCC_CarControllerV3 component.");
                        }
                    }
                }
            }
        }

        private void EnableGameObject(GameObject obj)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }

        private void DisableGameObject(GameObject obj)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }
}
