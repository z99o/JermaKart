using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Dreamteck.Splines;
public class BikeAgent : Agent
{
    public int XInput;
    public int YInput;
    public bool AccelerationInput;
    public bool DriftInput;
    public bool JumpInput;
    public bool itemInput;
    public bool dashInput;
    public AI_BikeController controller;
    public Racer_AI racer;
    public SplineComputer spline;
    public Inputs inputs;
    private void Awake()
    {
        spline = GameObject.FindGameObjectWithTag("MainSpline").GetComponent<SplineComputer>();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(controller.GetSpeed());
        sensor.AddObservation((int)controller.GetMoveState());
        sensor.AddObservation((int)controller.GetLastState());
        sensor.AddObservation((int)controller.GetDriftState());
        sensor.AddObservation(controller.driftDirection);
        sensor.AddObservation(controller.GetRotate());
        sensor.AddObservation(controller.driftDirection);
        sensor.AddObservation(racer.nextGoal);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        int item = racer.GetItem() != null ? racer.GetItem().id : -1;
        sensor.AddObservation(item);
        sensor.AddObservation(Vector3.Distance(transform.position, racer.nextGoal));
        spline = controller.GetCurrentSpline();
        if (spline == null)
        {
            spline = Game_Manager._Instance.GetMainSpline();
        }

        sensor.AddObservation(spline.Project(transform.position).position); //may overfit to splines
        sensor.AddObservation(Vector3.Distance(transform.position, spline.Project(transform.position).position));
        sensor.AddObservation((float)spline.Project(transform.position).percent);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        int steerX = 0;
        if (inputs.input_steer != 0)
            steerX = inputs.input_steer > 0 ? 1 : -1;
        int steerY = 0;
        if (inputs.input_Yaxis != 0)
            steerY = inputs.input_Yaxis > 0 ? 1 : -1;
        discreteActions[0] = steerX;
        discreteActions[1] = steerY;
        discreteActions[2] = inputs.input_gas == true ? 1 : 0;
        discreteActions[3] = inputs.input_break == true ? 1 : 0;
        discreteActions[4] = inputs.input_jump == true ? 1 : 0;
        discreteActions[5] = inputs.input_item == true ? 1 : 0;
        discreteActions[6] = inputs.input_dash == true ? 1 : 0;

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //action for horizontal input
       // Debug.Log("Turning " + actions.DiscreteActions[0]);
        XInput = actions.DiscreteActions[0];
        if (XInput == 2)
            XInput = -1;
        YInput = actions.DiscreteActions[1];
        if (YInput == 2)
            YInput = -1;
        //action for accelerating
        // Debug.Log(actions.DiscreteActions[1]);
        AccelerationInput = actions.DiscreteActions[2] == 1 ? true : false;

        //action for drifting
        //Debug.Log(actions.DiscreteActions[2]);
        DriftInput = actions.DiscreteActions[3] == 1 ? true : false;
        //action for jumping
        // Debug.Log(actions.DiscreteActions[3]);
        JumpInput = actions.DiscreteActions[4] == 1 ? true : false;
        //action for item use
        //Debug.Log(actions.DiscreteActions[4]);
        itemInput = actions.DiscreteActions[5] == 1 ? true : false;
        dashInput = actions.DiscreteActions[6] == 1 ? true : false;

    }

    public void GiveReward(float reward)
    {
        AddReward(reward);
    }
}
