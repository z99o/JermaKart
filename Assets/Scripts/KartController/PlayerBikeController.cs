using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;
using Cinemachine;

public class PlayerBikeController : BikeController
{
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera normalCam;
    [SerializeField]
    private CinemachineVirtualCamera grindCam;
    [SerializeField]
    private Transform camLookPos;
    [SerializeField]
    private Transform grindLookPos;
    [SerializeField]
    private float camLookDistance = 20f;

    void Start()
    {
        normalCam.GetCinemachineComponent<CinemachineTransposer>().m_AngularDamping = 0;
        StartCoroutine(UpdateSlope());
    }

    public Transform GetFollow()
    {
        return bikeDummy;
    }

    public Transform GetLookAt()
    {
        return camLookPos.GetChild(0);
    }

    override public int GetMoveDirection()
    {
        if (inputs.input_steer != 0)
            return inputs.input_steer > 0 ? 1 : -1;
        return 0;
    }


    // Update is called once per frame
    void Update()
    {
        //int the future call the base functions and pass in an input struct
        input_struct.input_gas = inputs.input_gas;
        input_struct.input_break = inputs.input_break;
        input_struct.input_steer = inputs.input_steer;
        input_struct.input_wheelie = inputs.input_wheelie;
        input_struct.input_item = inputs.input_item;
        input_struct.input_attack = inputs.input_attack;
        input_struct.input_jump = inputs.input_jump;
        input_struct.input_dash = inputs.input_dash;
        input_struct.input_Yaxis = inputs.input_Yaxis;
        //Control loops go in Update to eliminate stutter
        switch (moveState)
        {
            case MoveState.Normal:
                Control_Normal(input_struct);
                break;
            case MoveState.Grinding:
                Control_Grinding(input_struct);
                break;
            case MoveState.Dismounting:
                Control_Dismounting(input_struct);
                break;
            case MoveState.Launching:
                Control_Launching(input_struct);
                break;
            case MoveState.NoControl:
                Control_NoControl(input_struct);
                //let the player use the horn
                break;
        }
        //inputs.input_gas = false;
        //inputs.input_break = false;
        //inputs.input_wheelie = false;
        inputs.input_item = false;
        inputs.input_attack = false;
        inputs.input_jump = false;
    }

    private void FixedUpdate()
    {
        //int the future call the base functions and pass in an input struct
        //movement update goes in Fixed which elimenates rb stutter
        switch (moveState)
        {
            case MoveState.Normal:
                transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                bikeDummy.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                Move_Normal();
                break;
            case MoveState.Grinding:
                if (!enteredThisFrame)
                {
                    transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                    bikeDummy.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                }
                enteredThisFrame = false;
                Move_Grinding();
                break;
            case MoveState.Dismounting:
                transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                bikeDummy.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                Move_Dismounting();
                break;
            case MoveState.Launching:
                transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                bikeDummy.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                Move_Launching();
                break;
            case MoveState.NoControl:
                transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                bikeDummy.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
                Move_NoControl();
                break;

        }
    }


    override protected void Control_Normal(Input_Struct input_struct)
    {
        base.Control_Normal(input_struct);
        normalCam.m_LookAt = camLookPos.GetChild(0);
        normalCam.m_Lens.FieldOfView = Mathf.Lerp(normalCam.m_Lens.FieldOfView, baseFov, Time.deltaTime * 4);
        //normalCam.GetCinemachineComponent<CinemachineTransposer>().m_AngularDamping = 2f;
    }
    override protected void Control_Grinding(Input_Struct input_struct)
    {
        //camLookPos.localPosition = new Vector3(-20, 0, 0);
        base.Control_Grinding(input_struct);
        //float lookDistance = camLookDistance;
        //normalCam.m_LookAt = grindLookPos;
        //normalCam.GetCinemachineComponent<CinemachineTransposer>().m_AngularDamping = 2f;
        //grindLookPos.localPosition = new Vector3(-lookDistance, 0, 0);

    }

    override protected void Control_Dismounting(Input_Struct input_struct)
    {
        //camLookPos.localPosition = new Vector3(0, 0, 20);
        //normalCam.m_LookAt = camLookPos;
        //camLookPos.localPosition = new Vector3(0, 0, camLookDistance);
        base.Control_Dismounting(input_struct);
    }


    override protected void Control_NoControl(Input_Struct input_struct)
    {

    }

    override protected void Move_Normal()
    {
        camLookPos.localRotation = Quaternion.Euler(Vector3.zero);
        base.Move_Normal();
        
    }

    protected override void Move_Grinding()
    {
        base.Move_Grinding();
        camLookPos.rotation = Quaternion.Lerp(camLookPos.rotation, sphere.transform.rotation, Time.deltaTime * 20f);
    }

    protected override void Move_Dismounting()
    {
        camLookPos.localRotation = Quaternion.Euler(Vector3.zero);
        base.Move_Dismounting();
    }

    public override void OnRailEnter(RailScript railParent, float percentage)
    {
        //normalCam.m_LookAt = grindLookPos;
        //Vector3 pos = new Vector3(-20, 0, 0);
        //camLookPos.transform.localPosition = pos;
        DOVirtual.Float(0f, 1f, 1f, (float s) =>
        {
            normalCam.GetCinemachineComponent<CinemachineTransposer>().m_AngularDamping = s;
        });
        base.OnRailEnter(railParent, percentage);


    }

    override public void RailExit(string trigger = "")
    {
        DOVirtual.Float(1f, 0f, 1f, (float s) =>
        {
            normalCam.GetCinemachineComponent<CinemachineTransposer>().m_AngularDamping = s;
        });
        //camLookPos.localRotation = Quaternion.Euler(Vector3.zero);
        base.RailExit(trigger);
        //do lerp 
        
        
    }

    override protected void Drift_Boost(float duration)
    {
        base.Drift_Boost(duration);
        if (driftState > DriftState.Zero)
        {
            DOVirtual.Float(baseFov + ((int)driftState * 5), baseFov, duration, Callback_Fov)
            .OnComplete(() =>
            {
                baseFov = 70;

            });
        }

    }

    override public void Variable_Boost(float power, float duration)
    {
        base.Variable_Boost(power, duration);
        isBoosting = true;
        DOVirtual.Float(baseFov + (power * 20), baseFov, duration, Callback_Fov)
            .OnComplete(() => {
                isBoosting = false;
                baseFov = 70;
            }); 
    }


    private void Callback_Fov(float x)
    {
        baseFov = x;
    }


}
