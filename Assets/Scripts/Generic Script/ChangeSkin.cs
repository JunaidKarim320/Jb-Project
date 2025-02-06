using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChangeSkin : MonoBehaviour
{
   
    public MeshRenderer[] m_MeshRenderers;
    public int[] m_MaterialIndex;
    public Material[] m_Materials;
    public Material currentMat;
    
    // Start is called before the first frame update
  /*  IEnumerator Start()
    {
         currentMat = m_Materials[Random.Range(0, m_Materials.Length)];
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < m_MeshRenderers.Length; i++)
        {
            m_MeshRenderers[i].sharedMaterials[m_MaterialIndex[i]] = currentMat;   
        }
        
    }*/

}
 
