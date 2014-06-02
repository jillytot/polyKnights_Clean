using UnityEngine;
using System.Collections.Generic;

public class PlayerFactory : MonoBehaviour {
	public GameObject playerPrefab;
	public Material[] materials;
	public Color[] colors;

	List<Player> players;

	// Creates a number of player GameObjects from playerPrefab
	public GameObject[] CreatePlayers(playerClass[] playerClasses) {
		AyloDebug.Assert(playerClasses.Length <= 8);

		players = new List<Player>();
		var positions = new RowPositions(playerClasses.Length);

		for(uint i = 0; i < playerClasses.Length; i++) {
			var playerGameObject = (GameObject)Instantiate(playerPrefab);
			var player = new Player(playerGameObject, i, playerClasses[i]);

			player.material = new PlayerMaterial(materials[i], colors[i]);
			player.position = positions.GetNextPosition();

			players.Add(player);
		}

		// this is temporary
		var playerGameObjects = new List<GameObject>();
		foreach(var player in players)
			playerGameObjects.Add(player.gameObject);
		return playerGameObjects.ToArray();
	}
}

// Simple class that assigns positions in rows in the XZ plane
public class RowPositions {
	int columns = 4;
	float spacing = 3.0f;
	Vector3 center = new Vector3(0.0f, 10.0f, 0.0f);

	int currentPosition = 0;
	int nPositions;

	public RowPositions(int nPositions) {
		AyloDebug.Assert(nPositions > 0);

		this.nPositions = nPositions;
	}

	public Vector3 GetNextPosition() {
		AyloDebug.Assert(currentPosition < nPositions);

		var row = currentPosition / columns;
		var isLastRow = row == nPositions / columns;
		var rowSize = isLastRow && nPositions % columns != 0 ? nPositions % columns : columns;
		var positionInRow = currentPosition % columns;
		var columnSize = (nPositions - 1) / columns + 1;

		Vector3 position = new Vector3();
		position.x = positionInRow * spacing - (rowSize - 1) * spacing / 2;
		position.y = 0;
		position.z = -(row * spacing - (columnSize - 1) * spacing / 2);

		currentPosition++;

		return position;
	}
}

// Used to convert between playerNum enums and their corresponding (zero based) integers
// Hopefully just a temporary solution
public class PlayerNumConverter {
	static playerNum[] indexToPlayerNum = new playerNum[] {
		playerNum.PLAYER1,
		playerNum.PLAYER2,
		playerNum.PLAYER3,
		playerNum.PLAYER4,
		playerNum.PLAYER5,
		playerNum.PLAYER6,
		playerNum.PLAYER7,
		playerNum.PLAYER8
	};

	static Dictionary<playerNum, uint> playerNumToIndex = new Dictionary<playerNum, uint> {
		{playerNum.PLAYER1, 0},
		{playerNum.PLAYER2, 1},
		{playerNum.PLAYER3, 2},
		{playerNum.PLAYER4, 3},
		{playerNum.PLAYER5, 4},
		{playerNum.PLAYER6, 5},
		{playerNum.PLAYER7, 6},
		{playerNum.PLAYER8, 7},
	};

	public static playerNum IndexToPlayerNum(uint i) {
		AyloDebug.Assert(i < indexToPlayerNum.Length);

		return indexToPlayerNum[i];
	}

	public static uint PlayerNumToIndex(playerNum p) {
		return playerNumToIndex[p];
	}
}