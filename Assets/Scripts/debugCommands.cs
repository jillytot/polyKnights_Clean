using UnityEngine;
using System.Collections;

public class debugCommands : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		
		if (Input.GetKey(KeyCode.Tab)) { //slow game down to 10% speed
			Time.timeScale = 0.1f;
		} else {
			Time.timeScale = 1f;
		}
	}
}
