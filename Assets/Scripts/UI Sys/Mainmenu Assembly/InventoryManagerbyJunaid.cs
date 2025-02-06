using Invector.vItemManager;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static Invector.vItemManager.vCheckItemsInInventory;

public class InventoryManagerbyJunaid : MonoBehaviour
{
    #region Instance

    private static InventoryManagerbyJunaid _instance;

    public static InventoryManagerbyJunaid instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManagerbyJunaid>();
            }

            return _instance;
        }
    }

    #endregion
    //public vItemCollection vItemCollection;
    public vItemManager vItemManager;
    //public vItemCollection vItemCollection;
   
    private DataController _dataController;

    private void Start()
    {
        _dataController = DataController.instance;

        //AssignAllUnlockedItemsToPlayer();
    }
    public void AssignAllUnlockedItemsToPlayer()
    {
        int totalItems = ApplicationController.SelectedInventoryItem;
        Debug.Log("TotalItems " + totalItems);
            //if (_dataController.GetUnlockInventoryItem(totalItems))
            {
                Debug.Log("Unlocked ItemID_ " + totalItems);
                var reference = new ItemReference(totalItems);
                reference.amount = 1;
                reference.addToEquipArea = true;
                reference.autoEquip = true;
                reference.indexArea = 0;
                vItemManager.CollectItem(reference, textDelay: 2f, ignoreItemAnimation: false);
                print("Item Added" + reference.name);
            }

        /*int totalItems = PlayerPrefs.GetInt("TotalItems");
        for (int i = 0; i < totalItems; i++)
        {
            int itemID = PlayerPrefs.GetInt("ItemID_" + i);
            Debug.Log("ItemID_ " + itemID);
            if (_dataController.GetUnlockInventoryItem(i))
            {
                Debug.Log("Unlocked ItemID_ " + itemID);
                var reference = new ItemReference(itemID);
                reference.amount = 1;
                reference.addToEquipArea = true;
                reference.autoEquip = true;
                reference.indexArea = 0;
                vItemManager.CollectItem(reference, textDelay: 2f, ignoreItemAnimation: false);
            }
        }*/
    }
}
