using UnityEngine;
using UnityEngine.UI;

namespace Spirit604.DotsCity.Samples.PlayerInteract
{
    public class PlayerCarBehaviourExample : PlayerCarBehaviourBase
    {
        [SerializeField] private Button exitCarButton;

        private GameObject playerNpcRef;
        private bool isExitButtonPressed = false;

        private void Awake()
        {
            //exitCarButton = GameObject.Find("ExitCarButton")?.GetComponent<Button>();
            exitCarButton=InGameController.instance.m_ExitVehicleBtn;
            enabled = false;

            // Add listener to the button to handle car exit
            if (exitCarButton != null)
            {
                exitCarButton.onClick.AddListener(OnExitCarButtonPressed);
            }
        }

        private void Update()
        {
            if (ClickedExitCarButton())
            {
                ExitCar();
            }
        }

        private void OnExitCarButtonPressed()
        {
            isExitButtonPressed = true;
        }

        public override bool EnterCar(GameObject playerNpc)
        {
            playerNpcRef = playerNpc;

            // Enable custom car input, some code
            EnableInput();

            enabled = true;
            return true;
        }

        public override GameObject ExitCar()
        {
            var currentRef = playerNpcRef;
            playerNpcRef.GetComponent<PlayerInteractorBase>().ExitCar(this.gameObject);

            // Disable custom car input, some code
            DisableInput();

            playerNpcRef = null;
            enabled = false;

            return currentRef;
        }

        public override void Init() { }

        protected virtual bool ClickedExitCarButton()
        {
            if (isExitButtonPressed)
            {
                isExitButtonPressed = false; // Reset the state after processing
                return true;
            }
            return false;
        }

        protected virtual void EnableInput() { }

        protected virtual void DisableInput() { }

        private void OnDestroy()
        {
            // Clean up listener to avoid memory leaks
            if (exitCarButton != null)
            {
                exitCarButton.onClick.RemoveListener(OnExitCarButtonPressed);
            }
        }
    }
}
