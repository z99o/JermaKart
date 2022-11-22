using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Dreamteck.Splines;
[System.Serializable]
public struct progressPoint
{
    public  Vector3 point;
    public int prog;
}
public class Racer_AI : Racer
{

    // Update is called once per frame
    public BikeAgent agent;
    private Vector3 startPos;
    [SerializeField]
    private float deathTimer;
    public float deathTime = 5f;
    public float raceResetTime = 5f;
    public float timeSinceLastCheckpoint;
    public float checkPointTime = 5f;
    public float score;
    public Vector3 nextGoal;
    public List<progressPoint> progressCounters;
    public SplineComputer spline;
    public bool training;
    private bool passedFirstGate;
    public Quaternion startRotation;

    private void Awake()
    {
        startRotation = transform.rotation;
        if (training)
        {
            manager = Game_Manager._Instance;
            racerState = RacerState.Racing;
            controller.SetMoveState(MoveState.Normal);
        }
    }
        void Update()
    {
        if(racerState == RacerState.Racing)
            deathTimer += Time.deltaTime;
        timeSinceLastCheckpoint += Time.deltaTime;
        score = agent.GetCumulativeReward();
        if(deathTimer > deathTime || timeSinceLastCheckpoint > checkPointTime)
        {
            if (training)
            {
                AddScore(-15f); //punish more for idling
                StartCoroutine(Restart());
            }
            if (timeSinceLastCheckpoint > raceResetTime)
            {
                timeSinceLastCheckpoint = 0; //prevent gbj
                Respawn();
            }
        }
        switch (racerState)
        {
            case RacerState.Uninitialized:
                break;
            case RacerState.Initialized:
                GetPlacement();
                UpdateUI();
                break;
            case RacerState.Countdown:
                GetPlacement();
                UpdateUI();
                //if we have a countdown boost do it here
                break;
            case RacerState.Racing:
                GetPlacement();
                UpdateUI();
                break;
            case RacerState.Finished:
                break;
            default:
                break;
        }
        pickupCooldownTimer += Time.deltaTime;
    }

    public override void SetLapPoint(int prog)
    {
        /*if(prog == Lap_Progress.AI_Gate)
        {
            agent.AddReward(0.1f);
            //agent.EndEpisode();
            return;
        }
        if(prog == Lap_Progress.AI_MajorGate)
        {
            agent.AddReward(5f);
            return;
        }*/
        if (prog > numProgress && prog < (numProgress + (Game_Manager._Instance.GetNumGates() / 4)) && passedFirstGate)
        {
            timeSinceLastCheckpoint = 0;
            float reward = checkPointTime / timeSinceLastCheckpoint;
            reward = Mathf.Clamp(reward, 1, 5);
            agent.AddReward(1f * reward);
            numProgress = prog;
            if(prog == Game_Manager._Instance.GetNumGates() )
                nextGoal = progressCounters.Where(a => a.prog == 0).FirstOrDefault().point;
            else
                nextGoal = progressCounters.Where(a => a.prog == numProgress + 1).FirstOrDefault().point;

        }
        else if(prog == 0 && numProgress == 0 && !passedFirstGate)
        {
            passedFirstGate = true;
            timeSinceLastCheckpoint = 0;
            float reward = checkPointTime / timeSinceLastCheckpoint;
            reward = Mathf.Clamp(reward, 1, 5);
            agent.AddReward(1f * reward);
            numProgress = prog;
            nextGoal = progressCounters.Where(a => a.prog == numProgress + 1).FirstOrDefault().point;

        }
        else if (numProgress > Game_Manager._Instance.GetNumGates() - (Game_Manager._Instance.GetNumGates()/4) && prog == 0 && passedFirstGate)
        {
            timeSinceLastCheckpoint = 0;
            float reward = checkPointTime/timeSinceLastCheckpoint;
            reward = Mathf.Clamp(reward, 1, 5);
            agent.AddReward(1f* reward);
            agent.AddReward(100f);
            numProgress = 0;
            racePosition.lap++;
            nextGoal = progressCounters.Where(a => a.prog == 0).FirstOrDefault().point;
            if(training)
                StartCoroutine(Restart());

        }
        else if(prog <= numProgress && passedFirstGate)
        {
            //agent.AddReward(-2.5f); //later on this will punish the leader too much for getting stuck and wandering
        }
        if (racePosition.lap == manager.lapNum)
        {
            float timeRemaining = deathTime/deathTimer;
            timeSinceLastCheckpoint = 0;
            agent.AddReward(50f * timeRemaining);
            StopRacing();
            if(training)
                StartCoroutine(Restart());
        }
    }

    public override void AddScore(float score)
    {
        agent.AddReward(score);
    }
    public override void GiveItem(Item item)
    {
        pickupCooldownTimer = 0;
        if (item == null)
        {
            //UI_Manager.ItemShuffle();
            SetItem(Game_Manager._Instance.GetRandomItem());
            //SetItem(item);
            //agent.AddReward(0.001f);
        }
        else
        {
            currentItem = item;
        }
    }

