using UnityEngine;
using System.Collections;

public class cameraFX : Math3d {

	Transform myLeader; //used for choosing the camera target
	Vector3 cameraTarget; //what the camera is actually tracking
	Vector3 offset; //When the camera is started in the scene, it saves it's position relative to the cameraTarget, and maintains that offset during play
	float altitude; //Camera's vertical value saved. 

	public float camSpeed = 1.0f; //How fast the camera tracks it's target
	public float zoomSpeed = 2.0f; //How quickly the camera zooms in and out

	public float zoomFloor = 15f; //The most the camera will zoom in
	public float zoomCeiling = 50f; //the most the camera will zoom out
	public float zoomScaling = 1; //the current zoom level of the camera between zoomFloor and zoomCeiling

	Vector3 zoomOffset; //the initial zoom the camera starts at
	
	float playerDistances; //used to calculate camera target for multiple players
	float storeZoom; //used to calcuate camera zooming
	Transform playerCenter; //averaged position of all tracked players
	
	bool arenaCam = false; //if arena cam is true, the camera will move more freely
	bool walkerMode; //If the walker is present, then the camera will take them into account for calculations

	public GameObject holdCamera; //this is the game object holding the camera
	private Camera battleCam; //This is a reference to the camera component to the game object holding the camrea

	//=====Clamp players to camera view
	private Plane[] fPlanes; //The frustum planes of the main camera, used to calculate the edge of the screen for the players
	private int planeIndex; //used for frustrum planes
	private GameObject[] fpObjects; //create a gameobject containing the plane we just created

	private Plane playableSurface; //the area perpendicular to the y axis (up) players can move in
	private GameObject surfaceHolder;
	
	private Collider[] playerColliders; //the colliders of all the players. May no longer be needed...

	//These represent the egdes of the viewable camera frame
	public static Vector3 boundNorth;
	public static Vector3 boundSouth;
	public static Vector3 boundEast;
	public static Vector3 boundWest;

	//=======DEBUG STUFF=========
	public bool showCameraTarget;
	bool triggerShowTarget;
	public GameObject showTarget;
	GameObject showTargetInst;
	public bool showFrustumPlanes;
	bool storeShowPlanes;
	public bool keepPlayerInCamera;
	public static bool triggerBounds;
	//============================
	
	//TODO: the camera can only move so far away from the walker before stopping. 
	//TODO: Camera stops tracking dead players out of camera bounds.

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
		if (fpObjects.Length > 0) {
			for (int j = 0; j < fpObjects.Length; j++) {
				Collider planeCollider = fpObjects[j].gameObject.GetComponent<Collider>();
				planeCollider.enabled = false;
				MeshRenderer planeRender = fpObjects[j].gameObject.GetComponent<MeshRenderer>();
				planeRender.enabled = false;
			}
		}
		playableSurface = new Plane();
		surfaceHolder = GameObject.CreatePrimitive(PrimitiveType.Plane);
		surfaceHolder.transform.position = Vector3.zero;
		surfaceHolder.transform.rotation = Quaternion.LookRotation(Vector3.forward);

		boundingSurface();
	}
	
	void Start () {

		Collider surfaceCollider = surfaceHolder.gameObject.GetComponent<Collider>();
		surfaceCollider.enabled = false;
		MeshRenderer surfaceRender = surfaceHolder.gameObject.GetComponent<MeshRenderer>();
		surfaceRender.renderer.enabled = false;

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
		triggerBounds = keepPlayerInCamera;
		//we need to draw new planes to calculate the camera boundary for each update
		fPlanes = new Plane[planeIndex];
		fPlanes = GeometryUtility.CalculateFrustumPlanes(battleCam);
		int j = 0;
		while (j < fPlanes.Length) {
			fpObjects[j].transform.position = -fPlanes[j].normal * fPlanes[j].distance;
			fpObjects[j].transform.rotation = Quaternion.FromToRotation(Vector3.up, fPlanes[j].normal);
			j++;
		}

		//chose camera target based on game settings
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
		togglePlanes();
		boundingSurface();
	}

	//calculate the edges for the edge of screen colliders
	void boundingSurface () {
		Vector3[] linePoints = new Vector3[4];
		Vector3[] lineVectors = new Vector3[4];
		for (int i = 0; i < linePoints.Length; i++) {
			PlanePlaneIntersection( 
			                       out linePoints[i],
			                       out lineVectors[i],
			                       surfaceHolder.transform.rotation * Vector3.up, 
			                       surfaceHolder.transform.position, 
			                       fpObjects[i].transform.rotation *Vector3.up, 
			                       fpObjects[i].transform.position);
			Debug.DrawRay(linePoints[i], lineVectors[i] * 100, Color.cyan);
			Debug.DrawRay(linePoints[i], lineVectors[i] * -100, Color.cyan);
			//Debug.Log("Camera Boundary " + i + " = " + linePoints[i]);
		}

		//TODO:not sure if these results are always consistent, they might need to be sorted.
		boundNorth = linePoints[3];
		boundSouth = linePoints[2];
		boundEast = linePoints[1];
		boundWest = linePoints[0];
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
		var closestIndex = -1;
		for (int i = 0; i < gameMaster.playerTransforms.Length; i++) {

			if (gameMaster.getDamage[i].myHP < 1) {
				//This is super useful
				continue;
			}
			if (closestIndex == -1) {
				closestIndex = i;
			}
			if (i == closestIndex) {
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

	void togglePlanes () {
		
		if (!showFrustumPlanes && showFrustumPlanes != storeShowPlanes) {
			if (fpObjects.Length > 0) {
				for (int i = 0; i < fpObjects.Length; i++) {
					Collider planeCollider = fpObjects[i].gameObject.GetComponent<Collider>();
					planeCollider.enabled = false;
					MeshRenderer planeRender = fpObjects[i].gameObject.GetComponent<MeshRenderer>();
					planeRender.enabled = false;
				}
			}
			if (surfaceHolder) {
				Collider surfaceCollider = surfaceHolder.gameObject.GetComponent<Collider>();
				surfaceCollider.enabled = false;
				MeshRenderer surfaceRender = surfaceHolder.gameObject.GetComponent<MeshRenderer>();
				surfaceRender.enabled = false;
			}
		} 
		
		if (showFrustumPlanes && showFrustumPlanes != storeShowPlanes) {
			if (fpObjects.Length > 0) {
				for (int i = 0; i < fpObjects.Length; i++) {
					MeshRenderer planeRender = fpObjects[i].gameObject.GetComponent<MeshRenderer>();
					planeRender.enabled = true;
				}
			}
			if (surfaceHolder) {
				MeshRenderer surfaceRender = surfaceHolder.gameObject.GetComponent<MeshRenderer>();
				surfaceRender.enabled = true;
			}
		} 
		storeShowPlanes = showFrustumPlanes;
	}
}
