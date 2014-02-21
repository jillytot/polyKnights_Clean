using UnityEngine;
using System.Collections;

public class musicManager : MonoBehaviour {

	public AudioClip[] mySongs;
	AudioSource playSong;
	bool newSong;

	void Awake () {

		DontDestroyOnLoad(this.gameObject);
		playSong = this.gameObject.GetComponent<AudioSource>();
		newSong = true;


	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Application.loadedLevelName == "testLevel" && newSong == true) {

			Debug.Log("Play a new song yo!");
			playSong.clip = mySongs[1];
			playSong.Play();
			playSong.volume = 0.5f;
			newSong = false;

		}
	}
}
