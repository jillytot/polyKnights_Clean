using UnityEngine;
using System.Collections;

public class cameraFX : MonoBehaviour {

	Transform myLeader;
	Vector3 cameraTarget;
	Vector3 offset;
	float altitude;

	public float camSpeed = 1.0f;
	public float zoomSpeed = 2.0f;

	public float zoomFloor = 15f;
	public float zoomCeiling = 50f;
	public float zoomScaling = 1;

	public GameObject holdCamera;
	Vector3 zoomOffset;

	float playerDistances;
	Transform playerCenter;

	bool arenaCam = false;

	
	//camera will not move beyond a certain limit (i.e. edge of map);
	//Camera zooms in a little closer when all characters are close together


	// Use this for initialization
	void Start () {

		//If there is a walker on the map, assign myLeader
		if (gameMaster.walkers.Length > 0) {

			myLeader = gameMaster.walkers[0].GetComponent<Transform>();

		} else { 

			//If there are multiple players, center the camera between them
			myLeader = gameMaster.playerTransforms[0];

			if (gameMaster.playerTransforms.Length > 1) {

				arenaCam = true;

				//because i am using Transform instead of position, i have to create a transform for the camera to track for Multiplayer.
				var centerObject = new GameObject("center");
				centerObject.transform.position = playerTracking();

				myLeader = centerObject.GetComponent<Transform>();

			}
		}

		//If walker is present, assign to follow them
		if (myLeader) {

			offset = transform.position - myLeader.transform.position;

			altitude = transform.position.y;
	
		}

		zoomOffset = holdCamera.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

		if (myLeader) { 


			if (arenaCam == true) {

				if (gameMaster.playerTransforms.Length > 1) {

				cameraTarget = playerTracking();

				} else {

					cameraTarget = gameMaster.playerTransforms[0].transform.position;

				}

			} else {

				cameraTarget = myLeader.transform.position;

			}


			float relativeY =  transform.position.y - cameraTarget.y;

	
			//track camera target while maintainging distance.
			Vector3 targetThis = new Vector3(cameraTarget.x + offset.x, altitude + cameraTarget.y, cameraTarget.z + offset.z);
			transform.position = Vector3.Slerp(transform.position, targetThis, camSpeed * Time.deltaTime);

			//zoom in and out as characters get closer and further apart
			//zoomScaling = 10;
			zoomScaling = playerDistances - zoomFloor;

			if (zoomScaling > zoomCeiling) {

				zoomScaling = zoomCeiling;

			}

			var targetZoom = new Vector3(zoomOffset.x, zoomOffset.y, zoomOffset.z) + (holdCamera.transform.forward * zoomScaling * -1);
			holdCamera.transform.localPosition = Vector3.Lerp(holdCamera.transform.localPosition, targetZoom, zoomSpeed * Time.deltaTime);
		
		}
	}

	void averageCameraCenter () {

		//get all the positions of players, find the the greatest x and z  offsets, and divide them by 2 to get the camera center point.

	}

	Vector3 averageCenter() {

		float averageX = 0;
		float averageY = 0;
		float averageZ = 0;

		for (int i = 0; i < gameMaster.playerTransforms.Length; i++) {

			averageX += gameMaster.playerTransforms[i].transform.position.x;
			averageY += gameMaster.playerTransforms[i].transform.position.y;
			averageZ += gameMaster.playerTransforms[i].transform.position.z;

		}

		averageX /= gameMaster.playerTransforms.Length;
		averageY /= gameMaster.playerTransforms.Length;
		averageZ /= gameMaster.playerTransforms.Length;

		Vector3 AverageV = new Vector3(averageX, averageY, averageZ);
		return AverageV;
	}

	Vector3 playerTracking() {
		
		float minX = 0;
		float minY = 0;
		float minZ = 0;
		float maxX = 0;
		float maxY = 0;
		float maxZ = 0;

		for (int i = 0; i < gameMaster.playerTransforms.Length; i++) {

			if (i == 0) {

				minX = gameMaster.playerTransforms[i].transform.position.x;
				minY = gameMaster.playerTransforms[i].transform.position.y;
				minZ = gameMaster.playerTransforms[i].transform.position.z;
				maxX = gameMaster.playerTransforms[i].transform.position.x;
				maxY = gameMaster.playerTransforms[i].transform.position.y;
				maxZ = gameMaster.playerTransforms[i].transform.position.z;

			} else {

				if (minX > gameMaster.playerTransforms[i].transform.position.x) {

					minX = gameMaster.playerTransforms[i].transform.position.x;

				}

				if (minY > gameMaster.playerTransforms[i].transform.position.y) {
					
					minY = gameMaster.playerTransforms[i].transform.position.y;
					
				}

				if (minZ > gameMaster.playerTransforms[i].transform.position.z) {
					
					minZ = gameMaster.playerTransforms[i].transform.position.z;
					
				}

				if (maxX < gameMaster.playerTransforms[i].transform.position.x) {

					maxX = gameMaster.playerTransforms[i].transform.position.x;
				}

				if (maxY < gameMaster.playerTransforms[i].transform.position.y) {
					
					maxY = gameMaster.playerTransforms[i].transform.position.y;
				}

				if (maxZ < gameMaster.playerTransforms[i].transform.position.z) {
					
					maxZ = gameMaster.playerTransforms[i].transform.position.z;
				}
			}
		}

		//Find the distance between the furthest apart players
		Vector3 maxDist = new Vector3(maxX, maxY, maxZ);
		Vector3 minDist = new Vector3(minX, minY, minZ);
		playerDistances = Vector3.Distance(maxDist, minDist);
		//Debug.Log("Distance between players: " + playerDistances);

		//Find the center between all the players
		Vector3 centerPoint = new Vector3 ((maxX + minX) /2, (maxY + minY) /2, (maxZ + minZ) /2);
		return centerPoint;
	}
	
}
