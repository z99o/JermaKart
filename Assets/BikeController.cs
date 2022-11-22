using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;
using Cinemachine;


abstract public class BikeController : MonoBehaviour
{
    [Header("States")]
    [SerializeField]
    protected DriftState driftState;
    [SerializeField]
    protected MoveState moveState;
    [SerializeField]
    protected MoveState lastState;
    [SerializeField]
    public Input_Struct input_struct;

    [Header("General Parameters")]
    [SerializeField]
    protected LayerMask drivableLayers;
    [SerializeField]
    protected Transform bikeModel;
    [SerializeField]
    protected Transform bikeDummy;
    [SerializeField]
    protected Transform bikeNormal;
    [SerializeField]
    protected Rigidbody sphere;
    public KartAudioManager kartAudioManager;
    public Racer playerParent;
    [SerializeField]
    protected SparkController sparkController;
    [SerializeField]
    protected HopMaker hopMaker;
    [SerializeField]
    protected Transform visualContainer;
    [SerializeField]
    protected billboarder billboarder;
    [SerializeField]
    protected float speedSlowDownFactor = 3f;
    [SerializeField]
    protected float rotateSlowDownFactor = 2f;
    [SerializeField]
    protected float maxSpeedSlowDown = 0.25f;
    [SerializeField]
    protected float maxRotateSlowDown = 0.1f;
    [SerializeField]
    public float acceleration = 30f;
    [SerializeField]
    public float airFriction = .85f;
    [SerializeField]
    protected float driftTurnFactor = 1.25f;
    [SerializeField]
    protected float driftPowerFactor = .75f;
    [SerializeField]
    public float boostFactor = 0.3f;
    [SerializeField]
    public float steering = 80f;
    [SerializeField]
    public float gravity = 9.8f;
    [SerializeField]
    protected float grindFriction = 0.95f;
    [SerializeField]
    public float bikeHeight = 0.65f;
    [SerializeField]
    protected float jumpForce = 500f;
    [SerializeField]
    protected int numJumps = 2;
    [SerializeField]
    protected float hardFallFactor = 2f;
    [SerializeField]
    protected float softFallFactor = 1f;
    [SerializeField]
    protected float dashLength = 0.75f;
    [SerializeField]
    protected float dashForce = 3000f;


    [Header("Camera Settings")]
    [SerializeField]
    protected float baseFov;
    [SerializeField]
    protected float boostFov;

    [Header("Grinding")]
    [SerializeField]
    protected SplineComputer curRail;
    [SerializeField]
    protected SplineComputer lastRail;
    [SerializeField]
    protected float sameRailCoolDown = 0.65f;
    [SerializeField]
    protected float railMultiplier = 1.65f;
    [SerializeField]
    protected SplineFollower follower;
    [SerializeField]
    private float grindFactor;
    protected float wheelie = 0f;

    [Header("Runtime Variables")]
    [SerializeField]
    protected float fov;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float currentSpeed;
    [SerializeField]
    protected float terrainSpeedFactor;
    [SerializeField]
    protected float lastTerrainSpeedFactor;
    [SerializeField]
    protected float rotate;
    [SerializeField]
    protected float currentRotate;
    [SerializeField]
    protected float driftPower;
    [SerializeField]
    protected float slope;
    [SerializeField]
    protected float timeSinceLastGrind;
    [SerializeField]
    protected int currentJumps = 0;
    protected Tweener currentBoost; 
    [SerializeField]
    protected float timeAirBorne;
    [SerializeReference]
    protected float timeFastFalling;
    [SerializeField]
    public Vector3 lastGroundedPos;
    [SerializeField]
    protected Vector3 heading;
    [SerializeField]
    protected bool canDash;
    [SerializeField]
    protected bool is_spinning;
    [SerializeField]
    protected bool is_flipping;
    [SerializeField]
    protected bool is_rolling;
    [SerializeField]
    public bool isBoosting;
    [SerializeField]
    protected Quaternion dismountAngle;
    [SerializeField]
    protected Quaternion launchEntranceAngle;
    [SerializeField]
    protected Quaternion launchAngle;
    [SerializeField]
    protected bool grindingPositive;
    [SerializeField]
    private bool reversedDirection;
    [SerializeField]
    protected bool applyGravity = true;
    [SerializeField]
    protected bool isGrounded;
    [SerializeField]
    protected bool doJump;
    [SerializeField]
    protected bool jumpedLastFrame;
    [SerializeField]
    protected int jumpFrame;
    [SerializeField]
    protected bool enteredThisFrame;
    [SerializeField]
    public float driftTimer = 0;
    [SerializeField]
    public int driftDirection;
    [SerializeField]
    protected float driftSpeedTimer;
    [SerializeField]
    protected float driftRotateTimer;
    [SerializeField]
    protected bool invincibility;
    [SerializeField]
    protected SplineComputer lastMainRoad;
    [SerializeField]
    public Inputs inputs;

    public Transform GetNormal()
    {
        return bikeNormal; 
    }

    public SplineComputer GetCurrentSpline()
    {
        return lastMainRoad;
    }
    public bool GetGrindDirection()
    {
        return grindingPositive;
    }
    public Rigidbody GetRigidBody()
    {
        return sphere;
    }

