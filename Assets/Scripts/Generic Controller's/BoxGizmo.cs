using UnityEngine;
using System.Collections;

public class BoxGizmo : MonoBehaviour
{
    public Color color;
    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = color;
     //   Gizmos.DrawCube(transform.localEulerAngles, new Vector3(1, 1, 1));
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
      //  Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
       // Gizmos.color = new Color(1, 1, 1, 0.25f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

}
