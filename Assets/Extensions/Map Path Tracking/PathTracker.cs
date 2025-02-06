using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PathTracker : MonoBehaviour
{
 
    public Transform player; // Reference to the TPS character's transform
    public Transform Target; // Reference to the goal's transform
    public int navMeshAreaMask = NavMesh.AllAreas; // NavMesh area mask to use
    public Color lineColor = Color.blue; // Color of the navigation line
    public float lineWidth = 0.1f; // Width of the navigation line

    private LineRenderer lineRenderer;

     bool StopPath;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material.color = lineColor;

        UpdatePath();
    }

    void Update()
    {
        if  (Target ==null || player ==null || StopPath)
            return;
     
          UpdatePath();
      
    }

    void UpdatePath()
    {
        if  (Target ==null || player ==null || StopPath)
            return;
        
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(player.position, Target.position, navMeshAreaMask, path);

        Vector3[] corners = path.corners;

        if (corners.Length < 2)
        {
            lineRenderer.positionCount = 0; // Clear the line if there's no valid path
            return;
        }

        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
    }
     public void SetTarget()
        {
           StopPath=false;
        }
    public void StopTarget()
    {
       StopPath=true;
    }
}
