using UnityEngine;
using System.Collections;

public class gameMaster : MonoBehaviour {

	//controls universal variables for the game, tracks stats, etc...

	public static bool gameOver;

	public static GameObject[] players;
	public static int playerCount;

	public static GameObject[] walkers;
	public static int walkerCount; 
	public static int walkerHealth;
	

	void Awake () {

		players = GameObject.FindGameObjectsWithTag ("Player");
		walkers = GameObject.FindGameObjectsWithTag ("Walker");
		playerCount = players.Length;
		walkerCount = walkers.Length;

		foreach (GameObject walker in walkers) {

			walkerHealth += walker.GetComponent<damageControl>().myHP;

				}

		Debug.Log ("Number of players = " + playerCount);
		Debug.Log ("Walker Count = " + walkerCount + " & Together they have: " + walkerHealth + "hp");

		}


	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
