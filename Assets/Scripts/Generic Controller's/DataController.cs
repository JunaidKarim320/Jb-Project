
using UnityEngine;

public class DataController : MonoBehaviour
{
    
    #region Instance

    public bool OneMillion;
    private static DataController _instance;

    public static DataController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DataController>();
            }

            return _instance;
        }
    }
 
    #endregion
    
    private void Awake()
    {
        _instance = this;
  
    }

    #region Coins

    public int coins
    {
        get
        {
            if (_coins < 0)
            {
                // if (OneMillion)
                // {
                //     _coins = PlayerPrefs.GetInt("coins", 99999);
                // }
                // else
                // {
                    _coins = PlayerPrefs.GetInt("coins", 5000);
                //}
            }
            return _coins;
        }
    }
    
    public int AddCoins(int coin )
    {
        _coins = coins + coin;
        PlayerPrefs.SetInt("coins", _coins);
        coinsChanged?.Invoke();
        return _coins;
    }
    
    public int RemoveCoins(int coin)
    {
        _coins = coins - coin;
        PlayerPrefs.SetInt("coins", _coins);
        coinsChanged?.Invoke();
        return _coins;
    }
    
    #endregion
    
    #region Gold

    public int golds
    {
        get
        {
            if (_gold < 0)
            {
                _gold = PlayerPrefs.GetInt("gold", 0);
            }
            return _coins;
        }
    }
    
    public int AddGold(int gold )
    {
        _gold = golds + gold;
        PlayerPrefs.SetInt("gold", _gold);
        goldChanged?.Invoke();
        return _gold;
    }
    
    public int RemoveGold(int gold)
    {
        _gold = golds - gold;
        PlayerPrefs.SetInt("gold", _gold);
        goldChanged?.Invoke();
        return _gold;
    }
    
    #endregion

    #region Levels / Modes
    
    public void SetUnlockLevel(int levelID)
    {
        PlayerPrefs.SetInt(string.Concat("M"+ lastSelectedGameMode +"levelUnlocked", levelID + 1), 1);
    }
    public bool GetUnlockLevel(int levelID)
    {
        if (levelID == 0)
        {
            PlayerPrefs.SetInt(string.Concat("M"+ lastSelectedGameMode +"levelUnlocked", levelID), 1);
        }   
        return  PlayerPrefs.HasKey(string.Concat("M"+ lastSelectedGameMode +"levelUnlocked", levelID));
    }
    
    public static int lastSelectedLevel
    {
        get
        {
            return PlayerPrefs.GetInt("M" + lastSelectedGameMode + "lastSelectedLevel", 0);
        }
        set
        {
            PlayerPrefs.SetInt("M" + lastSelectedGameMode + "lastSelectedLevel", value);
        }
    }

    public static int lastSelectedGameMode
    {
        get
        {
            return PlayerPrefs.GetInt("lastSelectedGameMode", 0);
        }
        set
        {
            PlayerPrefs.SetInt("lastSelectedGameMode", value);
        }
    }
    public static int AllMissionPoint
    {
        get
        {
            return PlayerPrefs.GetInt("AllMissionPoint", 0);
        }
        set
        {
            PlayerPrefs.SetInt("AllMissionPoint", value);
        }
    }
    
    #endregion

    #region Inventory

    // public static int lastSelectedCustomizationType
    // {
    //     get
    //     {
    //         return PlayerPrefs.GetInt("lastSelectedCustomizationType", 0);
    //     }
    //     set
    //     {
    //         PlayerPrefs.SetInt("lastSelectedCustomizationType", value);
    //     }
    // }  
    //
    // public static int lastSelectedCustomizationItem
    // {
    //     get
    //     {
    //         return PlayerPrefs.GetInt(lastSelectedCustomizationType + "lastSelectedCustomizationItem", 0);
    //     }
    //     set
    //     {
    //         PlayerPrefs.SetInt(lastSelectedCustomizationType + "lastSelectedCustomizationItem", value);
    //     }
    // } 
    
    public static int lastSelectedGripItem
    {
        get
        {
            return PlayerPrefs.GetInt("lastSelectedGripItem", 0);
        }
        set
        {
            PlayerPrefs.SetInt(  "lastSelectedGripItem", value);
        }
    }
    public static int lastSelectedMuzzleItem
    {
        get
        {
            return PlayerPrefs.GetInt("lastSelectedMuzzleItem", 0);
        }
        set
        {
            PlayerPrefs.SetInt(  "lastSelectedMuzzleItem", value);
        }
    }
    public static int lastSelectedLazerItem
    {
        get
        {
            return PlayerPrefs.GetInt("lastSelectedLazerItem", 0);
        }
        set
        {
            PlayerPrefs.SetInt(  "lastSelectedLazerItem", value);
        }
    }
    public static int lastSelectedScopeItem
    {
        get
        {
            return PlayerPrefs.GetInt("lastSelectedScopeItem", 0);
        }
        set
        {
            PlayerPrefs.SetInt(  "lastSelectedScopeItem", value);
        }
    }
    
    
    
    public static int lastSelectedInventoryCategory
    {
        get
        {
            return PlayerPrefs.GetInt("lastSelectedInventoryCategory", 0);
        }
        set
        {
            PlayerPrefs.SetInt("lastSelectedInventoryCategory", value);
        }
    }
    
     public static int lastSelectedInventoryItem
    {
        get
        {
            return PlayerPrefs.GetInt(lastSelectedInventoryCategory + "lastSelectedInventoryItem", 0);
        }
        set
        {
            PlayerPrefs.SetInt(lastSelectedInventoryCategory + "lastSelectedInventoryItem", value);
        }
    }
     
     public static int lastSelectedHeadItem
     {
         get
         {
             return PlayerPrefs.GetInt("lastSelectedHeadItem", 0);
         }
         set
         {
             PlayerPrefs.SetInt(  "lastSelectedHeadItem", value);
         }
     }
     
     public static int lastSelectedBeardItem
     {
         get
         {
             return PlayerPrefs.GetInt("lastSelectedBeardItem", 0);
         }
         set
         {
             PlayerPrefs.SetInt(  "lastSelectedBeardItem", value);
         }
     }
     public static int lastSelectedMostacheItem
     {
         get
         {
             return PlayerPrefs.GetInt("lastSelectedMostacheItem", 0);
         }
         set
         {
             PlayerPrefs.SetInt(  "lastSelectedMostacheItem", value);
         }
     }
     public static int lastSelectedGlassesItem
     {
         get
         {
             return PlayerPrefs.GetInt("lastSelectedGlassesItem", 0);
         }
         set
         {
             PlayerPrefs.SetInt(  "lastSelectedGlassesItem", value);
         }
     }
     public static int lastSelectedBodyItem
     {
         get
         {
             return PlayerPrefs.GetInt("lastSelectedBodyItem", 0);
         }
         set
         {
             PlayerPrefs.SetInt(  "lastSelectedBodyItem", value);
         }
     } 
     public static int lastSelectedTorsoItem
     {
         get
         {
             return PlayerPrefs.GetInt("lastSelectedTorsoItem", 0);
         }
         set
         {
             PlayerPrefs.SetInt(  "lastSelectedTorsoItem", value);
         }
     }
    // Personal Function Created by Junaid.
    public bool GetUnlockInventoryItem(int itemID)
    {
        if (itemID == 0)
        {
            PlayerPrefs.SetInt(string.Concat("InventoryItemUnlocked", itemID), 1);
        }

        return PlayerPrefs.HasKey(string.Concat("InventoryItemUnlocked", itemID));
    }
    public bool GetUnlockInventoryItem(int Category , int itemID)
 {
     if (itemID == 0)
     {
         PlayerPrefs.SetInt(string.Concat("InventoryItemUnlocked", string.Concat(Category, itemID)), 1);
     }

     return  PlayerPrefs.HasKey(string.Concat("InventoryItemUnlocked",  string.Concat(Category, itemID)));
 }
 
 public void SetUnlockInventoryItem(int Category , int itemID)
 {
     PlayerPrefs.GetInt(string.Concat("InventoryItemUnlocked", string.Concat(Category, itemID)), 0);
     PlayerPrefs.SetInt(string.Concat("InventoryItemUnlocked", string.Concat(Category, itemID)), 1);
 }
 public bool GetUnlockInventoryItem(int Category , int SubCategory , int CustomItemID)
 {
     
   //  print($"InventoryItemUnlocked{Category}{SubCategory}{CustomItemID},{PlayerPrefs.HasKey($"InventoryItemUnlocked{Category}{SubCategory}{CustomItemID}")}"  );
     
     if (CustomItemID == 0)
     { 
         PlayerPrefs.SetInt($"InventoryItemUnlocked{Category}{SubCategory}{CustomItemID}", 1);
     }

     return  PlayerPrefs.HasKey($"InventoryItemUnlocked{Category}{SubCategory}{CustomItemID}");
 }
 
 public void SetUnlockInventoryItem(int Category , int SubCategory , int CustomItemID)
 {
//     print($"Unlock : InventoryItemUnlocked{Category}{SubCategory}{CustomItemID}");
     PlayerPrefs.GetInt($"InventoryItemUnlocked{Category}{SubCategory}{CustomItemID}", 0);
     PlayerPrefs.SetInt($"InventoryItemUnlocked{Category}{SubCategory}{CustomItemID}", 1);
 }
 
  public bool GetUnlockInventoryItem(int Category , int itemID, int CustomType , int CustomItemID)
 {
     
//     print($"InventoryItemUnlocked{Category}{itemID}{CustomType}{CustomItemID},{PlayerPrefs.HasKey($"InventoryItemUnlocked{Category}{itemID}{CustomType}{CustomItemID}")}"  );
     
     if (CustomItemID == 0)
     {
       //  PlayerPrefs.SetInt($"InventoryItemUnlocked{Category}{itemID}{CustomType}{CustomItemID}", 1);
     }

     return  PlayerPrefs.HasKey($"InventoryItemUnlocked{Category}{itemID}{CustomType}{CustomItemID}");
 }
 
 public void SetUnlockInventoryItem(int Category , int itemID, int CustomType , int CustomItemID)
 {
     PlayerPrefs.GetInt($"InventoryItemUnlocked{Category}{itemID}{CustomType}{CustomItemID}", 0);
     PlayerPrefs.SetInt($"InventoryItemUnlocked{Category}{itemID}{CustomType}{CustomItemID}", 1);
 }
    #endregion
    
    
    #region Sound
    
    public float sfxVolume
    {
        get
        {
            if (_sfxVolume == -1f)
            {
                _sfxVolume = PlayerPrefs.GetFloat("sfx", 1f);
            }
            return _sfxVolume;
        }
        set
        {
            if (_sfxVolume != value)
            {
                _sfxVolume = value;
                PlayerPrefs.SetFloat("sfx", value);
                if (sfxVolumeChanged != null)
                {
                    sfxVolumeChanged(value);
                }
            }
        }
    }
    
    public float musicVolume
    {
        get
        {
            if (_musicVolume == -1f)
            {
                _musicVolume = PlayerPrefs.GetFloat("music", 1f);
            }
            return _musicVolume;
        }
        set
        {
            if (_musicVolume != value)
            {
                _musicVolume = value;
                PlayerPrefs.SetFloat("music", value);
                if (musicVolumeChanged != null)
                {
                    musicVolumeChanged(value);
                }
            }
        }
    }
    
    #endregion
    
    
    [Header("Coins")]
    private int _coins = -1;
    
    [Header("Gold")]
    private int _gold = -1;
    
    [Header("Sound")]
    private float _sfxVolume = -1f;
    
    [Header("Music")]
    private float _musicVolume = -1f;

    //        Delegates
    public delegate void CoinChangedDelegate();
    
    public delegate void GoldChangedDelegate();

    public delegate void sfxVolumeChangedDelegate(float newVolume);

    public delegate void musicVolumeChangedDelegate(float newVolume);
    
    public static event CoinChangedDelegate coinsChanged;
    
    public static event GoldChangedDelegate goldChanged;

    
    public static event sfxVolumeChangedDelegate sfxVolumeChanged;
	
    public static event musicVolumeChangedDelegate musicVolumeChanged;
}
