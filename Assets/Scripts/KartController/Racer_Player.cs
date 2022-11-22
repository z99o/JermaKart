using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer_Player : Racer
{

    // Update is called once per frame
    public PlayerUI_Manager UI_Manager;
    public Inputs inputs;

    void Update()
    {
        switch (racerState)
        {
            case RacerState.Uninitialized:
                break;
            case RacerState.Initialized:
                UpdateUI();
                break;
            case RacerState.Countdown:
                UpdateUI();
                //if we have a countdown boost do it here
                break;
            case RacerState.Racing:
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
        //must hit a gate every total/4 steps
        int numGates = 100;//Game_Manager._Instance.GetNumGates();
        if (prog > numProgress && prog < (numProgress + (numGates/2))) 
        {
            numProgress = prog;
        }
        else if (numProgress + (numGates / 2) >= numGates && prog == 0)
        {

            numProgress = 0;
            racePosition.lap++;
        }
        if (racePosition.lap == manager.lapNum)
        {
            StopRacing();
        }
    }


    public override void GiveItem(Item item)
    {
        pickupCooldownTimer = 0;
        if (item == null)
        {
            currentItem = null;
            UI_Manager.ItemShuffle();
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

    public override void Initialize()
    {
        manager = Game_Manager._Instance;
        controller.SetMoveState(MoveState.NoControl);
        racerState = RacerState.Initialized;
        //UI_Manager.Initialize(this); //done before screen trans
        controller.kartAudioManager.ToggleMotor(true);
        controller.kartAudioManager.Initialize();
        //numProgress = 1;
        pickupCooldownTimer = 99;
        Debug.Log(this.name + "Initialized"); //change this to debug log when state is changed in its set function
    }

    public override void StartRace()
    {
        controller.SetMoveState(MoveState.Normal);
        racerState = RacerState.Racing;
    }

    public override int GetPlacement()
    {
        return Game_Manager._Instance.GetPlace(racePosition);
    }
    
    public void UpdateUI()
    {
        float speed = controller.GetRigidBody().velocity.magnitude;
        int lapNum = racePosition.lap + 1;
        bool isBoosting = ((PlayerBikeController)controller).isBoosting;
        UI_Manager.UpdateUI(lapNum,place,speed,currentItem,isBoosting);
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
        UI_Manager.SetState(UI_State.Normal);
        racerState = RacerState.Racing;
    }

    public override void StopRacing()
    {
        controller.kartAudioManager.ToggleMotor(false);
        controller.kartAudioManager.Toggle_Drift(false);
        controller.kartAudioManager.Toggle_Grind(false);
        UI_Manager.SetState(UI_State.FinishedRace);
        controller.SetMoveState(MoveState.NoControl);
        racerState = RacerState.Finished;
        Game_Manager._Instance.EndGame();
    }

    public override void StartCountdown()
    {
        UI_Manager.SetState(UI_State.Countdown);
        UI_Manager.Countdown();
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
    public override void OnWallHit()
    {
       // throw new System.NotImplementedException();
    }

    public override void OnRacerHit()
    {
        //throw new System.NotImplementedException();
    }

    public override void Respawn()
    {
        //teleport to respawn
        if (racerState == RacerState.Uninitialized)
            return;
        controller.TeleportRespawn();
        
    }
    public override void AddScore(float score)
    {
        //throw new System.NotImplementedException();
    }
}
