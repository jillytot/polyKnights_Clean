using UnityEngine;
using System.Collections;

public class disableRenderer : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//disables the renderer of any attached gameobject with a mesh renderer
		renderer.enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
