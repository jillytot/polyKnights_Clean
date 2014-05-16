using UnityEngine;
using System.Collections.Generic;

public class PlayerFactory : MonoBehaviour {
	public GameObject playerPrefab;
	public Material[] materials;
	public Color[] colors;

	RowPositions positions;

	// creates a number of player GameObjects from playerPrefab
	public GameObject[] SpawnPlayers(playerClass[] playerClasses)
	{
		positions = new RowPositions(playerClasses.Length);
		List<GameObject> players = new List<GameObject>();

		for(int i = 0; i < playerClasses.Length; i++)
		{
			var player = (GameObject)Instantiate(playerPrefab);
			players.Add(player);

			SetPlayerPosition(player);
			SetPlayerMaterial(player, i);
			SetPlayerNumber(player, i);
			SetPlayerClass(player, playerClasses[i]);
			SetPlayerInfoColor(i);
		}

		return players.ToArray();
	}

	void SetPlayerPosition(GameObject player)
	{
		player.transform.position = positions.GetNextPosition();
	}

	void SetPlayerMaterial(GameObject player, int iPlayer)
	{
		Material material = materials[iPlayer];

		player.GetComponentInChildren<SkinnedMeshRenderer>().material = material;
		player.GetComponent<playerMovement>().storeMat = material;
	}

	void SetPlayerNumber(GameObject player, int iPlayer)
	{
		playerNum[] indexToPlayerNum = new playerNum[] {
			playerNum.PLAYER1,
			playerNum.PLAYER2,
			playerNum.PLAYER3,
			playerNum.PLAYER4,
			playerNum.PLAYER5,
			playerNum.PLAYER6,
			playerNum.PLAYER7,
			playerNum.PLAYER8
		};
		
		player.GetComponent<playerMovement>().thisPlayer = indexToPlayerNum[iPlayer];
	}

	void SetPlayerClass(GameObject player, playerClass c)
	{
		player.GetComponent<playerMovement>().myClass = c;
	}

	void SetPlayerInfoColor(int iPlayer)
	{
		Dictionary<playerNum, int> playerNumToIndex = new Dictionary<playerNum, int> {
			{playerNum.PLAYER1, 0},
			{playerNum.PLAYER2, 1},
			{playerNum.PLAYER3, 2},
			{playerNum.PLAYER4, 3},
			{playerNum.PLAYER5, 4},
			{playerNum.PLAYER6, 5},
			{playerNum.PLAYER7, 6},
			{playerNum.PLAYER8, 7},
		};

		foreach(var playerInfo in GameObject.FindGameObjectsWithTag("PlayerInfo"))
		{
			var stats = playerInfo.GetComponent<displayPlayerStats>();

			if(playerNumToIndex[stats.displayNum] == iPlayer)
			{
//				stats.showName = "p" + (iPlayer + 1);
				var textMesh = playerInfo.GetComponent<TextMesh>();
				textMesh.color = colors[iPlayer];
			}
		}
	}
}

// Simple class that assigns positions in rows in the XZ plane
public class RowPositions
{
	int columns = 4;
	float spacing = 3.0f;
	Vector3 center = new Vector3(0.0f, 10.0f, 0.0f);

	int currentPosition = 0;
	int nPositions;

	public RowPositions(int nPositions)
	{
		this.nPositions = nPositions;
	}

	public Vector3 GetNextPosition()
	{
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