    public void SetDriftState(DriftState state)
    {
        driftState = state;
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }
    public float GetRotate()
    {
        return currentRotate;
    }
    public MoveState GetLastState()
    {
        return lastState;
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = 0;
    }

    abstract public int GetMoveDirection();

    public DriftState GetDriftState()
    {
        return driftState;
    }
    public MoveState GetMoveState()
    {
        return moveState;
    }

    public void SetMoveState(MoveState state)
    {
        lastState = moveState;
        moveState = state;
    }

    public void Toggle_Billboarder(bool on)
    {
        billboarder.enabled = on;
    }

    virtual public void OnRailEnter(RailScript railParent, float percentage)
    {
        //take the incoming vector
        if (moveState == MoveState.Grinding)
            return;
        currentJumps = 0;
        enteredThisFrame = true;
        kartAudioManager.Toggle_Drift(false);
        driftState = DriftState.Zero;
        kartAudioManager.Toggle_Grind(true);
        sparkController.SetSparkState(SparkState.Grind);
        SplineComputer newRail = railParent.GetSpline();
        curRail = newRail;
        if (newRail == lastRail && timeSinceLastGrind < sameRailCoolDown)
            return;
        follower.wrapMode = SplineFollower.Wrap.Default;
        if (newRail.isClosed)
            follower.wrapMode = SplineFollower.Wrap.Loop;
        //if we're at the edge then these small percentages can fail here
        Vector3 pointA = Vector3.positiveInfinity;
        Vector3 pointB = Vector3.negativeInfinity;
        pointA = railParent.GetSpline().Evaluate(percentage).position;
        Vector3 ABVector = Vector3.zero;
        try
        {
            pointB = railParent.GetSpline().Evaluate(percentage + 0.1d).position;
            ABVector = pointA - pointB;
        }
        catch
        {
            pointB = railParent.GetSpline().Evaluate(percentage - 0.1d).position; //* -1;
            ABVector = pointB - pointA;
        }
        Vector3 whisker = sphere.velocity.normalized * 2;
        Debug.DrawRay(pointB, ABVector, Color.magenta);
        Debug.DrawRay(transform.position, pointA - transform.position, Color.red);
        Debug.DrawRay(transform.position, pointB - transform.position, Color.blue);
        Debug.DrawRay(transform.position, whisker, Color.green);
        follower.spline = newRail;
        follower.SetPercent(percentage);
        follower.followSpeed = Mathf.Clamp(sphere.velocity.magnitude * railMultiplier,35f,acceleration/2f);
        float incidentAngle = (Vector3.Angle(ABVector, whisker));
        currentSpeed = follower.followSpeed;
        grindingPositive = true;
        follower.direction = Spline.Direction.Forward;
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
        bikeDummy.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
        if (incidentAngle < 90)
        { //if the we're approaching it from behind

            //if (Vector3.Distance(whisker, pointA - transform.position) > Vector3.Distance(whisker, pointB - transform.position))
            //{
            follower.direction = Spline.Direction.Backward;
            grindingPositive = false;
            //}
        }
        /*grindingPositive = true;
        if (Vector3.Distance(whisker,pointA - transform.position) > Vector3.Distance(whisker, pointB - transform.position))
        {
            follower.direction = Spline.Direction.Backward;
            grindingPositive = false;
        }*/

        moveState = MoveState.Grinding;

    }

    virtual public void RailExit(string trigger = "")
    {

        sparkController.SetSparkState(SparkState.Off);
        kartAudioManager.Toggle_Grind(false);
        bool exitedEarly = true;
        if (trigger != "")
        {
            if (curRail.isClosed) //we dont leave a looping rail
                return;
            if (trigger == "End" && !grindingPositive)
                return;
            else if (trigger == "Start" && grindingPositive)
                return;
            exitedEarly = false;
        }
        moveState = MoveState.Dismounting;
        lastRail = follower.spline;
        canDash = true;
        follower.enabled = false;
        follower.spline = null;
        timeSinceLastGrind = 0;
        currentSpeed = follower.followSpeed;
        dismountAngle = sphere.rotation;
        //if we exited early, such as to switch rails
        if (exitedEarly)
        {
            sphere.velocity = sphere.transform.forward * currentSpeed;
            if(input_struct.input_steer != 0)
            {
                int Xdir = input_struct.input_steer > 0 ? 1 : -1;
                AirDash(Xdir,0,1.2f,true,useSphere:true);
                canDash = true;
            }
            Jump(1.45f);
        }
        else
        {
            sphere.velocity = sphere.transform.forward  * currentSpeed * 2;
            Jump();
        }
        currentJumps = 0;
    }

