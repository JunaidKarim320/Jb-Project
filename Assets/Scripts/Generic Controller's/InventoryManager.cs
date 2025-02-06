using System.Collections;
using System.Collections.Generic;
using System.Drawing;
//using JUTPS.InventorySystem;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class InventoryManager : MonoBehaviour
{

    public InventoryController.Categories Categories;
    public int index;
    //private JUInventory _juInventory;
    private GameController _gameController;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        _gameController= GameController.instance;
       // _juInventory = _gameController.Player.GetComponent<JUInventory>();
     //   ApplicationController.SelectedInventoryItem

     yield return new WaitForSeconds(1f);
     if (Categories == InventoryController.Categories.Melee)
     {
         //_juInventory.SelectMeleeItem(index);
     }
     else if (Categories == InventoryController.Categories.HandGun)
     {
         //_juInventory.SelectHandGunItems(index);
     }
     else if (Categories == InventoryController.Categories.SemiAuto)
     {
         //_juInventory.SelectSemiAutoItems(index);
     }
     else if (Categories == InventoryController.Categories.Sniper)
     {
         //_juInventory.SelectSniperItem(index);
     }
     
    }

   
}