    public override bool CanPickupItem()
    {
        if (pickupCooldownTimer > pickupCooldown)
        {
            return true;
        }
        return false;
    }
    private void Start()
    {
        spline = Game_Manager._Instance.GetMainSpline();
        progressCounters  = GameObject.FindGameObjectsWithTag("ProgressGate").Select(a=> new progressPoint
        {
            point = a.transform.position,
            prog = a.GetComponent<ProgressCounter>().progress_num
        }).ToList();
        nextGoal = progressCounters.Where(a => a.prog == 0).FirstOrDefault().point;
    }
    public override void Initialize()
    {
        progressCounters = GameObject.FindGameObjectsWithTag("ProgressGate").Select(a => new progressPoint
        {
            point = a.transform.position,
            prog = a.GetComponent<ProgressCounter>().progress_num
        }).ToList();
        //numProgress = 1;
        nextGoal = progressCounters.Where(a => a.prog == numProgress).FirstOrDefault().point;
        startPos = controller.GetRigidBody().transform.localPosition;
        controller.kartAudioManager.ToggleMotor(true);
        controller.kartAudioManager.Initialize();
        manager = Game_Manager._Instance;
        racerState = RacerState.Initialized;
        controller.SetMoveState(MoveState.NoControl);
        controller.lastGroundedPos = startPos;
        pickupCooldownTimer = 99;
        Debug.Log(this.name + "Initialized"); //change this to debug log when state is changed in its set function
    }

    public override int GetPlacement()
    {
        return 1;
    }

    public void UpdateUI()
    {
        float speed = controller.GetRigidBody().velocity.magnitude;
        int lapNum = racePosition.lap + 1;
        if (!manager)
            manager = Game_Manager._Instance;
        int place = manager.GetPlace(racePosition);
    }

    public override void LoseRace()
    {
        throw new System.NotImplementedException();
    }

    public override void ToggleMovement(bool b)
    {
        throw new System.NotImplementedException();
    }

    public override void WinRace()
    {
        throw new System.NotImplementedException();
    }

    public override void SetItem(Item item)
    {
        currentItem = item;
    }

    public override void StartRacing()
    {
        racerState = RacerState.Racing;
    }

    public override void StopRacing()
    {
        controller.kartAudioManager.ToggleMotor(false);
        controller.kartAudioManager.Toggle_Drift(false);
        controller.kartAudioManager.Toggle_Grind(false);
        controller.SetMoveState(MoveState.NoControl);
        racerState = RacerState.Finished;
    }

    public override void StartCountdown()
    {
        racerState = RacerState.Countdown;
    }

    public override void TogglePaused(bool paused)
    {
        if (paused)
        {
            controller.SetMoveState(MoveState.NoControl);
        }
        else
            controller.RestoreState();
    }

    IEnumerator Restart()
    {
        //agent.AddReward(deathTimer * (0.01f*deathTime));
        deathTimer = 0;
        timeSinceLastCheckpoint = 0;
        agent.EndEpisode();
        //agent.SetReward(0f);
        passedFirstGate = false;
        controller.GetRigidBody().transform.localPosition = startPos;
        controller.GetRigidBody().velocity = Vector3.zero;
        controller.SetSpeed(0);
        controller.GetRigidBody().rotation = startRotation;
        controller.SetMoveState(MoveState.NoControl);
        racerState = RacerState.Countdown;
        controller.SetDriftState(DriftState.None);
        numProgress = 0;
        racePosition.lap = 0;
        controller.transform.rotation = startRotation;
        nextGoal = progressCounters.Where(a => a.prog == 0).FirstOrDefault().point;
        controller.RestoreState();
        yield return null;
        racerState = RacerState.Racing;
        controller.SetMoveState(MoveState.Normal);
    }


    /*new IEnumerator Respawn()
    {
        //agent.AddReward(deathTimer * (0.01f*deathTime));
        agent.AddReward(-5f);
        controller.GetRigidBody().transform.position = spline.Project(controller.lastGroundedPos).position + new Vector3(0, controller.bikeHeight, 0);
        controller.GetRigidBody().velocity = Vector3.zero;
        controller.SetSpeed(0);
        controller.GetRigidBody().rotation = Quaternion.Euler(Vector3.zero);
        controller.SetMoveState(MoveState.NoControl);
        racerState = RacerState.Countdown;
        controller.SetDriftState(DriftState.None);
        controller.transform.LookAt(nextGoal);
        controller.RestoreState();
        yield return new WaitForSeconds(1);
        racerState = RacerState.Racing;
        controller.SetMoveState(MoveState.Normal);
    }*/

    public override void OnWallHit()
    {
        agent.AddReward(-.5f);
        //StartCoroutine(Restart());
    }
    public override void OnRacerHit()
    {
        agent.AddReward(-0.5f);
    }

    public void OnGateHit(float reward)
    {
        agent.AddReward(reward);
    }

    public override void Respawn()
    {
        if (racerState == RacerState.Uninitialized)
            return;
        //agent.AddReward(-10f);
        controller.TeleportRespawn();
    }
}
