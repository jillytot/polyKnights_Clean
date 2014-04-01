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

	bool charging = false; //Triggers Charging
	public float chargeSpeed = 100.00f; //Rate at which you charge
	Vector3 attackDirection; //Direction you are facing when you use charge attack
	public float chargeTimer = 0.5f; //How long you use your charge attack for
	bool startCharge = false;

	Quaternion myRotation; //used to store direction of movement
	float Horizontal; //raw value for Horizontal axis
	float Vertical; //raw value for Vertical axis
	
	private Vector3 moveDirection = Vector3.zero; //initialize movement direction
	private Vector3 inputMagnitude; //store axis input
	private Vector3 lastMoveDirection; //record last movement.
	public Vector3 playerPos;

	Animator myAnimation; // Animation controller

	bool triggerDeath; //Starts the dying process
	bool healActive; //while true, the player is being healed.
	bool triggerHeal; //starts the healing process

	CharacterController controller; //create instance of character controller

	public playerNum thisPlayer;
	
	AudioSource myAudio;
	public AudioClip[] mySounds;


	//initialize controls for the player
	Controls controls;
	string myHorizontal = "Horizontal";
	string myVertical = "Vertical";
	string myFire1 = "Fire1";
	string myFire2 = "Fire2";
	string myFire3 = "Fire3";
	string myJump = "Jump";

	public int reviveCounter;
	bool reviveDelay;



	void Awake() {

		myAnimation = GetComponentInChildren<Animator>(); //Get animation controller and assign it to this character
		controller = GetComponent<CharacterController>();
		myRotation = transform.rotation;
		triggerDeath = false;
		myAudio = this.gameObject.GetComponent<AudioSource>();
		reviveDelay = false;
		healActive = false;
		triggerHeal = false;


	}

	void Start() {

		if (gameMaster.multiplayer == true) {

			getMyControls();
		
		}
	}
	
	void Update() {

		//Get axis values for calculating movement
		Horizontal = Input.GetAxis(myHorizontal);
		Vertical = Input.GetAxis(myVertical);

		inputMagnitude =  new Vector3(Horizontal, 0, Vertical);

		if (imDead == true) {

			playerDeath();


		} else {

		if (inputMagnitude.sqrMagnitude != 0.0f) {

			lastMoveDirection = inputMagnitude;

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
		if (Input.GetButtonDown(myJump)) {
			
			moveDirection.y = jumpSpeed;
					myAudio.PlayOneShot(mySounds[0]);

			
			}

		//Air based movement
		} else {

			//Air Control
			//TODO: This is messy, it needs to be cleaned up.
			moveDirection.x = Horizontal;
			moveDirection.z = Vertical;

			Vector3 normalizeXZ = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
			normalizeXZ *= newSpeed;
			Vector3 moveInAirDirection = new Vector3(normalizeXZ.x, moveDirection.y, normalizeXZ.z);

			moveDirection = transform.TransformDirection(moveInAirDirection);

			foreach (Transform child in transform)
				
			{
				
				if (moveDirection.sqrMagnitude > 0.5f) { 
					
					myAnimation.SetBool("Run", true); //Changes avatar to running state
					Vector3 lookatMoveDirection = new Vector3(	moveDirection.x, 0, moveDirection.z);

				
						if (inputMagnitude.sqrMagnitude > 0.5f) {

					var targetRotation = Quaternion.LookRotation(lookatMoveDirection); //set target towards direction of motion
					child.rotation = child.rotation.EaseTowards(targetRotation, turnSpeed); //rotate towards the direction of motion
					myRotation = child.rotation;

						}
					
				}  else {
					
					myAnimation.SetBool ("Run", false); 

					
					}
				}
			}

			if (imDead == false && triggerDeath == false) {

				healingTime();

			}


		//Controls Gravity
		moveDirection.y -= gravity * Time.deltaTime;

		if (charging == false) {

		controller.Move(moveDirection * Time.deltaTime);

		} else {

			controller.Move(attackDirection * chargeSpeed * Time.deltaTime);

			}
		}

		playerPos = this.gameObject.transform.position;


	}

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

		if (Input.GetButtonDown(myFire1) && nextAttack == true) {
			
			//print ("attack that shit!");
			
			//Enable the attack graphic & the corresponding attack animation.

			myAudio.PlayOneShot(mySounds[1]);

			//Debug.Log("Enable Attack");
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

		if (Input.GetButtonDown(myFire2) && nextAttack == true) {

			//Get facing direction, and charge forward quickly. 

			//bool startCharge = false; 
			charging = true;

			if (startCharge == false) {
				myAudio.PlayOneShot(mySounds[2]);

				if (inputMagnitude.sqrMagnitude == 0) {

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
				if (attackDirection.sqrMagnitude == 0) {

					attackDirection = Vector3.forward;

				}

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

	void blockingGrounded () {

		//lock player rotation, slow down movement, resist attacks from direction you are blocking in
		//play blocking animation

	}

	public void getMyControls () {

		controls = playerControls.getControls(thisPlayer);
		myHorizontal = controls.horizontal;
		myVertical = controls.vertical;
		myFire1 = controls.fire1;
		myFire2 = controls.fire2;
		myFire3 = controls.fire3;
		myJump = controls.jump;

	}

	void playerDeath () {

		//trigger death animation when you die...
		if (triggerDeath == false) {

			//myAnimation.SetBool("imDead", true);

			myAnimation.Play("imDead");
			//cancel all attacks
			myAttack.SetActive(false);
			myChargeAttack.SetActive(false);
			nextAttack = true;
			startCharge = false;
			triggerDeath = true;

		}

		//make the axis input zero so the player can't move. 
		Horizontal = 0;
		Vertical = 0;

	}

	//Current revive player behavior: Living player hits a dead player with an attack multple times to revive them to full Health. 
	//This is obviously temp...
	public void reviveMe () {

		//every time the player gits hit, wait a little bit before before they can get hit again
		if (reviveDelay == false) {

			//decrement the revive counter each time a hit registers. 
			reviveCounter -= 1;
			myMat.renderer.material = healMat;  //flash the heal material to indicate the player is being healed
			reviveDelay = true;
			StartCoroutine("reviveMat"); //this counter resets the reviveDelay bool so the player can get hit again

		}

		//if the revive counter counts down all the way, revive the player.
		if (reviveCounter < 1) {

			myHP = myMaxHp;
			imDead = false;
			triggerDeath = false;
			reviveCounter = 10;
			myAnimation.Play("idle");

		}
	}

	//This resets values for the reviveMe function
	IEnumerator reviveMat () { 
		
		yield return new WaitForSeconds(0.1f);
		reviveDelay = false;
		myMat.renderer.material = storeMat; //returns the player material to the normal material
		Debug.Log("Time to get hit again!");
		
	}

	void OnTriggerEnter (Collider other) {

		var safeZone = other.collider.GetComponent<safeZone>();

		//check to see if the safe zone is present and active
		if (safeZone && safeZone.disableProtection == false) {

			healActive = true;
			Debug.Log("I am now in the safe zone");

		}
	
	}

	void OnTriggerExit (Collider other) {

		var notSafe = other.collider.GetComponent<safeZone>();

		if (notSafe) {

			healActive = false;
			Debug.Log("I am not safe anymore!");
			healingEffect.SetActive(false);

		}

	}

	void healingTime () {

		if (healActive == true) {

			healingEffect.SetActive(true);

		}

		if (healActive == true && triggerHeal == false) {

			triggerHeal = true;
			StartCoroutine("healing");

		}

		if (safeZone.disableProtection == true) {

			healActive = false;
			Debug.Log("I am not safe anymore!");
			healingEffect.SetActive(false);
		}
	}

	IEnumerator healing () {



		myHP += saveMe.healAmount;
	

		if (myHP > myMaxHp) {

			myHP = myMaxHp;

		} else {

			//If the torch flame is healing me, substract more energy from the torch
			var healPenalty = safeZone.torchPower * saveMe.healAmount * 0.1f;
			safeZone.torchPower -= healPenalty;

		}

		yield return new WaitForSeconds(1);
		triggerHeal = false;
	}


	
}