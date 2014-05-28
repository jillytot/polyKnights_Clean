using UnityEngine;
using System.Collections.Generic;

public class PlayerInput {
	public const int Horizontal = 0;
	public const int Vertical = 1;
	public const int Left = 2;
	public const int Right = 3;
	public const int Up = 4;
	public const int Down = 5;
	public const int Start = 6;
	public const int Fire1 = 7;
	public const int Fire2 = 8;
	public const int Fire3 = 9;
	public const int Jump = 10;
	public const int numberOfInputs = 11;

	class InputStatus {
		public bool inputIsDown = false;
		public bool inputIsPressed = false;
		public float lastPressedTime = 0;
	}

	InputStatus[] inputStatuses = new InputStatus[numberOfInputs];
	string[] inputs = new string[numberOfInputs];
	int lastUpdate = 0;

	public PlayerInput() {
		for(int i = 0; i < numberOfInputs; i++) {
			inputs[i] = "";
			inputStatuses[i] = new InputStatus();
		}

	}

	public void Update() {
		if(Time.frameCount <= lastUpdate)
			return;

		lastUpdate = Time.frameCount;

		// Skips Horizontal and Vertical since they can't be pushed down
		for(int i = Left; i < numberOfInputs; i++) {
			bool inputIsDown = InputIsDown(i);
			bool inputIsPressed = false;

			if(inputIsDown) {
				float deltaTime = Time.time - inputStatuses[i].lastPressedTime;

				bool firstFrameDown = !inputStatuses[i].inputIsDown;
				bool timeToRepeat = !firstFrameDown && deltaTime > 0.3f;

				if(firstFrameDown || timeToRepeat) {
					inputIsPressed = true;
					inputStatuses[i].lastPressedTime = Time.time;
				}
			}

			inputStatuses[i].inputIsDown = inputIsDown;
			inputStatuses[i].inputIsPressed = inputIsPressed;
		}
	}

	// Assign a Unity input name to an input
	public void AssignControl(int input, string unityInput) {
		AyloDebug.Assert(input != Left && input != Right && input != Up && input != Down);
		AyloDebug.Assert(!ControlHasBeenSet(input));
		AyloDebug.Assert(unityInput != "");

		inputs[input] = unityInput;
	}

	// Same as Input.GetAxis
	public float GetAxis(int input) {
		AyloDebug.Assert(ControlHasBeenSet(input));

		return Input.GetAxis(inputs[input]);
	}

	// Same as Input.GetAxisRaw
	public float GetAxisRaw(int input) {
		AyloDebug.Assert(ControlHasBeenSet(input));
		
		return Input.GetAxisRaw(inputs[input]);
	}

	// Returns true if the input is currently pressed down.
	public bool GetDown(int input) {
		AyloDebug.Assert(ControlHasBeenSet(input));

		Update ();
		return inputStatuses[input].inputIsDown;
	}

	// Returns true if the input was pressed down this frame.
	public bool GetPressed(int input) {
		AyloDebug.Assert(ControlHasBeenSet(input));

		Update ();
		return inputStatuses[input].inputIsPressed;
	}

	bool InputIsDown(int input) {
		AyloDebug.Assert(input != Horizontal);
		AyloDebug.Assert(input != Vertical);

		if(input == Left || input == Right || input == Up || input == Down)
			return AxisIsDown(input);
		else if(inputs[input] == "")
			return false;
		else
			return Input.GetButtonDown(inputs[input]);
	}

	bool AxisIsDown(int input) {
		int axis = (input == Left || input == Right) ? Horizontal : Vertical;
		int direction = (input == Right || input == Up) ? 1 : -1;

		return direction * GetAxisRaw(axis) == 1;
	}

	bool ControlHasBeenSet(int input) {
		if(input == Left || input == Right)
			return inputs[Horizontal] != "";

		if(input == Up || input == Down)
			return inputs[Vertical] != "";

		return inputs[input] != "";
	}
}