    protected virtual void Control_Normal(Input_Struct inputs)
    {
        //if(Time.frameCount > jumpFrame+10)

        float lerpMultiplier = 1f;
        timeSinceLastGrind += Time.deltaTime;
        float curAcceleration = acceleration;
        if (!isBoosting)
            curAcceleration *= terrainSpeedFactor;
        //Accelerate
        if (isGrounded && inputs.input_gas)
            speed = curAcceleration;
        if (!isGrounded && inputs.input_gas)
        {
            if (currentJumps == 0)
                speed = curAcceleration * airFriction; //maintain speed and apply some air friction while not grounded
            else
                speed = curAcceleration * 0.5f; //slow down further for more precise movement and to prevent jump meta
        }
        else if (!inputs.input_gas)
        {
            lerpMultiplier = 3f;
        }

        //Steer
        if (inputs.input_steer != 0)
        {
            int dir = inputs.input_steer > 0 ? 1 : -1;
            float amount = Mathf.Abs((inputs.input_steer));
            Steer(dir, amount);
        }
        wheelie = 0;
        //Wheelie
        //if (inputs.input_wheelie)
        //{
        //    wheelie = 40f;
        //}

        if (inputs.input_item)
        {
            playerParent.UseItem();
        }

        //Drift
        if (isGrounded && inputs.input_break && driftState == DriftState.None && inputs.input_steer != 0)
        {
            driftState = DriftState.Zero;
            driftDirection = inputs.input_steer > 0 ? 1 : -1;
            //mariokart hop here
            bikeModel.parent.DOComplete();
            kartAudioManager.Sound_Hop();
            visualContainer.DOPunchPosition(transform.up * .2f, .3f, 2, 0).OnComplete(() => {
                visualContainer.localPosition = Vector3.zero; //cancelling a drift will increase the height of the visual box
                kartAudioManager.Toggle_Drift(true);
            });

        }

        if (driftState >= DriftState.Zero)
        {
            if (isGrounded)
            {
                if (!kartAudioManager.driftSource.isPlaying)
                    kartAudioManager.Toggle_Drift(true);
                if (driftState > DriftState.Zero)
                {
                    if (driftDirection > 0)
                        sparkController.SetSparkState(SparkState.Right);
                    else
                        sparkController.SetSparkState(SparkState.Left);
                }
            }
            else
            {
                if(kartAudioManager.driftSource.isPlaying)
                    kartAudioManager.Toggle_Drift(false);
                sparkController.SetSparkState(SparkState.Off);
            }
            float steerDrift = .25f;
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(inputs.input_steer, -1, 1, steerDrift, driftTurnFactor) : ExtensionMethods.Remap(inputs.input_steer, -1, 1, driftTurnFactor, steerDrift);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(inputs.input_steer, -1, 1, .2f, 1) : ExtensionMethods.Remap(inputs.input_steer, -1, 1, 1, .2f);
            Steer(driftDirection, control);
            driftPower += powerControl * driftPowerFactor;

            UpdateDriftState();
        }

        if (!inputs.input_break && driftState >= DriftState.Zero)
        {
            kartAudioManager.Toggle_Drift(false);
            Drift_Boost(boostFactor);
        }

        if (inputs.input_dash)
        {
            int Xdir = 0;
            if(inputs.input_steer != 0)
            {
               Xdir = inputs.input_steer > 0 ? 1 : -1;
            }
            int Ydir = 0;
            if (inputs.input_Yaxis != 0)
            {
                Ydir = inputs.input_Yaxis > 0 ? 1 : -1;
            }
            AirDash(Xdir,Ydir);
        }
        //jump order is important because grounded checks will reset
        if (inputs.input_jump)
        {
            doJump = true;
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f * lerpMultiplier); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;
        kartAudioManager.ModifyPitch(currentSpeed / acceleration);

        //a) Kart
        if (driftState == DriftState.None)
        {
            //lerp to the natural model's foreward
            sparkController.SetSparkState(SparkState.Off);
            kartAudioManager.Toggle_Drift(false);
            bikeModel.localEulerAngles = Vector3.Lerp(bikeModel.localEulerAngles, new Vector3(0, 90 + (inputs.input_steer * 15), bikeModel.localEulerAngles.z), .2f);
            //bikeDummy.localEulerAngles = Vector3.Lerp(bikeModel.localEulerAngles, new Vector3(0, 90 + (inputs.input_steer * 15), bikeModel.localEulerAngles.z), .2f);
        }

        else
        {
            //lerp at the drift direction 
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(inputs.input_steer, -1, 1, .5f, 2) : ExtensionMethods.Remap(inputs.input_steer, -1, 1, 2, .5f);
            bikeModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(bikeModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
            //bikeDummy.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(bikeModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
        }
    }
    virtual protected void Control_Grinding(Input_Struct inputs)
    {
        follower.enabled = true;
        speed = currentSpeed; //we're always pretending to accelerate here
        SwitchGrindDirection();
        Apply_Slope();
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        if (inputs.input_jump)
            RailExit();
    }
    virtual protected void Control_Dismounting(Input_Struct inputs)
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
        if (grounded && inputs.input_gas)
            speed = acceleration;
        else if (!grounded)
        {
            speed = currentSpeed * 0.99f; //maintain speed and apply some air friction while not grounded
        }

        //Steer
        if (inputs.input_steer != 0)
        {
            int dir = (inputs.input_steer > 0 ? 1 : -1);
            float amount = Mathf.Abs((inputs.input_steer));
            Steer(dir, amount);
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;
    }

    virtual protected void Control_Launching(Input_Struct inputs)
    {
        //generally we maintain speed when launching 
        //if(Time.frameCount > jumpFrame+10)

        float lerpMultiplier = 1f;
        timeSinceLastGrind += Time.deltaTime;
        float curAcceleration = acceleration;
        if (!isBoosting)
            curAcceleration *= terrainSpeedFactor;
        //Accelerate
        if (isGrounded && inputs.input_gas)
            speed = curAcceleration;
        if (!isGrounded && inputs.input_gas)
        {
            if (currentJumps == 0)
                speed = curAcceleration * airFriction; //maintain speed and apply some air friction while not grounded
            else
                speed = curAcceleration * 0.5f; //slow down further for more precise movement and to prevent jump meta
        }
        else if (!inputs.input_gas)
        {
            lerpMultiplier = 3f;
        }

        //Steer
        if (inputs.input_steer != 0)
        {
            int dir = inputs.input_steer > 0 ? 1 : -1;
            float amount = Mathf.Abs((inputs.input_steer));
            rotate = (steering * dir) * amount; //do not do fine movement 
        }
        wheelie = 0;
        //Wheelie
        //if (inputs.input_wheelie)
        //{
        //    wheelie = 40f;
        //}

        if (inputs.input_item)
        {
            playerParent.UseItem();
        }

        if (inputs.input_dash)
        {
            int Xdir = 0;
            if (inputs.input_steer != 0)
            {
                Xdir = inputs.input_steer > 0 ? 1 : -1;
            }
            int Ydir = 0;
            if (inputs.input_Yaxis != 0)
            {
                Ydir = inputs.input_Yaxis > 0 ? 1 : -1;
            }
            AirDash(Xdir, Ydir);
        }
        //jump order is important because grounded checks will reset
        if (inputs.input_jump)
        {
            doJump = true;
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f * lerpMultiplier); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;
        kartAudioManager.ModifyPitch(currentSpeed / acceleration);

        /*a) Kart
        if (driftState == DriftState.None)
        {
            //lerp to the natural model's foreward
            sparkController.SetSparkState(SparkState.Off);
            kartAudioManager.Toggle_Drift(false);
            bikeModel.localEulerAngles = Vector3.Lerp(bikeModel.localEulerAngles, new Vector3(0, 90 + (inputs.input_steer * 15), bikeModel.localEulerAngles.z), .2f);
            //bikeDummy.localEulerAngles = Vector3.Lerp(bikeModel.localEulerAngles, new Vector3(0, 90 + (inputs.input_steer * 15), bikeModel.localEulerAngles.z), .2f);
        }

        else
        {
            //lerp at the drift direction 
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(inputs.input_steer, -1, 1, .5f, 2) : ExtensionMethods.Remap(inputs.input_steer, -1, 1, 2, .5f);
            bikeModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(bikeModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
            //bikeDummy.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(bikeModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
        }*/
    }

    virtual protected void Control_NoControl(Input_Struct inputs)
    {

    }
    virtual protected void Move_Normal()
    {

        isGrounded = Is_Grounded();
        if (isGrounded)
        {
            canDash = true;
            applyGravity = true; //cancel anything that pauses gravity
            timeAirBorne = 0;
            //if (currentJumps > 0)
            //    Debug.Log("Resetting Jumps");
            currentJumps = 0;
        }
        if (jumpedLastFrame) //account for the possible lag when checking ground and resetting jumps
        {
            currentJumps++;
            jumpedLastFrame = false;
        }

        float speedSlowDown = 1;
        float rotateSlowDown = 1;
        if (speedSlowDownFactor > 0)
            speedSlowDown = (1 + maxSpeedSlowDown) - (driftSpeedTimer / speedSlowDownFactor);
        if (rotateSlowDownFactor > 0)
            rotateSlowDown = (1 + maxRotateSlowDown) - (driftRotateTimer / rotateSlowDownFactor);
        //Forward Acceleration
        if (driftState == DriftState.None)
        {
            if(isGrounded)
                sphere.AddForce(-bikeModel.transform.right * currentSpeed, ForceMode.Acceleration);
            //if we're in the air we need to make sure we apply speed in the cardinal right direciton, otherwise we can resist gravity
            else
                sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        }
        else
        {
            //get a timer going that when it reaches x number seconds we hit max slowdown
            sphere.AddForce(transform.forward * currentSpeed * speedSlowDown, ForceMode.Acceleration);
        }

        if (doJump)
        {
            Jump();
            doJump = false;
        }

        //Apply Gravity
        Apply_Gravity();

        //Steering
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f * rotateSlowDown);
        bikeDummy.rotation = Quaternion.Lerp(bikeDummy.rotation, Quaternion.Euler(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f * rotateSlowDown);
        if (!is_spinning && !is_flipping && !is_rolling)
        {
            Toggle_Billboarder(true);
            visualContainer.localRotation = Quaternion.Euler(Vector3.zero);
        }
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, drivableLayers);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, drivableLayers);

