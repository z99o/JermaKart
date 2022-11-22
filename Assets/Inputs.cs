using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[System.Serializable]
public struct Input_Struct
{
	public float input_steer;
	public bool input_jump;
	public bool input_gas;
	public bool input_break;
	public bool input_attack;
	public bool input_wheelie;
	public bool input_pause;
	public bool input_item;
	public bool input_dash;
	public float input_Yaxis;
}

public class Inputs : MonoBehaviour
{
	[Header("Character Input Values")]
	public float input_steer;
	public float input_Yaxis;
	public bool input_jump;
	public bool input_gas;
	public bool input_break;
	public bool input_attack;
	public bool input_wheelie;
	public bool input_pause;
	public bool input_item;
	public bool input_dash;

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	[Header("UI Settings")]
	public Vector2 point;
	public bool click;


//#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

	public void OnGas(InputValue value)
    {
		GasInput(value.isPressed);
    }

	public void OnSteer(InputValue value)
	{
		SteerInput(value.Get<float>());
	}

	public void OnYAxis(InputValue value)
	{
		YAxisInput(value.Get<float>());
	}

	public void OnBreak(InputValue value)
	{
		BreakInput(value.isPressed);
	}

	public void OnWheelie(InputValue value)
	{
		WheelieInput(value.isPressed);
	}

	public void OnAttack(InputValue value)
	{
		AttackInput(value.isPressed);
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnPause(InputValue value)
    {
		PauseInput(value.isPressed);
    }

	public void OnItem(InputValue value)
    {
		ItemInput(value.isPressed);
    }

	public void OnDash(InputValue value)
	{
		DashInput(value.isPressed);
	}

//#endif

	public void GasInput(bool newGasState)
	{
		input_gas = newGasState;
	}

	public void SteerInput(float newSteerDirection)
	{
		input_steer = newSteerDirection;
	}

	public void YAxisInput(float newYAxisInput)
    {
		input_Yaxis = newYAxisInput;
    }

	public void BreakInput(bool newBreakState)
	{
		input_break = newBreakState;
	}

	public void WheelieInput(bool newWheelieState)
	{
		input_wheelie = newWheelieState;
	}

	public void AttackInput(bool newAttackState)
	{
		input_attack = newAttackState;
	}

	public void PauseInput(bool newPauseState)
	{
		input_pause = newPauseState;
	}

	public void ItemInput(bool newItemState)
	{
		input_item = newItemState;
	}

	public void JumpInput(bool newJumpState)
	{
		input_jump = newJumpState;
	}

	public void DashInput(bool newDashState)
	{
		input_dash = newDashState;
	}

	//private void OnApplicationFocus(bool hasFocus)
	//{
	//	SetCursorState(cursorLocked);
	//}

	public void SetCursorState(CursorLockMode lockState)
	{
		Cursor.lockState = lockState;
	}
}