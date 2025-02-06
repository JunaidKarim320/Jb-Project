using System;
//using JUTPS.CrossPlataform;
//using JUTPS.JUInputSystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HelicopterControllerHelper))]
public class HelicopterController : MonoBehaviour
{
    public AudioSource HelicopterSound;
    public ControlPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 3f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 1.3f;

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MainRotorController.RotarSpeed = value * 80;
            SubRotorController.RotarSpeed = value * 40;
            HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
//            if (UIGameController.runtime.EngineForceView != null)
            //    UIGameController.runtime.EngineForceView.text = string.Format("Engine value [ {0} ] ", (int)value);

            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = true;

   
	void Start () 
    {
        ControlPanel = ControlPanel.instance;

	}
  
    void FixedUpdate()
    {
        KeyPressed();
        LiftProcess();
        MoveProcess();
        TiltProcess();
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

     private void KeyPressed()
    {


        if (!isActiveAndEnabled || !ControlPanel)
        {
           
                ControlPanel = ControlPanel.instance;
            
            return;
        }
        
        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = - Time.fixedDeltaTime;
        else
            if (hMove.y < 0)
                tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else
            if (hMove.x < 0)
                tempX = Time.fixedDeltaTime;



        if (ControlPanel.m_SpeedUp)
        {
            EngineForce += 0.1f;
        }

        if (ControlPanel.m_SpeedDown)
        {
            EngineForce -= 0.12f;
            if (EngineForce < 0) EngineForce = 0;
        }

        if (ControlPanel.m_Forward && !IsOnGround)
        {
            tempY = Time.fixedDeltaTime;
        }

        if (ControlPanel.m_Back && !IsOnGround)
        {
            tempY = -Time.fixedDeltaTime;
        }

        if (ControlPanel.m_Left && !IsOnGround)
        {
            tempX = -Time.fixedDeltaTime;
        }

        if (ControlPanel.m_Right && !IsOnGround)
        {
            tempX = Time.fixedDeltaTime;
        }
        
        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);
        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

    }

  
    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }


    private void OnDisable()
    {
        MainRotorController.RotarSpeed = 0f;
        SubRotorController.RotarSpeed = 0f;
        EngineForce = 0f;
        hMove= new Vector2(0f,0f);
        hTilt= new Vector2(0f,0f);
    }
}