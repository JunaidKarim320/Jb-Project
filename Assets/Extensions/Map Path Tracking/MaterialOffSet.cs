using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOffSet : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 0.5f;
    [SerializeField] Vector2 tilling;
    Renderer rend;
    [HideInInspector]
    public float Distance;

    void Start()
    {
        if (GetComponent<Renderer>())
        {
            rend = GetComponent<Renderer>();
        }
        else if (GetComponent <LineRenderer>())
        {
            rend = GetComponent<LineRenderer>();
        }
       
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.mainTextureOffset = new Vector2(offset, 0);
    }

    
}
