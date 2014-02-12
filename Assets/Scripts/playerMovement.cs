using UnityEngine;

using System.Collections;

public class playerMovement : damageControl {

	public float speed = 15.0F; //Max speed of the character
	float newSpeed; //Used to modify speed based on input

	public float jumpSpeed = 30.0F; //How high you can jump in relation to gravity
	public static float gravity = 120.0F; //how fast you fall
	public float turnSpeed = 0.2f; //how fast you the player turns

	public GameObject myAttack; //controls gameobject which executes the attack
	public GameObject myChargeAttack; //Controls Charge Attack
	public float attackSpeed = 0.2F; //used as buffer time before next attack;
	bool nextAttack = true; //returns true when the player is ready to do their next atttack
	//bool attackDone = true;

	bool charging = false; //Triggers Charging
	public float chargeSpeed = 100.00f; //Rate at which you charge
	Vector3 attackDirection; //Direction you are facing when you use charge attack
	public float chargeTimer = 0.5f; //How long you use your charge attack for
	bool startCharge = false;

	Quaternion myRotation; //used to store direction of movement
	float Horizontal; //raw value for Horizontal axis
	float Vertical; //raw value for Vertical axis
	
	private Vector3 moveDirection = Vector3.zero; //initialize movement direction
	private Vector3 lastMoveDirection; //record last movement.
	public Vector3 playerPos;

	Animator myAnimation; // Animation controller

	CharacterController controller; //create instance of character controller

	void Awake() {

		myAnimation = GetComponentInChildren<Animator>(); //Get animation controller and assign it to this character
		controller = GetComponent<CharacterController>();
		myRotation = transform.rotation;

	}
	

//	void FixedUpdate () {
//	
//	}


	void Update() {

		//Get axis values for calculating movement
		Horizontal = Input.GetAxis("Horizontal");
		Vertical = Input.GetAxis("Vertical");

		if (moveDirection.sqrMagnitude > 0.0f) {

			lastMoveDirection = moveDirection;

		}

		//Modifies speed based on axis input
		newSpeed = speedMod();

		//Do basic attack
		basicAttack ();
		chargeAttack ();


		//Ground Based Movement;
		if (controller.isGrounded) {

			//Get axis inputs and * by speed
			moveDirection = new Vector3(Horizontal, 0, Vertical);
			moveDirection *= newSpeed;


			//Apply changes to each child of the game object
			foreach (Transform child in transform)
			
			{

				if (moveDirection.sqrMagnitude > 0) { 

					myAnimation.SetBool("Run", true); //Changes avatar to running state
					var targetRotation = Quaternion.LookRotation(moveDirection); //set target towards direction of motion
					child.rotation = child.rotation.EaseTowards(targetRotation, turnSpeed); //rotate towards the direction of motion
					myRotation = child.rotation;

				}  else {

					myAnimation.SetBool ("Run", false); 

				}
			}

		//How to jump!
		if (Input.GetButtonDown("Jump")) {
			
			moveDirection.y = jumpSpeed;

			
			}

		//Air based movement
		} else {

			//Air Control
			//To do: This is messy, it needs to be cleaned up.
			//Also, when you jump, you automatically face north for some reason when not giving any input. 
			moveDirection.x = Horizontal;
			moveDirection.z = Vertical;

			Vector3 normalizeXZ = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
			normalizeXZ *= newSpeed;
			Vector3 moveInAirDirection = new Vector3(normalizeXZ.x, moveDirection.y, normalizeXZ.z);

			moveDirection = transform.TransformDirection(moveInAirDirection);

			foreach (Transform child in transform)
				
			{
				
				if (moveDirection.sqrMagnitude > 0) { 
					
					myAnimation.SetBool("Run", true); //Changes avatar to running state
					Vector3 lookatMoveDirection = new Vector3(	moveDirection.x, 0, moveDirection.z);
					var targetRotation = Quaternion.LookRotation(lookatMoveDirection); //set target towards direction of motion
					child.rotation = child.rotation.EaseTowards(targetRotation, turnSpeed); //rotate towards the direction of motion
					myRotation = child.rotation;
					
				}  else {
					
					myAnimation.SetBool ("Run", false); 
					
				}
			}
		}

		//Controls Gravity

		moveDirection.y -= gravity * Time.deltaTime;

		if (charging == false) {

		controller.Move(moveDirection * Time.deltaTime);

		} else {

			controller.Move(attackDirection * chargeSpeed * Time.deltaTime);

		}

		//float posX = this.gameObject.transform.position.x;
		//float posZ = this.gameObject.transform.position.z;
		//playerPos = new Vector3(posX, 0, posZ);
		playerPos = this.gameObject.transform.position;

	}

	//void OnTriggerEnter (Collider other) {

	//}



	//Thank Alex Austin for making this work!
	//This controls the speed while using a radial axis analogue stick so that it's constant in any degree at any velocity
	//With this method you don't need to normalize your movement vectors

	float speedMod () {

		//get the absolute value of each axis
		var horAbs = Mathf.Abs(Horizontal);
		var vertAbs = Mathf.Abs(Vertical);
		float angle = 0;

		//do some math...
		if (horAbs > vertAbs) { 

			angle = Mathf.Atan2(vertAbs, horAbs); 

		} else {

			angle = Mathf.Atan2(horAbs, vertAbs);

		}

		newSpeed = Mathf.Cos(angle);
		newSpeed *= speed;

		//It magically works!
		return newSpeed;

	}

	void basicAttack () {

		if (Input.GetButtonDown("Fire1") && nextAttack == true) {
			
			//print ("attack that shit!");
			
			//Enable the attack graphic & the corresponding attack animation.

			Debug.Log("Enable Attack");
			myAttack.SetActive(true); //This is a prefab instance which must be assigned in the editor
			myAnimation.SetBool("attacking", true);
			nextAttack = false;
			
			
		} else {
			
			//when the player releases the button, allow them to attack again.
			myAttack.SetActive(false);
			myAnimation.SetBool("attacking", false);
			nextAttack = true;

		}
	}

	void chargeAttack () {

		if (charging == false) {

			//myAttack.SetActive(false); //This is a prefab instance which must be assigned in the editor

		}

		if (Input.GetButtonDown("Fire2") && nextAttack == true) {

	
			//Get facing direction, and charge forward quickly. 

			//bool startCharge = false; 
			charging = true;

			if (startCharge == false) {

				if (moveDirection.sqrMagnitude == 0) {

					Horizontal = lastMoveDirection.x;
					Vertical = lastMoveDirection.z;

				}

				//This group of statements forces the inputs to go to their maximum
				if (Horizontal > 0) {

					Horizontal = 1;
				}

				if (Horizontal < 0) { 

					Horizontal = -1;

				}

				if (Vertical > 0) {	

					Vertical = 1;
				}
				
				if (Vertical < 0) { 

					Vertical = -1;
					
				}

				//This Vector3 is used for the charge attack
				attackDirection = new Vector3 (Horizontal, 0, Vertical);
				startCharge = true;

			}

			if (startCharge == true) {


				myChargeAttack.SetActive(true); //This is a prefab instance which must be assigned in the editor
				moveDirection = transform.TransformDirection(attackDirection).normalized;

				StartCoroutine("chargeTime");

			} 

		} 
	}

	IEnumerator reload () {
		
		yield return new WaitForSeconds (attackSpeed);
		nextAttack = true;
	}

	IEnumerator chargeTime () {
		
		yield return new WaitForSeconds (chargeTimer);
		charging = false;
		myChargeAttack.SetActive(false); //This is a prefab instance which must be assigned in the editor
		nextAttack = true;
		startCharge = false;
	}
}