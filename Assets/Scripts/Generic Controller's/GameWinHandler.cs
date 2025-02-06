using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinHandler : MonoBehaviour
{

    public int winAmount;
    public int LocationIndex;
    public bool LocationUnlocked;
    public InventoryController.Categories WeaponCategoryIndex;
    public int WeaponIndex;
    public bool WeaponItemUnlocked;
    public CharacterCustomizationController.CharacterCustomizationPart CharacterCustomizationPart;
    public int CharacterCustomizationSubPart;
    public int CharacterCustomizationIndex;
    public bool CharacterCustomizationUnlocked;
    
    
    private GameWinController _gameWinController;
    private GameController _gameController;
    // Start is called before the first frame update
    void Start()
    {
        _gameWinController= GameWinController.instance;
        _gameController= GameController.instance;
    }

    // Update is called once per frame
    public void GameWin()
    {
        _gameWinController. winAmount = winAmount;
        _gameWinController. LocationIndex = LocationIndex;
        _gameWinController. LocationUnlocked = LocationUnlocked;
        _gameWinController. WeaponIndex= WeaponIndex;
        _gameWinController. WeaponCategoryIndex= WeaponCategoryIndex;
        _gameWinController. WeaponItemUnlocked= WeaponItemUnlocked;
        _gameWinController. CharacterCustomizationPart =CharacterCustomizationPart;
        _gameWinController. CharacterCustomizationSubPart= CharacterCustomizationSubPart;
        _gameWinController. CharacterCustomizationIndex =CharacterCustomizationIndex;
        _gameWinController. CharacterCustomizationUnlocked =CharacterCustomizationUnlocked;
        _gameController.displayGameWin();
    }
    public void GameLose()
    {
        _gameController.displayGameOver();
    }
}
