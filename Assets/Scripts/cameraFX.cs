using UnityEngine;
using System.Collections;

public class cameraFX : MonoBehaviour {

	public Transform myLeader;
	Vector3 cameraTarget;
	Vector3 offset;
	float altitude;

	float camSpeed = 1.0f;

	// Use this for initialization
	void Start () {

		offset = transform.position - myLeader.transform.position;
		altitude = transform.position.y;
	
	}
	
	// Update is called once per frame
	void Update () {


		cameraTarget = myLeader.transform.position;
		//var offset = cameraTarget + transform.position;
	
		//i'm really confused : [
		Vector3 targetThis = new Vector3(cameraTarget.x + offset.x, altitude, cameraTarget.z + offset.z);
		transform.position = Vector3.Slerp(transform.position, targetThis, camSpeed * Time.deltaTime);


	
	}
}
