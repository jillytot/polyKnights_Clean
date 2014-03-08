using UnityEngine;
using System.Collections;

public class enemyFormations : MonoBehaviour {

	public Transform WalkerPos;
	public float distanceMag = 10;
	bool showEnemies = false;

	// Used to spawn groups of enemies.
	//at the start, disable all the children so they don't all load into the game at once.
	void Start () {

		foreach (Transform child in transform) {

			child.gameObject.SetActive(false);

		}

		showEnemies = false;
	}
	
	// Update is called once per frame
	void Update () {

		//once the enemy group is close enough to the screen, enable them.
		var offsetToWalker = this.gameObject.transform.position.z - WalkerPos.position.z;

		if (offsetToWalker < distanceMag && showEnemies == false) {

			showEnemies = true;

			Debug.Log("Display Enemeis");
			foreach (Transform child in transform)
				
			{

				child.gameObject.SetActive(true);

			}
		}
	}
}
