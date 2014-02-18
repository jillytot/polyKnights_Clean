using UnityEngine;
using System.Collections;

public class displayPlayerStats : MonoBehaviour {

	string myString;
	public string showName;
	int playerHP = 6;
	int playerMaxHP;
	public playerNum displayNum;
	TextMesh displayText;
	int statIndex;

	bool displayNothing;

	void Awake () {

		displayText = this.gameObject.GetComponent<TextMesh>();

	}

	// Use this for initialization
	void Start () {

		displayNothing = true;


		for (int i = 0; i < gameMaster.playerNames.Length; i ++) {

			Debug.Log("GameMaster Player HP = " + gameMaster.playerHP[i]);

			if (displayNum == gameMaster.playerNames[i]) {

				playerHP =  gameMaster.getDamage[i].myHP;
				playerMaxHP = gameMaster.getDamage[i].myMaxHp;
				statIndex = i;

			} 

			if (displayNum == gameMaster.playerNames[statIndex]) {

				displayNothing = false;

			}
		}

		displayText.text  = displayNum + " " + playerHP + "/" + playerMaxHP + "HP";


	}
	
	// Update is called once per frame
	void Update () {

		if (displayNothing == true) {

			displayText.text = "";

		} else { 

		playerHP = gameMaster.playerHP[statIndex];
		playerMaxHP = gameMaster.playerMaxHP[statIndex];
		displayText.text  = showName + " " + playerHP + "/" + playerMaxHP + "hp";

		}
	}
}
