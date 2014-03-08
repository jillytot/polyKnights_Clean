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

	//The max and minimum size of the torch safeZone
	public float zoneMax = 20;
	public float zoneMin = 7.5f;

	public GameObject[] protectSphere;
	public static bool disableProtection;

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

		//stop the torch area from shrinking beyond a certain threshold.
		if (torchPower > zoneMin && torchPower < zoneMax) {

			//TODO: lerp to new size
			transform.localScale = new Vector3(initialScale.x * torchPower * torchScalar, initialScale.y * torchPower * torchScalar, initialScale.z * torchPower * torchScalar);
	
		}

		//If the torch goes below a certain power threshold, then the safety zone should be disabled. 
		//tIf the safety is off, then the torch walker will become vunerable. 
		if (torchPower < zoneMin && disableProtection == false) {

		    disableProtection = true;

			for (int i = 0; i < protectSphere.Length; i ++) {

				protectSphere[i].SetActive(false);

			}
		}

			if (torchPower > zoneMin && disableProtection == true) {

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
