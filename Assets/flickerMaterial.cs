using UnityEngine;
using System.Collections;

public class flickerMaterial : MonoBehaviour {

	GameObject myMat;
	bool flickerMat;

	// Use this for initialization
	void Start () {

		myMat = this.gameObject;
		flickerMat = true;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (flickerMat == true) {

			myMat.renderer.enabled = false;
			flickerMat = false;

		} 

		if ( flickerMat == false) {

			myMat.renderer.enabled = true;
			flickerMat = true;
		}
	}
}
