using UnityEngine;
using System.Collections;

public interface IButton {
	bool Pressed { get; }
	bool Released { get; }
	bool Pressing { get; }
}

public class KeyboardButton : IButton
{
	readonly KeyCode code;
	
	public KeyboardButton(KeyCode code) {
		this.code = code;
	}
	
	public bool Pressed {
		get { return Input.GetKeyDown(code); }
	}
	public bool Released {
		get { return Input.GetKeyUp(code); }
	}
	public bool Pressing {
		get { return Input.GetKey(code); }
	}
}