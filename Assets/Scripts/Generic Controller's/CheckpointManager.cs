using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

[System.Serializable]
public class CheckpointData
{
    [Space(5)] 
    public bool showCheckpoint;
    public GameObject Checkpoint;
    public GameObject CheckpointPlacement;
    [Space (5)]
    public string ObjectiveText;
    [Space (5)]
    public UnityEvent Action; 
}

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager instance;

    public static CheckpointManager Instance
    {
        get
        {
            instance = FindObjectOfType<CheckpointManager>();
            return instance;
        }
    }
    
    public UnityEvent AwakeAction;

    public GameObject CurrentCheckpointObj, PostCheckpointObj;
    
    [Header("CheckPoint Setup")] [Space(4)]
    public CheckpointData[] CheckPoints;


    [Space(8)] [Header("Path Complete")] [Space(4)]
    public float Wait_Time;
    public string CompleteObjectiveText;
    public UnityEvent OnComplete;
    
  //  [HideInInspector] 
    public int CurrentCheckpoints = 0;
    
    public bool Objective,PostObjectiveOff;

    public bool HUD;

    public bool PathTracker;

    public bool MissionCompleted;
    

    [HideInInspector]
    //public PathTracker _pathTracker;
    
    //[HideInInspector]
    public ObjectiveController _ObjectiveController;
     public GameWinHandler gameWinHandler;
     public GameController gameController;
     
    public void Awake()
    {
        instance = this;
        //_pathTracker = FindObjectOfType<PathTracker>();
        _ObjectiveController= FindObjectOfType<ObjectiveController>();
       
        gameWinHandler= GetComponent<GameWinHandler>();
    }

    public void OnDisable()
    {
       // ResetCheckpoint();
    }

    public void ResetCheckpoint()
    {
        CurrentCheckpoints = 0;
        if(CheckPoints[CurrentCheckpoints].showCheckpoint)
        {CheckPoints[CurrentCheckpoints].Checkpoint.SetActive(true);}
        CurrentCheckpointObj.transform.position = CheckPoints[CurrentCheckpoints].Checkpoint.transform.position;
        CurrentCheckpointObj.transform.rotation = CheckPoints[CurrentCheckpoints].Checkpoint.transform.rotation;
        if (CheckPoints.Length >= 2)
        {
            PostCheckpointObj.transform.position = CheckPoints[CurrentCheckpoints + 1].Checkpoint.transform.position;
            PostCheckpointObj.transform.rotation = CheckPoints[CurrentCheckpoints + 1].Checkpoint.transform.rotation;
        }

        if (Objective)
        {
            _ObjectiveController.ShowObjective(CheckPoints[CurrentCheckpoints].ObjectiveText);
        }
    }

    public void Start()
    {
        gameController= GameController.instance;
        
        if(AwakeAction!= null)
            AwakeAction.Invoke();
      
        
        if (PathTracker)
        {
            /*if (_pathTracker)
            {
                if (CheckPoints[CurrentCheckpoints].CheckpointPlacement)
                {
                    _pathTracker.Target = CheckPoints[CurrentCheckpoints].CheckpointPlacement.transform;
                }
                else
                {
                    _pathTracker.Target = CheckPoints[CurrentCheckpoints].Checkpoint.transform;
                }
                _pathTracker.SetTarget();
            }*/
        }

        ResetCheckpoint();
    }
    
    public void CheckpointCollected( )
    {
        // if (Check.tag == "Player")
        // {
            CheckpointShift(CurrentCheckpoints);
        //}
    }
    
    private void CheckpointShift(int checkcurrent)
    {
       // checkcurrent = CurrentCheckpoints;
       CheckPoints[checkcurrent].Action?.Invoke();
       // CheckPoints[checkcurrent].Checkpoint.SetActive(false);
       // checkcurrent++;
        CurrentCheckpoints++;
        checkcurrent = CurrentCheckpoints;
        
        if (checkcurrent < CheckPoints.Length)
        {
            //CheckPoints[checkcurrent].Checkpoint.SetActive(true);
            CurrentCheckpointObj.transform.position = CheckPoints[checkcurrent].Checkpoint.transform.position;
            CurrentCheckpointObj.transform.rotation = CheckPoints[checkcurrent].Checkpoint.transform.rotation;
            if (checkcurrent+1 < CheckPoints.Length)
            {
                PostCheckpointObj.transform.position = CheckPoints[checkcurrent + 1].Checkpoint.transform.position;
                PostCheckpointObj.transform.rotation = CheckPoints[checkcurrent + 1].Checkpoint.transform.rotation;
            }
            else
            {
                PostCheckpointObj.SetActive(false);
            }
            
            if (PathTracker)
            {
                /*if (_pathTracker)
                {
                    if (CheckPoints[CurrentCheckpoints].CheckpointPlacement)
                    {
                        _pathTracker.Target = CheckPoints[CurrentCheckpoints].CheckpointPlacement.transform;
                    }
                    else
                    {
                        _pathTracker.Target = CheckPoints[CurrentCheckpoints].Checkpoint.transform;
                    }
                    _pathTracker.SetTarget();
                }*/
            }
            if (Objective)
            {
                _ObjectiveController.ShowObjective(CheckPoints[CurrentCheckpoints].ObjectiveText);
            }
        }
        else
        {
            CurrentCheckpointObj.SetActive(false);
            Invoke("OnCompleteCall", Wait_Time);
            if (PathTracker)
            {
                /*if (_pathTracker)
                {
                    _pathTracker.StopTarget();
                }*/
            }
            if (Objective)
            {
                if(PostObjectiveOff)
                _ObjectiveController.HidePopup();
            }
            
        }
    }

    
 
    

    public void OnCompleteCall()
    {
        if(OnComplete!= null) 
            OnComplete?.Invoke();
        if (Objective)
        {
            if(!PostObjectiveOff)
            _ObjectiveController.ShowObjective(CompleteObjectiveText);
        }
        if (MissionCompleted)
        {
            gameWinHandler.GameWin();
        }
    }


    public void CarOut()
    {
        gameController.CarOut();
    }
    

    public void CarIn(Transform t)
    {
        gameController.CarIn(t);
    }
}
