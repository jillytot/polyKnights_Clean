using UnityEngine;

public class DualShock {
	
	public class Button : IButton {
		readonly string name;
		
		public Button(string aName) { name = aName; }
		
		public bool Pressed { get { return Input.GetButtonDown(name); } }
		public bool Released { get { return Input.GetButtonUp(name); } }
		public bool Pressing { get { return Input.GetButton(name); } }
	}
	
	public class Stick {
		readonly string xname;
		readonly string yname;
		
		public Stick(string x, string y) { xname=x; yname=y; }
		
		public Vector2 Axis { get { return new Vector2(
			Deadzone(Input.GetAxisRaw(xname)),
			Deadzone(Input.GetAxisRaw(yname))
		); } }
		
		static float Deadzone(float u) {
			float dz = 0.1f;
			if (u > dz) { return u-dz; }
			else if (u < -dz) { return u+dz; }
			else { return 0f; }
		}
	}
	
	public readonly Stick LStick;
	public readonly Stick RStick;
	
	public readonly Button L1;
	public readonly Button R1;
	public readonly Button L2;
	public readonly Button R2;
	
	public readonly Button Start;
	public readonly Button Select;
	
	public readonly Button X;
	public readonly Button O;
	public readonly Button Triangle;
	public readonly Button Square;
	
	public readonly Button Up;
	public readonly Button Down;
	public readonly Button Left;
	public readonly Button Right;
	
	public readonly Button RStickBtn;
	public readonly Button LStickBtn;
	
	public DualShock(int i) {
		var suffix = (i == 0 ? "" : "2");
		
		LStick = new Stick("LeftStickX"+suffix, "LeftStickY"+suffix);
		RStick = new Stick("RightStickX"+suffix, "RightStickY"+suffix);
		
		L1 = new Button("L1"+suffix);
		R1 = new Button("R1"+suffix);
		L2 = new Button("L2"+suffix);
		R2 = new Button("R2"+suffix);
		
		Start = new Button("Start"+suffix);
		Select = new Button("Select"+suffix);
		
		X = new Button("X"+suffix);
		O = new Button("O"+suffix);
		Triangle = new Button("Triangle"+suffix);
		Square = new Button("Square"+suffix);
		
		Up = new Button("Up"+suffix);
		Down = new Button("Down"+suffix);
		Left = new Button("Left"+suffix);
		Right = new Button("Right"+suffix);
		
		RStickBtn = new Button("RStickBtn"+suffix);
		LStickBtn = new Button("LStickBtn"+suffix);
	}
	
	public static DualShock[] Inst = new DualShock[] {
		new DualShock(0),
		new DualShock(1)
	};
}
