using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour 
{
	[SerializeField]
    public bool m_SpeedUp ;
    [SerializeField]
    public bool m_SpeedDown;
    [SerializeField]
    public  bool m_Forward;
    [SerializeField]
    public  bool m_Back ;
    [SerializeField]
    public bool m_Left ;
    [SerializeField]
    public  bool m_Right;

    public static ControlPanel instance;

    public void Awake()
    {
	    instance = this;
    }

    public void Speedup(bool t)
	{
		m_SpeedUp = t;
	}

	public void SpeedDown(bool t)
	{
		m_SpeedDown = t;
	}
	public void Forward(bool t)
	{
		m_Forward = t;
	}
	public void Back(bool t)
	{
		m_Back = t;
	}
	public void Left(bool t)
	{
		m_Left = t;
	}
	public void Right(bool t)
	{
		m_Right = t;
	}
}
