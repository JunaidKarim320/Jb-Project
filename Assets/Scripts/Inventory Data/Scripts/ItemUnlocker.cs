using UnityEngine;
using Invector.vItemManager;

public class ItemUnlocker : MonoBehaviour
{
    [Header("References")]
    public vItemManager itemManager; // Reference to the Invector Item Manager.
    public DataController dataController; // Your custom script handling coins.

    [Header("UI Elements")]
    public GameObject lockButton; // Button to hide after unlocking.
    public GameObject customizationButton; // Customization button, if applicable.

    [Header("Item Settings")]
    public int itemId; // ID of the item from the Invector Item List.
    public int itemPrice; // Price of the item in coins.
    public bool addToEquipArea = true; // Whether to add the item to the equip area.
    public bool autoEquip = true; // Automatically equip the item.
    public int equipAreaIndex = 0; // Index of the equip area.

    /// <summary>
    /// Unlocks the specified item if the player has enough coins.
    /// </summary>
    public void UnlockItem()
    {
        // Check if the DataController has enough coins.
        if (dataController.coins >= itemPrice)
        {
            // Create a reference to the item.
            var reference = new ItemReference(itemId)
            {
                amount = 1, // Add 1 item.
                addToEquipArea = addToEquipArea,
                autoEquip = autoEquip,
                indexArea = equipAreaIndex
            };

            // Add the item to the inventory.
            itemManager.CollectItem(reference, textDelay: 2f, ignoreItemAnimation: false);

            // Deduct the item's price from the player's coins.
            dataController.RemoveCoins(itemPrice);

            // Hide the lock button.
            if (lockButton != null)
            {
                lockButton.SetActive(false);
            }

            // Enable customization button if applicable.
            if (customizationButton != null)
            {
                customizationButton.SetActive(true);
            }

            Debug.Log("Item unlocked and added to inventory!");
        }
        else
        {
            Debug.Log("Not enough coins to unlock this item.");
        }
    }
}
