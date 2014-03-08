using UnityEngine;
using System.Collections;

public class shadowChild : MonoBehaviour {

	public GameObject myParent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!myParent) {

			Destroy(this.gameObject);

		}
	
	}
}
