﻿using UnityEngine;
using System.Collections;

public class gameMaster : MonoBehaviour {

	//controls universal variables for the game, tracks stats, etc...

	public PlayerFactory playerFactory;
	public playerClass[] playerClasses;

	public static bool gameOver; //Gee, i wonder what this triggers

	public static GameObject[] getPlayers; //Store references to player objects for all to access
	public static Transform[] playerTransforms; //Store player Transforms for all to access
	public static int playerCount; //# of players
	public static int[] playerHP; // This should store the HP values for all players
	public static int[] playerMaxHP; // Stores the Max HP values for all players
	public static playerNum[] playerNames; //Stores the player designation, i.e. player 1
	public static damageControl[] getDamage; //temporarily am using this to try to reference the value for  HP so it updates properly
	public static int[] killCount;

	public static GameObject[] walkers; //store references to Walker objects for all to access
	public static int walkerCount; //Not sure if this will be needed, but good to have
	public static int walkerHealth; //Tracking total walker health (all walkers if applicable). 
	public static int torchWalkerHP; //Tracking main walker Health
	//public GameObject mainWalker;

	public bool MultiplayOn;
	public static bool multiplayer;

	public delegate void getStatUpdate(); //Place holder for creating event
	public static event getStatUpdate getPlayerStats; //place holder for creating event.
	public static bool updateMyStats; //place holder for triggering the event. 

	public static int[] playerIndexer;
	
	void Awake () {

		//initializing values

		if(!playerFactory)
			getPlayers = GameObject.FindGameObjectsWithTag ("Player"); // So the scenes without a PlayerFactory won't break.
		else if(GameObject.Find("playerDescriptions")) {
			// This is the "real" one. The two other cases are only there to make things work during the transition.
			var playerDescriptions = GameObject.Find("playerDescriptions").GetComponent<PlayerDescriptions>().descriptions;
			getPlayers = playerFactory.CreatePlayers(playerDescriptions);
		}
		else {
			// So scenes with a PlayerFactory but with PlayerDescriptions (from the player selection screen) won't break.
			var playerDescriptions = new PlayerDescription[playerClasses.Length];
			for(int i = 0; i < playerClasses.Length; i++) {
				var description = new PlayerDescription();
				description.playerClass = playerClasses[i];
				description.playerColorIndex = i;
				playerDescriptions[i] = description;
			}

			getPlayers = playerFactory.CreatePlayers(playerDescriptions);
		}

		walkers = GameObject.FindGameObjectsWithTag ("Walker");
		playerTransforms = new Transform[getPlayers.Length]; //Create a reference to the transforms of the players
		playerHP = new int[getPlayers.Length];
		playerMaxHP = new int[getPlayers.Length];
		playerNames = new playerNum[getPlayers.Length];
		getDamage = new damageControl[getPlayers.Length];
		playerCount = getPlayers.Length;
		walkerCount = walkers.Length;
		killCount = new int[getPlayers.Length];
		//playerIndexer = new int[getPlayers.Length];

		if (MultiplayOn == true) {multiplayer = true;} else {multiplayer = false;}
		//hahah rewrite this
		foreach (GameObject walker in walkers) {
			walkerHealth += walker.GetComponent<damageControl>().myHP;
				}
		//Debug.Log ("Number of players = " + playerCount);
		//Debug.Log ("Walker Count = " + walkerCount + " & Together they have: " + walkerHealth + "hp");

		//assign player transforms.
		for (int i = 0; i < getPlayers.Length; i++) {
			//store references to transforms
			playerTransforms[i] = getPlayers[i].GetComponent<Transform>();
			//storeReferences to playerHP
			getDamage[i] = getPlayers[i].GetComponent<damageControl>();
			playerHP[i] = getDamage[i].myHP;
			playerMaxHP[i] = getDamage[i].myMaxHp;
			playerNames[i] = getPlayers[i].GetComponent<playerMovement>().thisPlayer;
			Debug.Log(playerNames[i] + " P" + (i + 1) + " HP: " + playerHP[i] + "/" + playerMaxHP[i]);
		}
	}


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < getPlayers.Length; i++) {
			playerHP[i] = getDamage[i].myHP;
			playerMaxHP[i] = getDamage[i].myMaxHp;
		}
		if (updateMyStats == true) {
			if (getPlayerStats != null) {
				getPlayerStats();
				Debug.Log("Updating player Stats");
			}
			updateMyStats = false;
		}
	}	
}
