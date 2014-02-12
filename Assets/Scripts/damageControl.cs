using UnityEngine;
using System.Collections;

public class damageControl : MonoBehaviour {

	public int myHP;
	public int myMaxHp = 10;
	public float hitRefresh = 1.0f; //Time you are invincible after taking damage
	bool imHit; //returns true when damage has been taken. 

	public GameObject myMat; //access the gameobject that my material
	public Material hitMat; // this is the material used for when i take damage
	public Material storeMat; //used to store my default material
	
	// Use this for initialization
	void Start () {

		//storeMat = myMat.renderer.material; //Store the default enemy mate for later
		myHP = myMaxHp;
		Debug.Log("HP: " + myHP + " / " + myMaxHp);
		imHit = false; 

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void takeDamage (int attack) {

		if (imHit == false) {

			myHP -= attack;
			imHit = true;
			StartCoroutine("hitRefreshTimer");
			Debug.Log("i've been hit!");

			myMat.renderer.material = hitMat;

		}

		if (myHP <= 0) {

			Debug.Log("You are already deeeaaadd!");

		}
	}

	IEnumerator hitRefreshTimer () { 

		yield return new WaitForSeconds(hitRefresh);
		imHit = false;
		myMat.renderer.material = storeMat;
		Debug.Log("Time to get hit again!");

	}
}
