using UnityEngine;
using System.Collections;

public class arenaSpawn : MonoBehaviour {
	public GameObject rock;

	double totalTime = 0;
	// Use this for initialization
	void Start () {
		Debug.Log("Hellssssddfo, world!");
	}
	
	// Update is called once per frame
	void Update () {
		double s = Time.deltaTime;
		totalTime += s;
		if (totalTime > 0.5f) {
			GameObject dffff = (GameObject)Instantiate(rock, transform.position, transform.rotation);
			totalTime = 0.0f;
		}
		
	}
}
