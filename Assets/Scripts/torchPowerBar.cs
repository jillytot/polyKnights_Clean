using UnityEngine;
using System.Collections;

public class torchPowerBar : MonoBehaviour {

	Vector3 initialScale;

	// Use this for initialization
	void Start () {

		initialScale = transform.localScale;

	
	}
	
	// Update is called once per frame
	void Update () {

		var torchPower = safeZone.torchPower;
		var rescale = initialScale.x * torchPower * 0.1f;
		transform.localScale = new Vector3(rescale, initialScale.y, initialScale.z);
	
	}
}
