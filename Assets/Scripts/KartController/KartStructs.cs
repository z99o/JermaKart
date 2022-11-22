using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RacePosition
{
    public Vector3 pos;
    public int lap;
}
[System.Serializable]
public enum Lap_Progress
{
    AI_Gate,
    AI_MajorGate,
    First,
    Second,
    Third,
    Fourth
}
[System.Serializable]
public enum DriftState
{
    None,
    Zero,
    First,
    Second,
    Third
}
[System.Serializable]
public enum MoveState
{
    Normal,
    Grinding,
    Dismounting,
    Launching,
    NoControl,
}
[System.Serializable]
public enum RacerState
{
    Uninitialized,
    Initialized,
    Countdown,
    Racing,
    Finished
}