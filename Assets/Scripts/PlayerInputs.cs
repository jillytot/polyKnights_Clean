using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Takes care of creating PlayerInput objects for the players.
// 
// TODO: This needs to be updated after we agree on how we're going to handle the input.
public class PlayerInputs : MonoBehaviour, IEnumerator, IEnumerable {
	IList<PlayerInput> inputs = new List<PlayerInput>();
	int enumeratorIndex = -1;

	void Start() {
		CreateInputs();
	}

	void CreateInputs() {
		for(int i = 1; i <= 8; i++) {
			PlayerInput input = new PlayerInput();
			
			string namePrefix = "" + i + "p";

			if(i == 1)
				namePrefix = "";

			input.AssignControl(PlayerInput.Horizontal, "Horizontal");
			input.AssignControl(PlayerInput.Vertical, "Vertical");
			input.AssignControl(PlayerInput.Start, namePrefix + "Fire1");
			input.AssignControl(PlayerInput.Fire1, namePrefix + "Fire1");
			input.AssignControl(PlayerInput.Fire2, namePrefix + "Fire2");
//			input.AssignControl(PlayerInput.Fire3, namePrefix + "Fire3");
			input.AssignControl(PlayerInput.Jump, namePrefix + "Jump");
			
			inputs.Add(input);
		}
	}

	public PlayerInput GetInput(int input) {
		return inputs[input];
	}

	public IEnumerator GetEnumerator() {
		Reset();
		return this;
	}

	public void Reset() {
		enumeratorIndex = -1;
	}

	public bool MoveNext() {
		if(enumeratorIndex >= inputs.Count - 1)
			return false;

		enumeratorIndex++;
		return true;
	}

	public object Current {
		get {
			return inputs[enumeratorIndex];
		}
	}
}