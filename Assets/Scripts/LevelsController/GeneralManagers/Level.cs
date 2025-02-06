using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    public Transform PlayerSpawnPoint;
    [HideInInspector]
    public InGameController _ingameController;






    public virtual void StepComplete()
    {
        print("here step complete");

    }
    public virtual void NextStep()
    {
        print("here step complete");

    }
    
    public void ShowHUD()
    {
        _ingameController.ShowAllHUD();
        
    }
    public void HideHUD()
    {
        _ingameController.HideAllHUD();
    }
    
    public void ShowMovementHUD()
    {
        _ingameController.ShowMovementControl();
        
    }
    public void HideMovementHUD()
    {
        _ingameController.HideMovementControl();
    }
    
    public void ShowCarHUD()
    {
        _ingameController.CarHUD(true);
        
    }
    public void HideCarHUD()
    {
        _ingameController.CarHUD(false);
    }
}
    [Serializable]
    public class Steps
    {
        public string name;
        public int index;
        public bool IsTrafficOn;
        public bool Ad;

        [Space] 
        [Header("Steps Events")] 
        [Space]
        public UnityEvent OnStepStartAction;
        public UnityEvent OnStepCompleteAction;
    }
