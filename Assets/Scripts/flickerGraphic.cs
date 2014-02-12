using UnityEngine;
using System.Collections;

public class flickerGraphic : MonoBehaviour {

	public bool flick = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (flick == false) {

			flick = true;
			this.gameObject.renderer.enabled = false;
			StartCoroutine("flickRate");

		} 
	}

	IEnumerator flickRate () {
		
		yield return new WaitForSeconds (0.1f);
		this.gameObject.renderer.enabled = true;
		flick = true;
	}
}
