using UnityEngine;
using System.Collections;

public class shadow : CustomBehaviour {

	public Transform fxRoot;
	Transform root;
	Vector3 restScale;
	
	void Start() {
		
		// cache references to sublings
		root = transform;
		
		// detach from the player so we don't have to
		// compensate for her root-motion
		fxRoot.parent = null;
		
		// Save the rest-scale, which we will modulate
		// depending on jump-height
		restScale = fxRoot.localScale;
	}
	
	void LateUpdate() {
		
		// Perform a raycast to determine if there's a solid floor below us
		RaycastHit hit;
		if (Physics.Raycast(root.position.Above(Mathf.Epsilon), Vector3.down, out hit)) {
			
			// If there is position on top of it and scale based on the distance
			// so that it's smaller the higher we jump, as you would physically expect
			fxRoot.position = hit.point.Above(0.1f);
			fxRoot.localScale = restScale / (hit.distance+1);
			
		} else {
			
			// If there's no floor below us then scale to zero to effectively
			// hide the shadow.
			//fxRoot.localScale = Vector3.zero;
			fxRoot.position = this.gameObject.transform.parent.position;
			fxRoot.localScale = new Vector3(0.6f,0.6f,0.6f);
			
		}
	}

//	float yPos;
//
//	void Start() {
//
//	}
//	
//	void Update() {
//		
//		// Perform a raycast to determine if there's a solid floor below us
//		RaycastHit hit;
//		if (Physics.Raycast(transform.position, Vector3.down, out hit , 5)) {
//
//			Debug.Log ("CastShow");
//
//			yPos = hit.transform.position.y;
//
//			//transform.localPosition = new Vector3(0, yPos, 0);
//			this.gameObject.transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
//
//			//transform.position.y = hit;
//			
//
//			
//		} else {
//
//			
//		}
//	}
}
