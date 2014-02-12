using UnityEngine;
using System.Collections;

public class killMeWhen : MonoBehaviour {

	//Destory this game object in the allotted time.
	public float deathClock = 1.0f;

	// Use this for initialization
	void Start () {

		StartCoroutine(destroyOnTimer());
	
	}


	IEnumerator destroyOnTimer () {
		yield return new WaitForSeconds(deathClock);
		//print("Im dead");
		Destroy(this.gameObject);
	}
}
