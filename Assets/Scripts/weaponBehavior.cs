using UnityEngine;
using System.Collections;

public class weaponBehavior : MonoBehaviour {

	public int weaponDamage = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other) {

		var doDamage = other.GetComponent<baddie>();

		if (doDamage) {

			//Destroy(doDamage.gameObject);
			print ("enemyHit");
			doDamage.HP -= weaponDamage;
			doDamage.imHit = true;

		}
	}
}
