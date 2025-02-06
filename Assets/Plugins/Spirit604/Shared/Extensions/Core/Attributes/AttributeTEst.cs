using Spirit604.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTEst : MonoBehaviour
{
    [ShowIf(nameof(Val1))]
    public int val1;

    [HideIf(nameof(Val2))]
    public float val2;

    [ShowIf(nameof(Val3))]
    public bool val3;

    [HideIf(nameof(Val4))]
    public Animator val4;

    //public MonoBehaviour val5;

    public bool vall1 = false;
    public bool vall2 = false;
    public bool vall3 = true;
    public bool vall4 = true;

    //private bool vall1 = true;
    //private bool vall2 = false;
    //private bool vall3 = true;
    //private bool vall4 = false;

    public bool Val1 => vall1;
    public bool Val2 => vall2;
    public bool Val3 => vall3;
    public bool Val4 => vall4;
    //public bool Val4 => false;


    //public bool Val1 { get; set; } = false;
    //public bool Val2 { get; set; } = true;
    //public bool Val3 { get; set; } = false;
    //public bool Val4 { get; set; } = true;
}
