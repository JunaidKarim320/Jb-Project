using UnityEngine;
using UnityEngine.UI;

namespace Spirit604.DotsCity.Samples.PlayerInteract
{
    /// <summary>
    /// An example of a player NPC interacting with traffic when the player car & traffic car have different motion controllers.
    /// </summary>
    public class PlayerInteractorSwitchExample : PlayerInteractorExampleBase
    {
        [SerializeField] private Button enterCarButton;

        private bool isEnterButtonPressed = false;

        private void Awake()
        {
            // Add listener to button for entering the car
            if (enterCarButton != null)
            {
                enterCarButton.onClick.AddListener(OnEnterCarButtonPressed);
            }
        }

        private void OnEnterCarButtonPressed()
        {
            isEnterButtonPressed = true;
        }

        protected override bool ProcessUserInput()
        {
            if (isEnterButtonPressed)
            {
                isEnterButtonPressed = false; // Reset after processing
                return true;
            }
            return false;
        }

        protected override GameObject ConvertCarBeforeEnter(GameObject car)
        {
            return PlayerCustomInteractSwitchCarServiceBase.Instance.ConvertCarBeforeEnter(car, gameObject);
        }

        protected override void BeforeExitCarInternal(GameObject car, GameObject npc)
        {
            PlayerCustomInteractSwitchCarServiceBase.Instance.ExitCar(car, transform.gameObject);
        }

        private void OnDestroy()
        {
            // Clean up listener to avoid memory leaks
            if (enterCarButton != null)
            {
                enterCarButton.onClick.RemoveListener(OnEnterCarButtonPressed);
            }
        }
    }
}
