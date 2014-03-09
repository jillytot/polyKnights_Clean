﻿using UnityEngine;
using System.Collections;

public class emberBehavior : CustomBehaviour {

	public int emberValue = 1;
	
	Vector3 target; //disired position
	public float mySpeed = 1.0f; //the speed at which the object moves between its origin and target positions
	bool triggerCollect; //triggers movement behavior

	public AudioClip getCollected;

	GameObject myTarget;


	// Use this for initialization
	void Start () {

	
		//Check to see if the walker is present
		if (gameMaster.walkers.Length > 0) {

			//get target location (walker)
			myTarget = gameMaster.walkers[0];

		} else { 

			//If there are no walkers present, my target is this
			myTarget = this.gameObject;

		}

		triggerCollect = false;
	
	
	}
	
	// Update is called once per frame
	void Update () {

		//If this item has been collected, use this behavior to move from the current location to the walker
		if (triggerCollect == true) {

			target = myTarget.transform.position;

			transform.position = Vector3.Slerp(transform.position, target, mySpeed * Time.deltaTime);

		}
	}

	void OnTriggerEnter (Collider other) {


		var collectMe = other.gameObject.GetComponent<playerMovement>(); //If i collide with a player
		var collected = other.gameObject.GetComponent<safeZone>(); //If i collide with the safe zone / torch lit area

		if (collectMe) {

			//trigger the associated behavior
			if (triggerCollect == false) {
			
					audio.PlayOneShot(getCollected);

			}

			triggerCollect = true;

			//If no walker is present, the embers will give you HP instead...
			if (gameMaster.walkers.Length == 0) {

				collectMe.myHP += emberValue;
				Destroy(this.gameObject);

			}


		}

		if (collected) { 

			//for each ember collected, add value to the torch power
			safeZone.torchPower += emberValue;
			Debug.Log("Torch Power: " + safeZone.torchPower);
			Destroy(this.gameObject);

		}
	}
}