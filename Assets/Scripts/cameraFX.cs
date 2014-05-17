using UnityEngine;
using System.Collections;

public class cameraFX : Math3d {

	Transform myLeader;
	Vector3 cameraTarget;
	Vector3 offset;
	float altitude;

	public float camSpeed = 1.0f;
	public float zoomSpeed = 2.0f;

	public float zoomFloor = 15f;
	public float zoomCeiling = 50f;
	public float zoomScaling = 1;

	Vector3 zoomOffset;
	
	float playerDistances;
	float storeZoom; 
	Transform playerCenter;
	
	bool arenaCam = false;
	bool walkerMode;

	public GameObject holdCamera; //this is the game object holding the camera
	private Camera battleCam; //This is a reference to the camera component to the game object holding the camrea

	//=====Clamp players to camera view
	private Plane[] fPlanes; //The frustum planes of the main camera, used to calculate the edge of the screen for the players
	private int planeIndex; //used for frustrum planes
	private GameObject[] fpObjects; //create a gameobject containing the plane we just created

	private Plane playableSurface; //the area perpendicular to the y axis (up) players can move in
	private GameObject surfaceHolder;

	private Collider[] playerColliders;

	//=======DEBUG STUFF=========
	public bool showCameraTarget;
	bool triggerShowTarget;
	public GameObject showTarget;
	GameObject showTargetInst;
	//============================
	
	//TODO: clamp player positions to camera max boundry. - Since the camera is at a tilt, using it's frustrum boundary will mess with movement in Y,
	//I will try calculating the fustrum in x and z where it meets the camera target, but draw the collision plains parallel to the y axis instead of pointed toward the camera
	//TODO: camera doesn't follow players falling off edges
	//TODO: the camera can only move so far away from the walker before stopping. 

	void Awake () {
		battleCam = holdCamera.gameObject.GetComponent<Camera>();
		fPlanes = GeometryUtility.CalculateFrustumPlanes(battleCam);
		fpObjects = new GameObject[fPlanes.Length];
		planeIndex = fPlanes.Length;
		int i = 0;
		while (i < fPlanes.Length) {
			fpObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Plane);
			fpObjects[i].name = "Plane " + i.ToString ();
			fpObjects[i].transform.position = -fPlanes[i].normal * fPlanes[i].distance;
			fpObjects[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, fPlanes[i].normal);
			i++;
		}

