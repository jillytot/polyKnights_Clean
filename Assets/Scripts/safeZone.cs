using UnityEngine;
using System.Collections;

public class safeZone : MonoBehaviour {

	//These stats control the timed burn rate of the torch
	public static float torchPower = 10;
	public static float burnAmount = 0.99f;
	public static float burnRate = 0.1f;

	//triggers when the torch loses power over time
	bool decrementPower;

	//for controlling the size of the safeZone based off Torch Power
	Vector3 initialScale;
	public float torchScalar = 0.1f;
	public float scalingSpeed = 1.0f;

	//The max and minimum size of the torch safeZone
	public float zoneMax = 20;
	public float zoneMin = 7.5f;

	public GameObject[] protectSphere;
	public static bool disableProtection;
	public float disableMin = 5.0f;

	// Use this for initialization
	void Start () {

		decrementPower = true;
		disableProtection = false;
		initialScale = transform.localScale;

		//protectSphere = new GameObject[protectSphere.Length];
	
	}
	
	// Update is called once per frame
	void Update () {

		if (decrementPower == true) {

			decrementPower = false;
			StartCoroutine("burnTorch");
		}

		//the safezone has a min and max size, and grows and shrinks based on the torch power
		if (torchPower > zoneMin && torchPower < zoneMax) {

			Vector3 rescale = new Vector3(initialScale.x * torchPower * torchScalar, initialScale.y * torchPower * torchScalar, initialScale.z * torchPower * torchScalar);
			transform.localScale = Vector3.Slerp(transform.localScale, rescale, scalingSpeed * Time.deltaTime);
	
		}

		//If the torch goes below a certain power threshold, then the safety zone should be disabled. 
		//tIf the safety is off, then the torch walker will become vunerable. 
		if (torchPower < disableMin && disableProtection == false) {

		    disableProtection = true;

			for (int i = 0; i < protectSphere.Length; i ++) {

				protectSphere[i].SetActive(false);

			}
		}

			if (torchPower > disableMin && disableProtection == true) {

			for (int i = 0; i < protectSphere.Length; i ++) {

				disableProtection = false;
				protectSphere[i].SetActive(true);
				
			}
		}
	}

	IEnumerator burnTorch () {

		torchPower *= burnAmount;
		yield return new WaitForSeconds(burnRate);
		decrementPower = true;

	}
}
