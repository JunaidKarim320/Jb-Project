
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour

{
    #region Instance
 
    private static ApplicationController _instance;

    public static ApplicationController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ApplicationController>();
            }

            return _instance;
        }
    }
 
    #endregion
    
    private void Awake()
    {
        _instance = this;
  
    }

    public static int SelectedLevel;
    public static int currentPersonKilled;
    public static bool NoTrafficVehicle;
    public static bool ExternalVehicle;

    /*public static RewardTriggers _RewardTriggerValue;
    public static RewardBtn _RewardBtnValue;*/

    public static int AllMissionPoint
    {
        get { return DataController.AllMissionPoint; }
        set { DataController.AllMissionPoint = value ; }
    }
    public static int SelectedGameMode
    {
        get { return DataController.lastSelectedGameMode; }
        set { DataController.lastSelectedGameMode = value ; }
    }
    
    public static int LastSelectedLevel
    {
        get { return DataController.lastSelectedLevel; }
        set { DataController.lastSelectedLevel = value; }
    }
    
    public static int SelectedInventoryItem
    {
        get { return DataController.lastSelectedInventoryItem; }
        set { DataController.lastSelectedInventoryItem = value ; }
    }
    
    
    public static int SelectedInventoryCategory
    {
        get { return DataController.lastSelectedInventoryCategory; }
        set { DataController.lastSelectedInventoryCategory = value ; }
    }
    
    // public static int SelectedCustomizationType
    // {
    //     get { return DataController.lastSelectedCustomizationType; }
    //     set { DataController.lastSelectedCustomizationType = value ; }
    // }
    //
    // public static int SelectedCustomizationItem
    // {
    //     get { return DataController.lastSelectedCustomizationItem; }
    //     set { DataController.lastSelectedCustomizationItem = value ; }
    // }
    
    public static int SelectedGripItem
    {
        get { return DataController.lastSelectedGripItem; }
        set { DataController.lastSelectedGripItem = value ; }
    }
    
    public static int SelectedMuzzleItem
    {
        get { return DataController.lastSelectedMuzzleItem; }
        set { DataController.lastSelectedMuzzleItem = value ; }
    }
    public static int SelectedLazerItem
    {
        get { return DataController.lastSelectedLazerItem; }
        set { DataController.lastSelectedLazerItem = value ; }
    }
    public static int SelectedScopeItem
    {
        get { return DataController.lastSelectedScopeItem; }
        set { DataController.lastSelectedScopeItem = value ; }
    }
    
    
    public static int SelectedHeadItem
    {
        get { return DataController.lastSelectedHeadItem; }
        set { DataController.lastSelectedHeadItem = value ; }
    }
    public static int SelectedBeardItem
    {
        get { return DataController.lastSelectedBeardItem; }
        set { DataController.lastSelectedBeardItem = value ; }
    }
    
    public static int SelectedMostacheItem
    {
        get { return DataController.lastSelectedMostacheItem; }
        set { DataController.lastSelectedMostacheItem = value ; }
    }
    public static int SelectedGlassesItem
    {
        get { return DataController.lastSelectedGlassesItem; }
        set { DataController.lastSelectedGlassesItem = value ; }
    }
    public static int SelectedBodyItem
    {
        get { return DataController.lastSelectedBodyItem; }
        set { DataController.lastSelectedBodyItem = value ; }
    }
    public static int SelectedTorsoItem
    {
        get { return DataController.lastSelectedTorsoItem; }
        set { DataController.lastSelectedTorsoItem = value ; }
    }
}
 
 