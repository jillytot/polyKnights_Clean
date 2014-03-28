using UnityEngine;
using System.Collections;

public class saveMe : damageControl {

	public Vector3 saveMePos; //Position of saveMe character
	public bool safe; //While safe, walker won't be targeted
	Vector3 myPos; // stores Walker's position
	Quaternion myRot; //store Walker rotation
	public static int healAmount = 1;

	public float mySpeed = 5f; //Walker speed
	float storeSpeed;
	public GameObject myLight;
	public GameObject myTorch;
	
	// Use this for initialization
	void Awake () {

		safe = true;
		storeSpeed = mySpeed;

	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (imDead == true) {

			mySpeed = 0;
			myLight.SetActive(false);
			myTorch.SetActive(false);

		}

		saveMePos = this.gameObject.transform.position;

		//eventually the Walker will need to stop when confronted with an obstical
		transform.position += Vector3.forward * mySpeed * Time.deltaTime;

		if (safeZone.disableProtection == true) {

			safe = false;
		
		} else {

			safe = true;

		}
	
	}

}
