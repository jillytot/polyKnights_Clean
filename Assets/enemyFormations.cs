using UnityEngine;
using System.Collections;

public class enemyFormations : MonoBehaviour {

	public Transform WalkerPos;
	public float distanceMag = 10;
	bool showEnemies = false;

	// Use this for initialization
	void Start () {

		foreach (Transform child in transform) {

			child.gameObject.SetActive(false);

		}

		showEnemies = false;
	}
	
	// Update is called once per frame
	void Update () {

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
