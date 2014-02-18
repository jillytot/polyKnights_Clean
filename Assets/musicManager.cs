using UnityEngine;
using System.Collections;

public class musicManager : MonoBehaviour {

	public AudioClip[] mySongs;
	AudioSource playSong;

	void Awake () {

		DontDestroyOnLoad(this.gameObject);
		playSong = this.gameObject.GetComponent<AudioSource>();


	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Application.loadedLevelName == "2p_Test") {

			Debug.Log("Play a new song yo!");

		}
	}
}