        //Normal Rotation
        bikeNormal.up = Vector3.Lerp(bikeNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        bikeNormal.Rotate(0, transform.eulerAngles.y, 0);
    }
    virtual protected void Move_Grinding()
    {
        sphere.AddForce(-bikeModel.transform.right * currentSpeed, ForceMode.Acceleration);
        follower.followSpeed = Mathf.Clamp(Mathf.Abs(currentSpeed),0,acceleration/2f);
        //quaternions remove the -360 0 error
        //transform.rotation = Quaternion.Lerp(transform.rotation, sphere.transform.rotation * Quaternion.Euler(0, 90, 0), Time.deltaTime * 10f);
        bikeDummy.rotation = Quaternion.Lerp(bikeDummy.transform.rotation, sphere.transform.rotation, Time.deltaTime * 10f);
        //rotate towards the normal of the spline;
    }

    virtual protected void Move_Dismounting()
    {
        float speedSlowDown = (1 + maxSpeedSlowDown) - (driftSpeedTimer / speedSlowDownFactor);
        float rotateSlowDown = (1 + maxRotateSlowDown) - (driftRotateTimer / rotateSlowDownFactor);
        //Forward Acceleration
        if (driftState == DriftState.None)
            sphere.AddForce(-bikeModel.transform.right * currentSpeed, ForceMode.Acceleration);

        //Apply Gravity
        Apply_Gravity();

        //Steering
        transform.rotation = Quaternion.Lerp(transform.rotation, dismountAngle, Time.deltaTime * 50f);
        bikeDummy.rotation = Quaternion.Lerp(bikeDummy.rotation, dismountAngle, Time.deltaTime * 50f);
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, drivableLayers);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, drivableLayers);

        //Normal Rotation
        bikeNormal.up = Vector3.Lerp(bikeNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        bikeNormal.Rotate(0, transform.eulerAngles.y, 0);


    }

    bool startDive = false;
    protected void Move_Launching()
    {

        isGrounded = Is_Grounded();
        if (jumpedLastFrame) //account for the possible lag when checking ground and resetting jumps
        {
            currentJumps++;
            jumpedLastFrame = false;
        }

        float speedSlowDown = 1;
        float rotateSlowDown = 1;
        if (speedSlowDownFactor > 0)
            speedSlowDown = (1 + maxSpeedSlowDown) - (driftSpeedTimer / speedSlowDownFactor);
        if (rotateSlowDownFactor > 0)
            rotateSlowDown = (1 + maxRotateSlowDown) - (driftRotateTimer / rotateSlowDownFactor);
        //Forward Acceleration
        sphere.AddForce(-bikeModel.transform.right * currentSpeed, ForceMode.Acceleration);

        if (doJump)
        {
            Jump();
            doJump = false;
        }

        //Apply Gravity
        Apply_Gravity();

        //Steering
        //first rotate towards where we need to be going in the launch
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + currentRotate, transform.eulerAngles.z), Time.deltaTime * 5f * rotateSlowDown);
        //bikeDummy.rotation = Quaternion.Lerp(bikeDummy.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + currentRotate, transform.eulerAngles.z), Time.deltaTime * 5f * rotateSlowDown);
        launchAngle = Quaternion.Lerp(launchAngle,Quaternion.Euler(launchAngle.eulerAngles.x, launchAngle.eulerAngles.y + currentRotate, launchAngle.eulerAngles.z), Time.deltaTime * 10f);
        transform.rotation = Quaternion.Lerp(transform.rotation, launchAngle, Time.deltaTime * 10f);
        bikeDummy.rotation = Quaternion.Lerp(bikeDummy.rotation, launchAngle, Time.deltaTime * 10f);
        if (transform.rotation.eulerAngles.x == launchAngle.eulerAngles.x)
            startDive = true;
        if (startDive)
        {  //dive towards the original x axis
            launchAngle = Quaternion.Lerp(launchAngle, Quaternion.Euler(launchEntranceAngle.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * 7f);
            if (isGrounded || transform.rotation.eulerAngles.x == launchEntranceAngle.eulerAngles.x)
            {
                CancelBoost();
                Variable_Boost(1.25f, .5f);
                moveState = MoveState.Normal;
            }
        }
        //then add our steering


        if (!is_spinning && !is_flipping && !is_rolling)
        {
            Toggle_Billboarder(true);
            visualContainer.localRotation = Quaternion.Euler(Vector3.zero);
        }
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, drivableLayers);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, drivableLayers);

        //Normal Rotation
        //bikeNormal.up = Vector3.Lerp(bikeNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        //bikeNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    protected void Move_NoControl()
    {
        Apply_Gravity();
    }

    virtual protected void CancelBoost()
    {
        currentBoost.Kill();
    }

    virtual protected void Drift_Boost(float duration)
    {

        if (driftState > DriftState.Zero)
        {
            kartAudioManager.Sound_Boost();
            isBoosting = true;
            float power = (int)driftState * .75f;
            duration += power / 6f;
            lastTerrainSpeedFactor = terrainSpeedFactor;
            terrainSpeedFactor = 1;
            DOVirtual.Float(currentSpeed * power, currentSpeed, duration, Callback_Speed).OnComplete(() => {
                if(isGrounded) //if we're grounded we reset the  speed but if we're airborn we dont
                    terrainSpeedFactor = lastTerrainSpeedFactor;
                isBoosting = false;
                }
            );
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
    }

    virtual public void Variable_Boost(float power, float duration)
    {
        //sets acceleration high then goes back down
        kartAudioManager.Sound_Boost();
        currentSpeed = acceleration * power;
        isBoosting = true;
        currentBoost = DOVirtual.Float(acceleration * power, acceleration * power, duration/2f, Callback_Speed)
            .OnComplete(() => {
                DOVirtual.Float(acceleration * power, acceleration, duration / 2f, Callback_Speed);
                isBoosting = false;
            }
        );
    }

    public void SpinSprite(float duration, int rotations = 1)
    {
        is_spinning = true;
        Toggle_Billboarder(false);
        visualContainer.DORotate(new Vector3(0, 360 * rotations, 0), duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).OnComplete(() => 
        { is_spinning = false; });
    }

    public void RollSprite(float duration, int rotations = 1)
    {
        is_rolling = true;
        Toggle_Billboarder(false);
        visualContainer.DOLocalRotate(new Vector3(0, 0, 360 * rotations), duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).OnComplete(() =>
        { is_rolling = false; });
    }

    public void FlipSprite(float duration, int rotations = 1)
    {
        is_flipping = true;
        Toggle_Billboarder(false);
        visualContainer.DOLocalRotate(new Vector3(360 * rotations, 0, 0), duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).OnComplete(() =>
        { is_flipping = false; });
    }

    public void ShakeSprite(float duration = 0.25f, float strength = 5)
    {
        visualContainer.DOShakePosition(duration, strength,1,0,fadeOut:true);
    }

    public void BlinkSprite()
    {
        //
    }

    virtual protected void Jump(float multiplier = 1f)
    {
        if (currentJumps >= numJumps)
            return;
        if (moveState == MoveState.NoControl)
            return; //we really shouldn't have this case happen if code is good\
        if (!Is_Grounded()) //if midair jump
        {
            timeAirBorne = 0;
            Vector3 curVelocity = sphere.velocity;
            curVelocity.y = 0;
            sphere.velocity = curVelocity;
            curVelocity = sphere.angularVelocity;
            curVelocity.y = 0;
            sphere.angularVelocity = curVelocity;
            Apply_Gravity(-1);
            SpinSprite(.25f);
        }
        kartAudioManager.Sound_Jump(currentJumps);
        jumpedLastFrame = true;
        sphere.AddForce((jumpForce * multiplier) * Vector3.up, ForceMode.Impulse);
    }

    protected void AirDash(int hor_input, int vert_input, float multiplier = 1, bool cancel_momentum = false, bool useSphere = false)
    {
        if (Is_Grounded() || !canDash)
            return;
        Vector3 dir = transform.forward;
        HopDirection hopDirection = HopDirection.Front;
        if (hor_input != 0)
        {
            if (useSphere)
                dir = hor_input == 1 ? sphere.transform.right : -sphere.transform.right;
            else
                dir = hor_input == 1 ? transform.right : -transform.right;
            hopDirection = hor_input == 1 ? HopDirection.Right : HopDirection.Left;
        }
        if (vert_input < 0)
        {
            CancelMomentum();
            dir = -transform.forward;
            hopDirection = HopDirection.Back;
            FlipSprite(.25f,-1);
        }
        else if (cancel_momentum)
            CancelMomentum();
        hopMaker.PlayHop(hopDirection);
        kartAudioManager.Sound_Dash();
        if (hopDirection == HopDirection.Right)
            RollSprite(.25f,-1);
        else if(hopDirection == HopDirection.Left)
            RollSprite(.25f, 1);
        else if (hopDirection == HopDirection.Front)
            FlipSprite(.25f, 1);
        sphere.AddForce(dir * multiplier * dashForce * 4, ForceMode.Impulse);
        float timer = 0;
        applyGravity = false;
        canDash = false;
        DOVirtual.Float(0, 1, dashLength, (float val) => { timer = val; }).SetEase(Ease.Linear)
            .OnComplete(() => applyGravity = true);
    }

    public void CancelMomentum(bool gravity = false)
    {
        if (gravity)
        {
            Vector3 curVelocity = sphere.velocity;
            curVelocity.y = 0;
            sphere.velocity = curVelocity;
            curVelocity = sphere.angularVelocity;
            curVelocity.y = 0;
            sphere.angularVelocity = curVelocity;
            return;
        }
        currentSpeed = 0;
        speed = 0;
        sphere.velocity = Vector3.zero;
        sphere.angularVelocity = Vector3.zero;
    }

    public void ResetJumps()
    {
        currentJumps = 0;
    }

    public void ResetDash()
    {
        canDash = true;
    }

    protected void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
        if (!Is_Grounded() && currentJumps > 0)
            rotate = (steering* 1.35f * direction) * amount;
    }

    protected void Callback_Speed(float x)
    {
        currentSpeed = x;
    }

    public void OnBoostPad(Quaternion r,float turnDuration,float power = 2f, float boostDuration = 0.75f, bool isLaunchPad = false)
    {
        startDive = false;
        CancelMomentum(true);
        Vector3 eulerR = r.eulerAngles;
        launchEntranceAngle = Quaternion.Euler(30f, eulerR.y, eulerR.z); //this is where we'll dive down into;
        launchAngle = r;
        if (isLaunchPad)
        {
            //applyGravity = false;
            RollSprite(boostDuration/2,5);
            moveState = MoveState.Launching; //go into launch control mode so we force the player where we want
        }
        else { //gently suggest we go that way
            transform.DORotateQuaternion(r, turnDuration).SetEase(Ease.Linear);
            bikeDummy.DORotateQuaternion(r, turnDuration).SetEase(Ease.Linear);
        }
        ResetDash();
        Variable_Boost(power, boostDuration);
        //DOVirtual.DelayedCall(boostDuration, () => { if (moveState == MoveState.Launching) moveState = MoveState.Normal; applyGravity = true; });

    }

    virtual public void ApplyStun(float duration)
    {
        if (invincibility)
            return;
        invincibility = true;
        kartAudioManager.Sound_Hurt();
        moveState = MoveState.NoControl;
        //Jump();
        FlipSprite(duration/3f);
        SpinSprite(duration, 3);
        DOVirtual.DelayedCall(duration, () =>
        {
            moveState = MoveState.Normal;
            DOVirtual.DelayedCall(3f, () =>
            {
                invincibility = false;
            });
        });
    }

    protected void UpdateDriftState()
    {
        if (driftSpeedTimer < speedSlowDownFactor)
            driftSpeedTimer += Time.deltaTime;
        else
        {
            driftSpeedTimer = speedSlowDownFactor;
        }

        if (driftRotateTimer < rotateSlowDownFactor)
            driftRotateTimer += Time.deltaTime;
        else
        {
            driftRotateTimer = rotateSlowDownFactor;
        }

        if (driftPower > 50 && driftPower < 100 - 1 && driftState < DriftState.First)
        {
            driftState = DriftState.First;
            if (driftDirection > 0)
                sparkController.SetSparkState(SparkState.Right);
            else
                sparkController.SetSparkState(SparkState.Left);
        }

        if (driftPower > 125 && driftPower < 150 - 1 && driftState <= DriftState.Second)
        {
            driftState = DriftState.Second;
            sparkController.SetSparkColor(new Color(0,187,255,255));
        }

        if (driftPower > 200 && driftState <= DriftState.Third)
        {
            sparkController.SetSparkColor(new Color(255, 0, 255, 255));
            driftState = DriftState.Third;
        }
    }

    protected IEnumerator UpdateSlope()
    {
        Vector3 lastPos = transform.position;
        yield return new WaitWhile(() => Time.frameCount % 2 != 0); //wait until every 2 frames
        heading = (transform.position - lastPos);
        //Debug.DrawRay(transform.position,heading,Color.green);
        StartCoroutine(UpdateSlope());
    }

    virtual public bool Is_Grounded()
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
            return true;
        }
        timeAirBorne += Time.deltaTime;
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + (-bikeNormal.up * bikeHeight), 0.1f);
    }

    public float GetTerrainFactor(int? layer)
    {
        if (isBoosting)
            return 1;
        if (layer == null)
            return 1;
        if (layer == LayerMask.NameToLayer("Unsafe Terrain"))
        {
            return 1;
        }
        if (layer == LayerMask.NameToLayer("Road") || layer == LayerMask.NameToLayer("MainRoad"))
        {
            lastGroundedPos = transform.position;
            return 1;
        }
        else if (layer == LayerMask.NameToLayer("Rough Terrain"))
            return 0.5f;

        return 1;
    }

    public void Apply_Gravity(float factor = 1)
    {
        //Gravity
        if (!applyGravity)
            return;
        RaycastHit groundHit;
        Vector3 gravityForce = Vector3.zero;
        Physics.Raycast(transform.position, -bikeNormal.up, out groundHit, Mathf.Infinity, drivableLayers);
        if (!Is_Grounded())
        {
            //apply gravity vertically
            float gravityFactor = (1 + Mathf.Clamp((timeAirBorne * timeAirBorne)/2.35f, 0f, hardFallFactor));
            if(gravityFactor > hardFallFactor)
            {
                //phase 3
                timeFastFalling += Time.deltaTime;
                gravityFactor = Mathf.Clamp(hardFallFactor + timeFastFalling, hardFallFactor + 1, hardFallFactor * 2);
            }
            //Debug.Log(groundHit.distance);
            if (groundHit.distance < bikeHeight * 3 && groundHit.transform != null)
            {
                gravityFactor = softFallFactor;
            }

            gravityForce = Vector3.down * gravity * gravityFactor;
            //Debug.Log(gravityFactor);
        }
        else
        {
            //apply gravity to normal
            timeFastFalling = 0;
            gravityForce = -bikeNormal.up * gravity;
            
        }
        Debug.DrawRay(transform.position, (-bikeNormal.up), Color.blue);
        sphere.AddForce(gravityForce * factor, ForceMode.Acceleration);
    }


    public void Apply_Slope()
    {
        Vector3 pos = transform.position;
        pos.y -= bikeHeight;
        float increment = 0.01f;
        if (!grindingPositive)
            increment *= -1;
        try
        {
            Vector3 dir = curRail.EvaluatePosition(follower.GetPercent() + increment) - pos;
            Vector3 flatDir = new Vector3(dir.x, 0, dir.z);
            Vector3 axis = Vector3.back;
            if (follower.direction == Spline.Direction.Backward)
                axis = Vector3.forward;
            Debug.DrawRay(pos, dir, Color.green);
            Debug.DrawRay(pos, flatDir, Color.green);
            slope = Vector3.SignedAngle(dir, flatDir, axis);
            grindFactor = Mathf.Sin(slope * Mathf.Deg2Rad) * 0.5f;
            if (reversedDirection)
                speed += grindFactor * 9.8f;
            else
                speed -= grindFactor * 9.8f;
        }
        catch
        {
            return;
        }

    }

    public void SwitchGrindDirection()
    {
        //you always enter with positive speed because thats just how it works
        Vector3 pos = transform.position;
        pos.y -= bikeHeight;
        float increment = 0.01f;
        if (!grindingPositive)
            increment *= -1;
        Vector3 dir = Vector3.positiveInfinity;
        try
        {
            dir = curRail.EvaluatePosition(follower.GetPercent() + increment) - pos;
        }
        catch
        {
            dir = Vector3.positiveInfinity;
        }
        Vector3 flatDir = new Vector3(dir.x, 0, dir.z);
        Vector3 axis = Vector3.back;
        if (follower.direction == Spline.Direction.Backward)
            axis = Vector3.forward;
        float oldSlope = Vector3.SignedAngle(dir, flatDir, axis);
        if (currentSpeed < 0 && !reversedDirection)
        {
            //check slope before applyings
            if (follower.direction == Spline.Direction.Forward)
            {
                follower.direction = Spline.Direction.Backward;
                grindingPositive = false;
            }
            else
            {
                follower.direction = Spline.Direction.Forward;
                grindingPositive = true;
            }
            try
            {
                if (!grindingPositive)
                    increment *= -1;
                dir = curRail.EvaluatePosition(follower.GetPercent() + increment) - pos;
                flatDir = new Vector3(dir.x, 0, dir.z);
                axis = Vector3.back;
                if (follower.direction == Spline.Direction.Backward)
                    axis = Vector3.forward;
                float newSlope = Vector3.SignedAngle(dir, flatDir, axis);
                if (newSlope > 0 && oldSlope > 0)
                {  //we're in a valley
                    //RailExit();
                    //return;
                }
            }
            catch
            {

            }

            reversedDirection = true;
        }
        if (currentSpeed > 0 && reversedDirection)
        {
            //check slope before applyings
            if (follower.direction == Spline.Direction.Forward)
            {
                follower.direction = Spline.Direction.Backward;
                grindingPositive = false;
            }
            else
            {
                follower.direction = Spline.Direction.Forward;
                grindingPositive = true;
            }
            try
            {
                if (!grindingPositive)
                    increment *= -1;
                dir = curRail.EvaluatePosition(follower.GetPercent() + increment) - pos;
                flatDir = new Vector3(dir.x, 0, dir.z);
                axis = Vector3.back;
                if (follower.direction == Spline.Direction.Backward)
                    axis = Vector3.forward;
                float newSlope = Vector3.SignedAngle(dir, flatDir, axis);
                if (newSlope > 0 && oldSlope > 0)
                {  //we're in a valley
                    //RailExit();
                    //return;
                }
            }
            catch
            {

            }
            reversedDirection = false;
        }
    }

    public void RotateTowards(Quaternion angle, float duration, float percentage = 1)
    {
        //applyGravity = false;
        transform.DORotateQuaternion(angle, duration).SetEase(Ease.Linear);
        bikeDummy.DORotateQuaternion(angle, duration).SetEase(Ease.Linear);
    }

    public void TeleportRespawn()
    {
        if (lastGroundedPos == Vector3.zero)
            return;
        float respawnHeight = 10f;
        //get the respawn point
        SplineSample respawnPoint = lastMainRoad.Project(lastGroundedPos);
        //get a point in front of the respawn point to look at may be reversed on some tracks, we might need to ask the game manager about it
        bool isReversed = false;
        RoadInfo info = lastMainRoad.GetComponent<RoadInfo>();
        if (info != null)
            isReversed = info.pointsReversed;
        float inc = 0.05f;
        if (isReversed)
            inc *= -1;
        float nextPercent = Mathf.Clamp01((float)respawnPoint.percent + inc);

        Vector3 nextPoint = lastMainRoad.Evaluate(nextPercent).position;
        Vector3 respawnPos = respawnPoint.position;

        respawnPos.y += respawnHeight;
        nextPoint.y += respawnHeight; //we dont want to be looking down
        transform.position = respawnPos;
        sphere.transform.position = respawnPos;
        //look in the correct direction of the road, 
        transform.LookAt(nextPoint);
        sphere.transform.LookAt(nextPoint);
        bikeDummy.transform.LookAt(nextPoint);

        timeAirBorne = 0;
        sphere.velocity = Vector3.zero;
        sphere.angularVelocity = Vector3.zero;
        Apply_Gravity(-1);
    }

    public void RestoreState()
    {
        moveState = lastState;
    }

}

