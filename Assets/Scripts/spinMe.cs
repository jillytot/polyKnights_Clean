using UnityEngine;
using System.Collections;

public class spinMe : MonoBehaviour {

	//Rotate objects along the following axis
	public int x;
	public int y;
	public int z;

	//set speed for rotation
	public float rotationSpeed;

	// Update is called once per frame
	void Update () {

		//You spin me right round baby right round like a round thing right round right round...
		transform.Rotate (x,y,z * rotationSpeed * Time.deltaTime);
	
	}
}
