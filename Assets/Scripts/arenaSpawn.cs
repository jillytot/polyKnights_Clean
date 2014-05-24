/*
Author: Adrian Alberto
Version: 0.1
Updated: 23 May 2014

Simple automatic spawning system.
Currently spawns at a fixed rate.
Planned features:
	Procedurally generated waves
	Increasing difficulty
	More creatures
	Confirm successful spawn within map
*/

using UnityEngine;
using System.Collections;


public class arenaSpawn : MonoBehaviour {
	public GameObject creature;
	public float spawnInterval = 2.5f; // Time between spawns.
	double totalTime = 0;
	
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

			GameObject.Instantiate(creature, final, transform.rotation);
			totalTime = 0; //Reset counter.
		}
	}
}