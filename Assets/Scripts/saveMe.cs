using UnityEngine;
using System.Collections;

public class saveMe : damageControl {

	public Vector3 saveMePos; //Position of saveMe character
	public bool safe; //While safe, walker won't be targeted
	public bool triggerSafe; //temp bool for triggering safe
	Vector3 myPos; // stores Walker's position
	Quaternion myRot; //store Walker rotation

	public float mySpeed = 5f; //Walker speed

	// Use this for initialization
	void Awake () {

		safe = false;

	}

	void Start () {

		triggerSafe = false;
	
	}
	
	// Update is called once per frame
	void Update () {



		saveMePos = this.gameObject.transform.position;


		//conditions for turning "safe" off
		if (triggerSafe == true) { 

			//Safe turns off after "n" seconds
			//triggerSafe = false;
			//StartCoroutine("triggerUnsafe");

		}

		if (safe == false) {

			Debug.Log ("Walker is not safe"); 

		}

		print ("is derp happening?");

		//eventually the Walker will need to stop when confronted with an obstical
		transform.position += Vector3.forward * mySpeed * Time.deltaTime;
	
	}

	IEnumerator triggerUnsafe () {
	
		Debug.Log ("You won't be safe for long!!!");
		yield return new WaitForSeconds(5.0f);
		Debug.Log("Safety is off!");
		safe = false;

	}
}
