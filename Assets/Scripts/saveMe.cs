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

	CharacterController controller; //create instance of character controller

	Vector3 moveDirection;
	
	// Use this for initialization
	void Awake () {

		safe = true;
		storeSpeed = mySpeed;
		controller = GetComponent<CharacterController>();

	}

	void Start () {



	}
	
	// Update is called once per frame
	void Update () {

		//When im dead, stop all motion and put out the light. 
		if (imDead == true) {

			mySpeed = 0;
			myLight.SetActive(false);
			myTorch.SetActive(false);

		} else { //If i am not dead, do all this other stuff

			mySpeed = storeSpeed;

			//triggers the protective field around the walker. 
			if (safeZone.disableProtection == true) {

				safe = false;
		
			} else {

				safe = true;

				}

			if (controller.isGrounded) {
				
				//Get axis inputs and * by speed
				moveDirection = Vector3.forward;
				moveDirection *= mySpeed;

			}

			moveDirection.y -= playerMovement.gravity * Time.deltaTime;

			//eventually the Walker will need to stop when confronted with an obstical
			controller.Move(moveDirection * Time.deltaTime);
			saveMePos = this.gameObject.transform.position;

		}
	
	}

}