		playableSurface = new Plane();
		surfaceHolder = GameObject.CreatePrimitive(PrimitiveType.Plane);
		surfaceHolder.transform.position = Vector3.zero;
		surfaceHolder.transform.rotation = Quaternion.LookRotation(Vector3.forward);
	}

	void Start () {

		playerColliders = new Collider[gameMaster.getPlayers.Length];
		for (int i = 0; i < gameMaster.getPlayers.Length; i++) {
			playerColliders[i] = gameMaster.getPlayers[i].GetComponent<Collider>();

		}

		//If there is a walker on the map, assign myLeader
		if (gameMaster.walkers.Length > 0) {
			myLeader = gameMaster.walkers[0].GetComponent<Transform>();
			walkerMode = true;
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
			storeZoom = myLeader.transform.position.y;
		}
		zoomOffset = holdCamera.transform.localPosition;
	}
	
	void Update () {

		fPlanes = new Plane[planeIndex];
		fPlanes = GeometryUtility.CalculateFrustumPlanes(battleCam);
		int j = 0;
		while (j < fPlanes.Length) {
			fpObjects[j].transform.position = -fPlanes[j].normal * fPlanes[j].distance;
			fpObjects[j].transform.rotation = Quaternion.FromToRotation(Vector3.up, fPlanes[j].normal);
			j++;
		}

		for (int i = 0; i < playerColliders.Length; i++) {

				if (GeometryUtility.TestPlanesAABB(fPlanes, playerColliders[i].collider.bounds)) {
					Debug.Log("Player has hit edge");
				} else { 
					Debug.Log("nothing has been detected");
			}
		}

		if (myLeader) { 
			if (arenaCam == true) {
				if (gameMaster.playerTransforms.Length > 1) {
					cameraTarget = playerTracking();
				} else {
					cameraTarget = gameMaster.playerTransforms[0].transform.position;
				}
			} else {

				if (gameMaster.playerTransforms.Length == 1) {
					cameraTarget = trackWithWalker(myLeader.transform.position, gameMaster.playerTransforms[0].transform.position);
				} else if (gameMaster.playerTransforms.Length > 1) {
					cameraTarget = trackWithWalker(myLeader.transform.position, playerTracking());
				} else {
					cameraTarget = myLeader.transform.position;
				}
			}

			//track camera target while maintainging distance.
			Vector3 targetThis = new Vector3(cameraTarget.x + offset.x, altitude + cameraTarget.y, cameraTarget.z + offset.z);
			transform.position = Vector3.Slerp(transform.position, targetThis, camSpeed * Time.deltaTime);

			//zoom in and out as characters get closer and further apart
			//var zoomFloorOffset = storeZoom - cameraTarget.y;
			if (walkerMode == true) {
				var averageDistance = Vector3.Distance(myLeader.transform.position, cameraTarget);
				averageDistance *= 2;
				zoomScaling = averageDistance - zoomFloor;
			} else {
				zoomScaling = playerDistances - zoomFloor;
			}
			if (zoomScaling > zoomCeiling) {
				zoomScaling = zoomCeiling;
			}

			var targetZoom = new Vector3(zoomOffset.x, zoomOffset.y, zoomOffset.z) + (holdCamera.transform.forward * zoomScaling * -1);
			holdCamera.transform.localPosition = Vector3.Lerp(holdCamera.transform.localPosition, targetZoom, zoomSpeed * Time.deltaTime);
		}

		//show Camera Target to help design camera tracking. 
		if (showCameraTarget == true && triggerShowTarget == false) {
			showTargetInst = (GameObject)Instantiate(showTarget, cameraTarget, transform.rotation);
			triggerShowTarget = true;
		} else if (showCameraTarget == true && showTargetInst) {
			showTargetInst.transform.position = cameraTarget;
		} else if (showCameraTarget == false && showTargetInst) {
			Destroy(showTargetInst);
			triggerShowTarget = false;
		}

		//send a ray from the camera center to each frustum edge, 
		surfaceHolder.transform.position = cameraTarget;
		RaycastHit hit;
		for (int i = 0; i < fPlanes.Length; i ++) {
			//Physics.Raycast(transform.position, Vector3.down, out hit);
			Debug.DrawRay(cameraTarget, fpObjects[i].transform.position, Color.blue);
		}
	}

	//calculate the edges for the edge of screen colliders
	void boundingSurface () {

		Vector3[] boundingLines = new Vector3[4];
		for (int i = 0; i < boundingLines.Length; i++) {
			//boundingLines[i] = PlanePlaneIntersection(surfaceHolder.transform.rotation, surfaceHolder.transform.position, fpObjects[i].transform.rotation, fpObjects[i].transform.position);

		}
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

	//Find the center point between all the players in the game.
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
		//player Distances is used to zoom the camera in and out based on how far spread the players are.
		playerDistances = Vector3.Distance(maxDist, minDist);
		//Debug.Log("Distance between players: " + playerDistances);
		//Find the center between all the players
		Vector3 centerPoint = new Vector3 ((maxX + minX) /2, (maxY + minY) /2, (maxZ + minZ) /2);
		return centerPoint;
	}

	//lLet the camera pan around the walker a little bit as the player moves
	Vector3 trackWithWalker(Vector3 Walker, Vector3 averageWith) {
		float checkPlayers = 2;
		float weightX = (Walker.x + averageWith.x) / checkPlayers;
		float weightY = (Walker.y + averageWith.y)/ checkPlayers;
		float weightZ = (Walker.z + averageWith.z) / checkPlayers;
		Vector3 weightedTarget = new Vector3(weightX, weightY, weightZ);
		return weightedTarget;
	}
}
