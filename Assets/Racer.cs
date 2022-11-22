using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Racer : MonoBehaviour
{
    // Start is called before the first frame update

    public abstract void ToggleMovement(bool b);

    public abstract int GetPlacement();
    //something like spawn the win object
    public abstract void WinRace();
    //something like play the animator to show loss or explode them or something
    public abstract void LoseRace();
    public abstract void SetLapPoint(int prog);
    public abstract void AddScore(float score);
    public abstract void Initialize();
    public abstract void SetItem(Item item);
    public abstract void StartCountdown();
    public abstract void StartRacing();
    public abstract void StopRacing();
    public abstract void GiveItem(Item item);
    public abstract void TogglePaused(bool paused);
    public abstract bool CanPickupItem();
    public abstract void OnWallHit();
    public abstract void OnRacerHit();
    public abstract void Respawn();
    //public abstract void UseItem();

    public Game_Manager manager;
    public GameObject WinSign;
    [SerializeField]
    protected float pickupCooldown = 1f;
    protected float pickupCooldownTimer;
    [SerializeField]
    protected RacePosition racePosition;
    [SerializeField]
    protected Lap_Progress lapProgress;
    public int numProgress;
    [SerializeField]
    protected RacerState racerState;
    [SerializeField]
    protected Item currentItem;
    public bool doneRacing;
    public bool isPlayer;
    public int place;
    [SerializeField]
    protected BikeController controller;



    public void GetRespawnDirection()
    {

    }

    public void SetState(RacerState state)
    {
        racerState = state;
    }
    virtual public void StartRace()
    {
        controller.SetMoveState(MoveState.Normal);
        racerState = RacerState.Racing;
    }

    public int GetProgress()
    {
        return numProgress;
    }

    public Item GetItem()
    {
        return currentItem;
    }
    public void SetPlace(int place)
    {
        if (!doneRacing)
        {
            this.place = place;
        }
    }
    public BikeController GetController()
    {
        return controller;
    }

    public RacePosition GetRacePosition()
    {
        return racePosition;
    }

    public void SetRacePosition(RacePosition pos)
    {
        racePosition = pos;
    }

    public void OnBoostPad(Quaternion r, float turnDuration, float power, float boostDuration, bool isLaunchPad = false)
    {
        controller.OnBoostPad(r,turnDuration,power,boostDuration,isLaunchPad);
    }

    public void ApplyStun(float duration)
    {
        controller.ApplyStun(duration);
    }

    public void UseItem()
    {
        if (currentItem == null)
            return;
        switch (currentItem.name)
        {
            case "Mushroom":
                controller.Variable_Boost(1.55f, 1.75f);
                currentItem = null;
                break;
        }
        currentItem = null;
    }
}
