using UnityEngine;

namespace Spirit604.DotsCity.Samples.PlayerInteract
{
    public abstract class PlayerInteractorExampleBase : PlayerInteractorBase
    {
        [Tooltip("Camera follow target if null parent is used as origin")]
        [SerializeField] private Transform cameraOrigin;
        [SerializeField] private LayerMask carRaycastLayer;
        [SerializeField] private float castDistance = 0.5f;
        [SerializeField] private float castFrequncy = 0.1f;

        private GameObject car;
        private float nextCastTime;

        protected override Transform CameraOrigin => cameraOrigin ?? transform;

        private void FixedUpdate()
        {
            CastRay();
        }

        private void Update()
        {
            ProcessCar();
        }

        /// <summary>
        /// Check that the player has clicked the Enter Car button.
        /// </summary>
        protected abstract bool ProcessUserInput();

        /// <summary>
        /// Method for the NPC when NPC enterered the car.
        /// </summary>
        protected override void EnterCarNpcActionFinished(GameObject car, GameObject npc)
        {
            npc.transform.SetParent(car.transform);
            npc.transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Method for the NPC to use when the NPC starts to leave the car.
        /// </summary>
        protected override void StartExitCarNpcAction(GameObject car, GameObject npc)
        {
            npc.transform.SetParent(null);
            npc.SetActive(true);
        }

        /// <summary>
        /// Method for the NPC when the NPC has left the car.
        /// </summary>
        protected override void ExitCarNpcActionFinished(GameObject car, GameObject npc)
        {
            transform.position = car.transform.position - car.transform.right * 1.5f;
        }

        protected override void SetCameraTarget(Transform target, bool playerNpcCamera)
        {
            PlayerCameraBehaviourExample.Instance.SetTarget(target, playerNpcCamera);
        }

        protected virtual Vector3 GetCastOrigin() => transform.position + new Vector3(0, 0.5f);

        protected virtual Vector3 GetCastDirection() => transform.forward;

        /// <summary>
        /// The method for checking the raycasted object is a car & available.
        /// </summary>
        /// <param name="car"></param>
        protected virtual bool CarIsAvailable(GameObject car)
        {
            return car != null;
        }

        protected virtual GameObject GetCarRootFromCollider(GameObject colliderObj)
        {
            return colliderObj;
        }

        private void CastRay()
        {
            if (Time.time < nextCastTime) return;

            nextCastTime = Time.time + castFrequncy;
            Vector3 origin = GetCastOrigin();

            if (Physics.Raycast(origin, GetCastDirection(), out var hit, castDistance, carRaycastLayer, QueryTriggerInteraction.Ignore))
            {
                if (CarIsAvailable(hit.collider.gameObject))
                {
                    car = GetCarRootFromCollider(hit.collider.gameObject);
                }
            }
            else
            {
                car = null;
            }
        }

        private void ProcessCar()
        {
            if (car != null)
            {
                if (ProcessUserInput())
                {
                    EnterCar(car);
                }
            }
        }
    }
}