using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsCustomIDs : MonoBehaviour
{
    #region Instance

    private static GunsCustomIDs _instance;

    public static GunsCustomIDs instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GunsCustomIDs>();
            }

            return _instance;
        }
    }

    #endregion

    public int m_GunIDs;
}
