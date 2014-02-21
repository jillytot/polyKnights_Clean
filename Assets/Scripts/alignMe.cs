using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//This script runs in the editor, and helps me do useful things.
[ExecuteInEditMode () ]
public class alignMe : MonoBehaviour {
	
	public static float tileSize = 2;


	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//When enabled, objects wiill snap to the nearest integer when being moved around. 
		if   (Selection.activeTransform) {

			foreach(var mySelection in Selection.gameObjects) {

				//Debug.Log("I am a selected transform..." + gameObject.transform.position.ToString());

				float snapX = Mathf.Round (mySelection.transform.position.x);
				float snapY = Mathf.Round (mySelection.transform.position.y) * 0.66666f;
				float snapZ = Mathf.Round (mySelection.transform.position.z);


				//Debug.Log (snapX);
				//Debug.Log (snapY);
				//Debug.Log (snapZ);

				mySelection.transform.position = new Vector3(snapX, snapY, snapZ);

			}
		}
	}
}

#endif
