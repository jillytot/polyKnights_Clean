using UnityEngine;

public class Player {
	GameObject myGameObject;
	uint playerNumber;
	playerMovement myPlayerMovement;
	CharacterController characterController;
	
	public Player(GameObject gameObject, uint playerNumber, playerClass pc) {
		myGameObject = gameObject;
		this.playerNumber = playerNumber;
		
		myPlayerMovement = myGameObject.GetComponent<playerMovement>();
		characterController = myGameObject.GetComponent<CharacterController>();
		
		myPlayerMovement.thisPlayer = PlayerNumConverter.IndexToPlayerNum(playerNumber);
		myPlayerMovement.myClass = pc;
	}
	
	public GameObject GameObject {
		get {
			return myGameObject;
		}
	}
	
	public Vector3 Position {
		get {
			return characterController.transform.position;
		}
		
		set {
			characterController.transform.position = value;
		}
	}
	
	public PlayerColor Color {
		set {
			// TODO: Needs to be updated when the mage mesh is ready.
			myGameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = value.materials[0];
			myPlayerMovement.storeMat = value.materials[0];
			
			myGameObject.transform.Find("FX").Find("direction").gameObject.GetComponent<MeshRenderer>().material.color = value.color;
			
			SetTextColor(value.color);
		}
	}
	
	void SetTextColor(Color color) {
		var playerStats = GameObject.Find("cameraMan").transform.FindChild("battleCam").gameObject.transform.FindChild("playerStats");

//		foreach(var playerInfo in GameObject.FindGameObjectsWithTag("PlayerInfo")) {
		for(int iPlayerInfo = 0; iPlayerInfo < playerStats.childCount; iPlayerInfo++) {
			var playerInfo = playerStats.GetChild(iPlayerInfo).gameObject;
			var stats = playerInfo.GetComponent<displayPlayerStats>();
			
			if(stats.displayNum == myPlayerMovement.thisPlayer) {
				//				stats.showName = "p" + (iPlayer + 1);
				var textMesh = playerInfo.GetComponent<TextMesh>();
				textMesh.color = color;
			}
		}
	}
}