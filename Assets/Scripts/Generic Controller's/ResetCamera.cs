using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ResetCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public void ResetCameraPos()
    {
        transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
    }

    
}
