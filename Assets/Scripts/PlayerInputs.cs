using UnityEngine;
using System.Collections.Generic;

// Takes care of creating PlayerInput objects for the players.
// 
// TODO: This needs to be updated after we agree on how we're going to handle the input.
public class PlayerInputs : MonoBehaviour {
	public bool playerOneIsSpecial;
	IList<PlayerInput> inputs;

	void CreateInputs() {
		inputs = new List<PlayerInput>();

		for(int i = 1; i <= 8; i++) {
			PlayerInput input = new PlayerInput();
			
			string namePrefix = "" + i + "p";
			
			if(playerOneIsSpecial && i == 1)
				namePrefix = ""; // Use keyboard for player 1

			input.AssignControl(PlayerInput.Horizontal, namePrefix + "Horizontal");
			input.AssignControl(PlayerInput.Vertical, namePrefix + "Vertical");
			input.AssignControl(PlayerInput.Start, namePrefix + "Fire1");/**/
			input.AssignControl(PlayerInput.Fire1, namePrefix + "Fire1");
			input.AssignControl(PlayerInput.Fire2, namePrefix + "Fire2");
//			input.AssignControl(PlayerInput.Fire3, namePrefix + "Fire3");
			input.AssignControl(PlayerInput.Jump, namePrefix + "Jump");
			
			inputs.Add(input);
		}
	}

	public PlayerInput GetInput(int player) {
		if(inputs == null)
			CreateInputs();

		return inputs[player];
	}
}
