using UnityEngine;
using System.Collections;

public class gameMaster : MonoBehaviour {

	//controls universal variables for the game, tracks stats, etc...

	public static bool gameOver; //Gee, i wonder what this triggers

	public static GameObject[] getPlayers; //Store references to player objects for all to access
	public static Transform[] playerTransforms; //Store player Transforms for all to access
	public static int playerCount; //# of players

	public static GameObject[] walkers; //store references to Walker objects for all to access
	public static int walkerCount; //Not sure if this will be needed, but good to have
	public static int walkerHealth; //Tracking walker health. 
	

	void Awake () {

		getPlayers = GameObject.FindGameObjectsWithTag ("Player");
		walkers = GameObject.FindGameObjectsWithTag ("Walker");
		playerTransforms = new Transform[getPlayers.Length]; //Create a reference to the transforms of the players
		playerCount = getPlayers.Length;
		walkerCount = walkers.Length;

		foreach (GameObject walker in walkers) {

			walkerHealth += walker.GetComponent<damageControl>().myHP;

				}

		Debug.Log ("Number of players = " + playerCount);
		Debug.Log ("Walker Count = " + walkerCount + " & Together they have: " + walkerHealth + "hp");

		//assign player transforms.
		for (int i = 0; i < getPlayers.Length; i++) {
			
			//store references to transforms
			playerTransforms[i] = getPlayers[i].GetComponent<Transform>();
			
		}

	
		}


	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
