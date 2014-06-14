/*
Author: Adrian Alberto
Version: 0.2
Updated: 13 June 2014

Simple automatic spawning system.
Currently spawns at a fixed rate.
Planned features:
	Procedurally generated waves
	Increasing difficulty
	More creatures
	Confirm successful spawn within map




DESIGN
[small x 10]
[small x 20]
[small x 20, large x 3]

*/

using UnityEngine;
using System.Collections;


public class arenaSpawn : MonoBehaviour {
	public GameObject creature;
	//public GameObject creature2;
	public float spawnInterval = 2.5f; // Time between spawns.
	double totalTime = 0;
	int spawnCount = 0;
	int wave = 1;
	
	// Use this for initialization
	void Start () {
		Debug.Log("ArenaSpawn Loaded.");
	}
	
	// Update is called once per frame
	void Update () {
		double s = Time.deltaTime; //Elapsed time between current and previous frame
		totalTime += s; //Manually keep track of total time elapsed

		if (totalTime > spawnInterval && GameObject.Find("cameraTarget") != null) {
			//Enough time has passed, spawn a creature based on camera position and center of map
			Vector3 final;
			Vector3 center = transform.position;
			Vector3 campos = GameObject.Find("cameraTarget").transform.position;

			final = campos + (Vector3.Scale((center - campos).normalized, new Vector3(1,0,1)) * 100) + new Vector3(0,100,0);

			for (int i = 0; i < 4; i++){
				GameObject.Instantiate(creature, final + new Vector3(Mathf.Sin(i*2*Mathf.PI/4)*4,0,Mathf.Cos(i*2*Mathf.PI/4)*4), transform.rotation);
			}
			
			totalTime = 0; //Reset counter.
			spawnCount++; //Add to number of times spawned, used in wave calculation

			if (spawnCount > wave * 3){
				//Next wave
				spawnCount = 0;
				wave++;
			}
		}
	}
}