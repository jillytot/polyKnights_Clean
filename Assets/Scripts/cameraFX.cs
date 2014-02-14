using UnityEngine;
using System.Collections;

public class cameraFX : MonoBehaviour {

	public Transform myLeader;
	Vector3 cameraTarget;
	Vector3 offset;
	float altitude;

	float camSpeed = 1.0f;

	//TODO: camera centers on players as they move around
	//camera will not move beyond a certain limit (i.e. edge of map);
	//Camera zooms in a little closer when all characters are close together


	// Use this for initialization
	void Start () {

		//If there is a walker on the map, assign myLeader
		if (gameMaster.walkers.Length > 0) {

			myLeader = gameMaster.walkers[0].GetComponent<Transform>();

		}

		//If walker is present, assign to follow them
		if (myLeader) {

			offset = transform.position - myLeader.transform.position;
			altitude = transform.position.y;
	
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (myLeader) { 

		cameraTarget = myLeader.transform.position;
	
		//track camera target while maintainging distance. 
		Vector3 targetThis = new Vector3(cameraTarget.x + offset.x, altitude, cameraTarget.z + offset.z);
		transform.position = Vector3.Slerp(transform.position, targetThis, camSpeed * Time.deltaTime);


		}
	}

	void averageCameraCenter () {

		//get all the positions of players, find the the greatest x and z  offsets, and divide them by 2 to get the camera center point.

	}
}
