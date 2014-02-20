using UnityEngine;
using System.Collections;

public class baddie : MonoBehaviour {

	public enum enemyType { //possible basic enemy types 
		
		DEFAULT,
		SHOOTER,
		
	}

	public enemyType thisEnemyType; //enum for this enemy object

	//Basic Enemy Stats

	public int maxHP = 3; //enemies max total HP
	public int HP; //Enemies starting HP
	public float movementSpeed = 1; //speed of enemy
	public int attackPower = 1; //strength of attacks

	//Material / Mesh control

	public GameObject enemyMat; //access the gameobject that contains the enemy material
	public Material hitMat; // this is the material used for when the enemy takes damage
	public Material attackMat; //switch material to this while attacking
	Material storeMat; //used to store the default enemy material

	//variables that manage taking damage

	public bool imHit = false; //Let's the enemy knows it's been hit so it can act accordingly
	public float flashTime; //sets the time for the material swap to last for when the enemy gets hit

	//Variables for finding targets


	Vector3 storePlayerPos; //position of player(s)
	
	GameObject walkerObjects; //Eventually this might need to be an array of objects, but from here we can get any component we need from the Walker;
	Vector3 storeWalkerPos; //The vector3 position of the walker(s)
	bool walkerSafe; //If the walker is targetable or not. 
	saveMe checkWalker; //reference to the walker object(s) for checking variables.

	Vector3 targetPosition; //current target of the enemy
	Vector3 groundTarget; //target position adjusted for ground unit

	//Specific enemy Behavior

	bool attackReady; //returns true when enemy is ready to attack

	public bool moveOnGround; //If the enemy is a walker enemy

	//variables for smoothing enemy movement
	private float smoothX = 0.0f;
	private float smoothZ = 0.0f;
	public float smoothTime = 0.3f;
	
	//Shooter specific behavior
	public bool shooter; //Enables this enemy to shoot arrows
	bool targetLocked; //If shooter has target
	Vector3 myTarget; //Location of target
	public float attackRate = 1.0f; // Frequency of attack rate
	bool reloadArrow; //triggers relaoding of the next round
	public GameObject myArrow; //the object which the shooter fires
	public float arrowSpeed = 10.0f; //Speed at the projectile goes at
	public float maxTargetRange = 50.0f; //maximum distance the projectile will fire.

	//basic melee variables

	public bool meleeEnabled; //toggle on if you want the enemy to do a melee strike (requires groundMovement to be on)

	public float attackRange = 6.0f; //Range at which the enemy will initiate an attack, this should always be a larger number than hold distance.
	public float holdDistance = 1.0f; //distance the enemy will keep from the target
	Vector3 holdPosition; //position of the enemy at holdDistance from the target
	Vector3 offsetToTarget; //difference in position between enemy and target
	Vector3 chargeTarget; //direction of attack


	bool attacking; //used to disable other movement while attacking
	bool attackDone; //returns true when attack is finished
	bool startAttacking;

	//new targetting stuff;
	public static Vector3[] playerPositions;
	float[] myTargetDistances;
	int selectTarget;
	bool refreshTarget;
	public float refreshTargetRate = 1;
	Transform lastTarget;

	bool triggerRefresh;

	
	void Awake () {

		//find Walker game objects
	//TODO move this reference to gameMaster as well.
		walkerObjects = GameObject.FindWithTag("Walker"); 

		}

