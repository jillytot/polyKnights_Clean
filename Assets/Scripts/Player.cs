using UnityEngine;

public struct PlayerMaterial {
	public Material material;
	public Color color;
	
	public PlayerMaterial(Material material, Color color) {
		this.material = material;
		this.color = color;
	}
}

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
	
	public GameObject gameObject {
		get {
			return myGameObject;
		}
	}
	
	public Vector3 position {
		get {
			return characterController.transform.position;
		}
		
		set {
			characterController.transform.position = value;
		}
	}
	
	public PlayerMaterial material {
		set {
			gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = value.material;
			myPlayerMovement.storeMat = value.material;
			
			gameObject.transform.Find("FX").Find("direction").gameObject.GetComponent<MeshRenderer>().material.color = value.color;
			
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