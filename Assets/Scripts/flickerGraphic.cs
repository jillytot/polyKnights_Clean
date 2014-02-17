using UnityEngine;
using System.Collections;

public class flickerGraphic : MonoBehaviour {

	public bool flick = false;
	public bool keepFlickering;
	public float flickerRate;

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
		
		yield return new WaitForSeconds (flickerRate);
		this.gameObject.renderer.enabled = true;
		if (keepFlickering == true) {

			flick = false;

		} else {

		flick = true;

		}
	}
}
