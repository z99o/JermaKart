using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;
using Cinemachine;
using Unity.MLAgents;


public class AI_BikeController : BikeController
{
    [SerializeField]
    private float jumpCooldown = 1f;
    private float jumpTimer;
    [SerializeField]
    private bool canJump = true;
    [SerializeField]
    private BikeAgent agent;

    void Start()
    {
        StartCoroutine(UpdateSlope());
    }

    // Update is called once per frame
    void Update()
    {
        input_struct.input_gas = agent.AccelerationInput;
        input_struct.input_break = agent.DriftInput;
        input_struct.input_steer = agent.XInput;
        input_struct.input_Yaxis = agent.YInput;
        //input_struct.input_wheelie = inputs.input_wheelie;
        input_struct.input_item = agent.itemInput;
        input_struct.input_jump = agent.JumpInput;
        input_struct.input_dash = agent.dashInput;
        //input_struct.input_attack = agent.attackInput;
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
            case MoveState.NoControl:
                Control_NoControl(input_struct);
                //let the player use the horn
                break;

        }
        if (currentSpeed > acceleration * 3)
            currentSpeed = acceleration * 3;
        jumpTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (currentSpeed > acceleration * 3)
            currentSpeed = acceleration * 3;
        switch (moveState)
        {
            case MoveState.Normal:
                Move_Normal();
                break;
            case MoveState.Grinding:
                Move_Grinding();
                break;
            case MoveState.Dismounting:
                Move_Dismounting();
                break;
            case MoveState.NoControl:
                Move_NoControl();
                break;

        }
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
        bikeDummy.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
    }

    override public int GetMoveDirection()
    {
        if (agent.XInput != 0)
            return agent.XInput > 0 ? 1 : -1;
        return 0;
    }

    override protected void Jump(float multiplier = 1f)
    {
        base.Jump(multiplier);
        jumpedLastFrame = false;
        currentJumps++;
        inputs.input_jump = false;
        jumpCooldown = 0;
    }

    override public void ApplyStun(float duration)
    {
        base.ApplyStun(duration);
        agent.AddReward(-5f);
    }

    /*override protected void Control_Normal()
    {
        bool grounded = Is_Grounded();
        float lerpMultiplier = 1f;
        timeSinceLastGrind += Time.deltaTime;
        //Accelerate
        if (grounded && agent.AccelerationInput)
        {
            agent.AddReward(0.0001f * currentSpeed); //reward going fast
            speed = acceleration;
        }
        else if (!grounded)
        {
            speed = currentSpeed * 0.85f; //maintain speed and apply some air friction while not grounded
        }
        else if (!agent.AccelerationInput)
        {
            lerpMultiplier = 3f;
        }

        //Steer
        if (agent.XInput != 0)
        {
            int dir = agent.XInput > 0 ? 1 : -1;
            float amount = Mathf.Abs(agent.XInput);
            Steer(dir, amount);
        }
        wheelie = 0;
        //Wheelie
        if (Input.GetKey(KeyCode.LeftControl))
        {
            wheelie = 40f;
        }

        if (agent.itemInput)
        {
            playerParent.UseItem();
        }

        //Drift
        if (grounded && agent.DriftInput && driftState == DriftState.None && agent.XInput != 0)
        {
            driftState = DriftState.Zero;
            driftDirection = agent.XInput > 0 ? 1 : -1;
            //mariokart hop here
            bikeModel.parent.DOComplete();
            kartAudioManager.Sound_Hop();

            bikeModel.parent.DOPunchPosition(transform.up * .2f, .3f, 2, 0).OnComplete(() => {

                kartAudioManager.Toggle_Drift(true);
                if (driftDirection > 0)
                    sparkController.SetSparkState(SparkState.Right);
                else
                    sparkController.SetSparkState(SparkState.Left);
            }
            );

        }

        if (driftState >= DriftState.Zero)
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(agent.XInput, -1, 1, 0, driftFactor) : ExtensionMethods.Remap(agent.XInput, -1, 1, driftFactor, 0);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(agent.XInput, -1, 1, .2f, 1) : ExtensionMethods.Remap(agent.XInput, -1, 1, 1, .2f);
            Steer(driftDirection, control);
            driftPower += powerControl;

            UpdateDriftState();
        }

        if (!agent.DriftInput && driftState >= DriftState.Zero)
        {
            kartAudioManager.Toggle_Drift(false);
            Boost();
        }


        if (agent.JumpInput && canJump)
        {
            StartCoroutine(Jump());
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f * lerpMultiplier); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;
        Mathf.Clamp(currentSpeed, -acceleration * 3, acceleration * 3);
        kartAudioManager.ModifyPitch(currentSpeed / acceleration);
        //a) Kart
        if (driftState == DriftState.None)
        {
            //lerp to the natural model's foreward\
            sparkController.SetSparkState(SparkState.Off);
            bikeModel.localEulerAngles = Vector3.Lerp(bikeModel.localEulerAngles, new Vector3(0, 90 + (agent.XInput * 15), bikeModel.localEulerAngles.z), .2f);
            //bikeDummy.localEulerAngles = Vector3.Lerp(bikeModel.localEulerAngles, new Vector3(0, 90 + (Input.GetAxis("Horizontal") * 15), bikeModel.localEulerAngles.z), .2f);
        }

        else
        {
            //lerp at the drift direction 
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(agent.XInput, -1, 1, .5f, 2) : ExtensionMethods.Remap(agent.XInput, -1, 1, 2, .5f);
            bikeModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(bikeModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
            //bikeDummy.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(bikeModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
        }

    }*/



    override public bool Is_Grounded()
    {
            RaycastHit groundHit;
            int? layer = null;
            //Debug.DrawRay(transform.position, -bikeNormal.up * bikeHeight, Color.red);
            if (Physics.SphereCast(transform.position, 0.1f, -bikeNormal.up, out groundHit, bikeHeight, drivableLayers))
            {
                layer = groundHit.transform.gameObject.layer;
                terrainSpeedFactor = GetTerrainFactor(layer);
                if (layer == LayerMask.NameToLayer("MainRoad") && Time.frameCount % 5 == 0) //only check every 5 frames to save computation
                {
                    lastMainRoad = groundHit.transform.parent.GetComponent<SplineComputer>();
                }
                else if (layer == LayerMask.NameToLayer("Rough Terrain"))
                {
                    if(isBoosting) //reward boosting over rough terrain
                        agent.AddReward(.1f);
                    else
                        agent.AddReward(-.005f);
                }

                /*if (layer == LayerMask.NameToLayer("MainRoad"))
                    agent.AddReward(.001f);
                else if (layer == LayerMask.NameToLayer("Unsafe Terrain"))
                    agent.AddReward(.002f);
                else if (layer == LayerMask.NameToLayer("Shortcut"))
                    agent.AddReward(.002f);*/
                return true;
            }
            timeAirBorne += Time.deltaTime;
            return false;
    }

    /*override protected void Control_Grinding()
    {
        exitTimer += Time.deltaTime;
        speed = currentSpeed; //we're always pretending to accelerate here
        SwitchGrindDirection();
        Apply_Slope();
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        if (agent.JumpInput)
            RailExit();
    }*/


    /*override protected void Control_Dismounting()
    {

        bool grounded = Is_Grounded();
        Vector3 eulerTransform = transform.eulerAngles;
        Vector3 eulerDismount = dismountAngle.eulerAngles;
        if (grounded || (
                    (int)eulerTransform.x == (int)eulerDismount.x
                    && (int)eulerTransform.y == (int)eulerDismount.y
                    && (int)eulerTransform.z == (int)eulerDismount.z))
            moveState = MoveState.Normal;

        timeSinceLastGrind += Time.deltaTime;
        //Accelerate
        if (grounded && agent.AccelerationInput)
            speed = acceleration;
        else if (!grounded)
        {
            speed = currentSpeed * 0.99f; //maintain speed and apply some air friction while not grounded
        }

        /*Steer
        if (Input.GetAxis("Horizontal") != 0)
        {
            int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            float amount = Mathf.Abs((Input.GetAxis("Horizontal")));
            Steer(dir, amount);
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

    }


    override protected void Control_NoControl()
    {

    }*/



    override protected void Drift_Boost(float duration)
    {
        base.Drift_Boost(duration);
        /*if (driftState > DriftState.Zero)
        {
            DOVirtual.Float(currentSpeed * 3, currentSpeed, boostFactor * ((int)driftState), Callback_Speed);
            //DOVirtual.Float(0, 1, .5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, .5f, ChromaticAmount));
            //chromatic abberations stuff
            //kart animations
        }

        //reset drift parameters
        driftPower = 0;
        driftState = DriftState.None;
        driftSpeedTimer = 0;
        driftRotateTimer = 0;
        sparkController.SetSparkState(SparkState.Off);
        //clear particles
        //rotate back to parent
        bikeModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);
        */
    }


}

