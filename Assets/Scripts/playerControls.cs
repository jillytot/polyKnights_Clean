using UnityEngine;
using System.Collections;

public enum playerNum { //playerNumber 
	
	PLAYER1,
	PLAYER2,
	PLAYER3,
	PLAYER4,
	PLAYER5,
	PLAYER6,
	PLAYER7,
	PLAYER8,
	
}

//Thanks David for the help here
	public class Controls  {

	public string horizontal = "Horizontal";
	public string vertical = "Vertical";
	
	public string fire1 = "Fire1";
	public string fire2 = "Fire2";
	public string fire3 = "Fire3";
	
	public string jump = "Jump";

}

public class playerControls  {
	
	public static Controls getControls(playerNum getPlayerNum) {

		var controls = new Controls();

		switch (getPlayerNum) {
			
		case playerNum.PLAYER1:
			MonoBehaviour.print ("this is player 1!!");

			controls.horizontal = "1pHorizontal";
			controls.vertical = "1pVertical";

			controls.fire1 = "1pFire1";
			controls.fire2 = "1pFire2";
			controls.fire3 = "1pFire3";

			controls.jump = "1pJump";

			break;

		case playerNum.PLAYER2:
			//print ("this is player 2!!");

			controls.horizontal = "2pHorizontal";
			controls.vertical = "2pVertical";
			
			controls.fire1 = "2pFire1";
			controls.fire2 = "2pFire2";
			controls.fire3 = "2pFire3";
			
			controls.jump = "2pJump";

			break;
		case playerNum.PLAYER3:
			//print ("this is player 3!!");

			controls.horizontal = "3pHorizontal";
			controls.vertical = "3pVertical";
			
			controls.fire1 = "3pFire1";
			controls.fire2 = "3pFire2";
			controls.fire3 = "2pFire3";
			
			controls.jump = "3pJump";

			break;

		case playerNum.PLAYER4:
			//print ("this is player 4!!");

			controls.horizontal = "4pHorizontal";
			controls.vertical = "4pVertical";
			
			controls.fire1 = "4pFire1";
			controls.fire2 = "4pFire2";
			controls.fire3 = "2pFire3";
			
			controls.jump = "4pJump";

			break;

		case playerNum.PLAYER5:
			//print ("this is player 5!!");

			controls.horizontal = "5pHorizontal";
			controls.vertical = "5pVertical";
			
			controls.fire1 = "5pFire1";
			controls.fire2 = "5pFire2";
			controls.fire3 = "2pFire3";
			
			controls.jump = "5pJump";

			break;

		case playerNum.PLAYER6:
			//print ("this is player 6!!");

			controls.horizontal = "6pHorizontal";
			controls.vertical = "6pVertical";
			
			controls.fire1 = "6pFire1";
			controls.fire2 = "6pFire2";
			controls.fire3 = "2pFire3";
			
			controls.jump = "6pJump";

			break;

		case playerNum.PLAYER7:
			//print ("this is player 7!!");

			controls.horizontal = "7pHorizontal";
			controls.vertical = "7pVertical";
			
			controls.fire1 = "7pFire1";
			controls.fire2 = "7pFire2";
			controls.fire3 = "2pFire3";
			
			controls.jump = "7pJump";

			break;

		case playerNum.PLAYER8:
			Debug.Log ("this is player 8!!");

			controls.horizontal = "8pHorizontal";
			controls.vertical = "8pVertical";
			
			controls.fire1 = "8pFire1";
			controls.fire2 = "8pFire2";
			controls.fire3 = "2pFire3";
			
			controls.jump = "8pJump";

			break;

		default:
			Debug.Log ("There is no player... oops!");
			break;
		}
		return controls;
	}
}