	// Use this for initialization
	void Start () {

		playerPositions = new Vector3[gameMaster.getPlayers.Length]; //used to for calculating enemy targets
		myTargetDistances = new float[gameMaster.getPlayers.Length]; //distance to each target

		HP = maxHP; //initialize HP to start at Max HP
		storeMat = enemyMat.renderer.material; //Store the default enemy mate for later
		targetLocked = false;
		attackReady = true;
		attacking = false;
		reloadArrow = false;
		attackDone = false;
		startAttacking = false;
		refreshTarget = true;
		triggerRefresh = false;

		//Store reference to walkerScript
		if (walkerObjects) {

		checkWalker = walkerObjects.GetComponent<saveMe>();
		
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (walkerObjects ) {

		storeWalkerPos = walkerObjects.transform.position;

		}

		//if enemy requires a target search, go into the findTarget method
		if (refreshTarget == true ) {

			refreshTarget = false;
			lastTarget = findTarget(transform.position);

		}

		//current target is the most recently assigned target
		storePlayerPos = lastTarget.transform.position;


		//kill this game object if it's dead, you know, out of HPs
		if (HP <= 0) {

			Destroy(this.gameObject);

		}

		//If i've been hit, do this:
		//TODO: change this to a function which the player access with get component

		if (imHit == true) {

			//Change the enemy material to indicate that it's been hit
			enemyMat.renderer.material = hitMat;
			StartCoroutine("flashTimer"); //resets imHIt bool after certain time

		} else if (attacking == true) {
		
			enemyMat.renderer.material = attackMat;

		} else {

			//Return to default material if i'm not hit;
			enemyMat.renderer.material = storeMat;

		}

	if (moveOnGround == true) {

			basicBaddieBehavior ();

		}

		if (shooter == true) {

			shooterBehavior ();

		}
	}

	//counts down by flashTime then returns false for imhit
	IEnumerator flashTimer () {
		
		yield return new WaitForSeconds (flashTime);
		imHit = false;

	}

	//Basic enemy behavior
	void basicBaddieBehavior () { 



		//check walker status to determine target, if walker is not safe, then target walker
		if (checkWalker && checkWalker.safe == false) {

			targetPosition = storeWalkerPos;


			//Debug.Log("Enemy moving towards Walker at: " +  targetPosition);

		} else {

			targetPosition = storePlayerPos;
			//targetPosition = storePlayerPos;
			//Debug.Log("Enemy moving towards Player at: " +  targetPosition);

		}

		if (meleeEnabled == true) {

			basicMelee ();

			groundTarget = new Vector3 (holdPosition.x, this.gameObject.transform.position.y, holdPosition.z);
			
		} else {

			groundTarget = new Vector3 (targetPosition.x, this.gameObject.transform.position.y, targetPosition.z);

			}

		//take x and z positions and smooth them out relative to the enemies current posotion (This will help stop the critter jitters)

		if (attacking == true) {

		} 

		else {

		//Debug.Log("normal movement is happening");
		float newXPos = Mathf.SmoothDamp(transform.position.x, groundTarget.x, ref smoothX, smoothTime);
		float newZPos = Mathf.SmoothDamp(transform.position.z, groundTarget.z, ref smoothZ, smoothTime);

		groundTarget = new Vector3 (newXPos, groundTarget.y, newZPos);

		transform.position = Vector3.MoveTowards(transform.position, groundTarget, movementSpeed * Time.deltaTime);
		
		}
	}

	void shooterBehavior () {

		//check walker status to determine target, if walker is not safe, then target walker
		if (checkWalker && checkWalker.safe == false) {
			
			myTarget = storeWalkerPos + Vector3.up;
			//Debug.Log("Enemy moving towards Walker at: " +  targetPosition);
			
		} else {

			
			myTarget = storePlayerPos + Vector3.up;
			//Debug.Log("Enemy moving towards Player at: " +  targetPosition);
			
		}

		var targettingRange = maxTargetRange * maxTargetRange;
		//myTarget = storePlayerPos + Vector3.up;
		var offsetToShootTarget = transform.position - myTarget;

		if (offsetToShootTarget.sqrMagnitude < targettingRange) {

		if (attackReady == true) {

			attackReady = false; 

			if (targetLocked == false) {

				targetLocked = true;
			}

			if (targetLocked == true) {

				//Debug.Log("Target at: " + myTarget);
				targetLocked = false;
			}
		} 

		if (attackReady == false && reloadArrow == false) {

			//Debug.Log("Commence Firing!");
			reloadArrow = true;
			StartCoroutine("refreshAttack");
		
			var arrow = (GameObject)Instantiate(myArrow, transform.position, Quaternion.identity);
			var arrowComponent = arrow.GetComponent<arrowBehavior>();
			arrowComponent.ShootSelf(myTarget, arrowSpeed );

			}
		}
	}

	IEnumerator refreshAttack () {

		yield return new WaitForSeconds (Random.Range(attackRate * 0.9f, attackRate));
		attackReady = true;
		refreshTarget = true;

		if (shooter == true) {

		reloadArrow = false;

		}
	}

	IEnumerator chargeAttackTimer () {

		yield return new WaitForSeconds(1);

		attackDone = true;
		refreshTarget = true;

	}

	IEnumerator chargeAttackTiming () {

		yield return new WaitForSeconds(Random.Range(attackRate * 0.5f, attackRate));

		attackReady = false;


	}

	void basicMelee () {

		offsetToTarget =  this.gameObject.transform.position - targetPosition;
	

		if (offsetToTarget.sqrMagnitude < holdDistance * holdDistance) {

			holdPosition = transform.position + offsetToTarget;

		} else {

			holdPosition = targetPosition;

		}



		//Do a charge attack
		if (offsetToTarget.sqrMagnitude < attackRange * attackRange) {

			if (attackReady == true && startAttacking == false) {
				
				//randomize & spread out attack rate
				StartCoroutine("chargeAttackTiming");
				startAttacking = true;

			}


			if (attacking == false && attackReady == false) {

				//calculate the target position to charge to
				chargeTarget = targetPosition - this.gameObject.transform.position;
				chargeTarget = new Vector3(chargeTarget.x, 0, chargeTarget.z);
				chargeTarget = chargeTarget.normalized;
				attacking = true; 

				//attack will last for this long
				StartCoroutine("chargeAttackTimer");

			}

			if (attacking == true) {

				//Get charge speed, and charge through target
				var chargeSpeed = movementSpeed * movementSpeed;
				transform.position = Vector3.MoveTowards(transform.position, transform.position + chargeTarget, chargeSpeed * Time.deltaTime);
		
			}

		} else if (attackDone == true) {

			//Once the attack is over, reset all the parameters.
			attacking = false;
			attackDone = false;
			attackReady = true;
			startAttacking = false;
			
		} 

		//If not attacking, peridoically update your target.
		if (triggerRefresh == false) {

			triggerRefresh = true;
			StartCoroutine("refreshTargetBool");
		}
	}

	void OnTriggerEnter (Collider other) {

		//only damage the walker if not safe
		if (walkerObjects && checkWalker.safe == false) {

		var hitWalker = other.GetComponent<saveMe>();

			//for Melee
			if (hitWalker && attacking == true) {

				hitWalker.takeDamage (attackPower);

			}
		}

		var hitPlayer = other.GetComponent<playerMovement>();

		//for melee
		if (hitPlayer && attacking == true) {

			//Debug.Log("enemy hit to player");
			hitPlayer.takeDamage(attackPower);

		}
	}

	//returns transform of closest target to enemy
	Transform findTarget (Vector3 myPos) {

		//default value for target
		var closestIndex = -1;
		
		for (int i = 0; i < gameMaster.getPlayers.Length; i++) {

			if (gameMaster.getDamage[i].myHP < 1) {

				//This is super useful
				continue;

			}
			
			if (closestIndex == -1) {

				closestIndex = i;
		
			}

			//get the position of each player
			 playerPositions[i] = gameMaster.playerTransforms[i].transform.position;
			
			//Debug.Log("Player: " + (i + 1) + " is located at: " + playerPositions[i]);

			//find the distance to each player from this enemy
			var distanceOffset = myPos - playerPositions[i];
			myTargetDistances[i] = distanceOffset.sqrMagnitude;

			
			//check targets to see if their dead... 
			if (i > 0 &&   myTargetDistances[i] < myTargetDistances[closestIndex]) {

				//comare distances to targets

				//if the distance to the current position is shorter than the previous position, target is current position.
				closestIndex = i;
				//Debug.Log("Update target to: " + playerPositions[selectTarget] + "from previous target: " + playerPositions[i - 1]);
					
				}
			}

		if (closestIndex >= 0) {

		selectTarget = closestIndex;

		}

		//This will probably still need further refinment
		var chosenTarget = gameMaster.playerTransforms[selectTarget];
		//print ("I CHOOSE YOU " + chosenTarget);
		
		return chosenTarget; 
		
	}

	IEnumerator refreshTargetBool () {

		yield return new WaitForSeconds(Random.Range( refreshTargetRate *0.75f, refreshTargetRate * 1.25f) );

		if (attacking == false) {

			refreshTarget = true;
		}

		triggerRefresh = false;

	}
}
