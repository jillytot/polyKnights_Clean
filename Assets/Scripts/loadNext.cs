using UnityEngine;
using System.Collections;

public class loadNext : MonoBehaviour {

	public float loadTime = 5.0f;

	// Use this for initialization
	void Start () {

		StartCoroutine("loadTimer");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator loadTimer () {

		yield return new WaitForSeconds(loadTime);
		Application.LoadLevel(Application.loadedLevel + 1);
	}
}
