using UnityEngine;
using System.Collections.Generic;
using Gui;

// Takes care of the functionality on the player selection screen.
namespace PlayerSelection {
	public class PlayerSelection : MonoBehaviour {
		public Material disabledMaterial;
		public Color blinkingColor;
		public int countdownInSeconds;
		public float fadeTime;
		public float timeBeforeFade;

		IList<Player> players;
		HashSet<PlayerInput> inputsInUse = new HashSet<PlayerInput>();
		CountdownTimer timer;
		bool playersCanJoin = true;
		bool gameIsStarting;
		float gameIsStartingTime;
		int numberOfPlayersDuringCountdown;
		ColorFader fader;

		void Start() {
			CreatePlayers(disabledMaterial);
			timer = new CountdownTimer(countdownInSeconds);

			GameObjectFunctions.Find("player1", "ready").GetComponent<TextMesh>().text = "Start game";
			fader = ColorFader.Create(GameObject.Find("camera").GetComponent<Camera>());
		}

		void Update() {
			if(gameIsStarting) {
				var doneFading = Fading();

				if(doneFading) {
					PreparePlayerDescriptions();
					GotoNextScreen();
				}

				return;
			}

			CheckStartButtons();

			UpdatePlayers();
			UpdateBlinkingText();

			if(AllPlayersHaveJoined())
				HideJoinMessage();

			if(TimeToStartGame()) {
				gameIsStarting = true;
				gameIsStartingTime = Time.time;
				playersCanJoin = false;

				MakeEveryoneReady();
				HideJoinMessage();
				HideTimer();

				return;
			}

			if(TimeForCountdown()) {
				// Start the countdown
				if(!timer.Started) {
					timer.Start();
					numberOfPlayersDuringCountdown = NumberOfJoinedPlayers();
				}
				// Reset the new countdown when another player joins
				else if(NumberOfJoinedPlayers() > numberOfPlayersDuringCountdown) {
					numberOfPlayersDuringCountdown = NumberOfJoinedPlayers();
					timer.Reset();
				}

				UpdateTimerText();
			}
		}

		void CreatePlayers(Material disabledMaterial) {
			players = new List<Player>();

			for(int i = 0; i < 8; i++)
				players.Add(new Player(GameObject.Find("player" + (i + 1)), disabledMaterial));
		}

		bool Fading() {
			float timeSinceStarting = Time.time - gameIsStartingTime;
			
			if(timeSinceStarting >= timeBeforeFade && !fader.Fading)
				fader.BeginFade(fadeTime);
			
			if(timeSinceStarting >= timeBeforeFade + fadeTime)
				return true;
			else
				return false;
		}

		void GotoNextScreen() {
			GameObject.DontDestroyOnLoad(GameObject.Find("playerColors"));
			Application.LoadLevel("playerSpawn");
		}

		void PreparePlayerDescriptions() {
			int nPlayers = NumberOfJoinedPlayers();
			var descriptions = new PlayerDescription[nPlayers];

			for(int i = 0; i < nPlayers; i++) {
				var player = players[i];
				var description = new PlayerDescription();

				description.playerColorIndex = player.ColorIndex;
				description.playerClass = (playerClass)player.PlayerClass;

				descriptions[i] = description;
			}

			var playerDescriptionsGameObject = GameObject.Find("playerDescriptions");
			GameObject.DontDestroyOnLoad(playerDescriptionsGameObject);
			var playerDescriptions = playerDescriptionsGameObject.GetComponent<PlayerDescriptions>();
			playerDescriptions.descriptions = descriptions;
		}

		void CheckStartButtons() {
			PlayerInputs inputs = GameObject.Find("playerInputs").GetComponent<PlayerInputs>();

			foreach(PlayerInput input in inputs) {
				if(!inputsInUse.Contains(input) && input.GetPressed(PlayerInput.Start))
					AddPlayer(input);
			}
		}

		void AddPlayer(PlayerInput input) {
			if(playersCanJoin) {
				var player = players[inputsInUse.Count];

				if(!player.Joined) {
					player.Join(input);
					inputsInUse.Add(input);
				}
			}
		}

		void UpdatePlayers() {
			foreach(var player in players)
				player.Update();
		}

		void UpdateBlinkingText() {
			var baseColor = GameObjectFunctions.Find("pressStartToJoin", "press").GetComponent<TextMesh>().color;
			GameObjectFunctions.Find("pressStartToJoin", "start").GetComponent<TextMesh>().color = Color.Lerp(baseColor, blinkingColor, Mathf.Abs(Mathf.Sin(Time.time * 3)));
		}

		int NumberOfJoinedPlayers() {
			int numberOfJoinedPlayers = 0;

			foreach(var player in players)
				if(player.Joined)
					numberOfJoinedPlayers++;

			return numberOfJoinedPlayers;
		}

		bool AllPlayersHaveJoined() {
			return NumberOfJoinedPlayers() == 8;
		}

		bool TimeForCountdown() {
			if(gameIsStarting)
				return false;

			if(players[0].Ready && !EveryoneIsReady())
				return true;

			return false;
		}

		int NumberOfReadyPlayers() {
			int numberOfReadyPlayers = 0;

			foreach(var player in players)
				if(player.Ready)
					numberOfReadyPlayers++;
			
			return numberOfReadyPlayers;
		}

		void HideJoinMessage() {
			GameObject.Find("pressStartToJoin").transform.position = new Vector3(0, 0, -1000);
		}

		void HideTimer() {
			GameObject.Find("timer").GetComponent<MeshRenderer>().enabled = false;
		}

		void UpdateTimerText() {
			var time = timer.GetTimeInt();
			string timerString = "";

			if(time > 0)
				timerString = time.ToString();

			GameObject.Find("timer").GetComponent<TextMesh>().text = timerString;
		}

		bool TimeToStartGame() {
			if(EveryoneIsReady()) // This includes the case where player 1 is the only player.
				return true;

			// Start the game if the timer has run out
			if(timer.Started && timer.GetTimeInt() == 0)
				return true;

			return false;
		}

		bool EveryoneIsReady() {
			return NumberOfReadyPlayers() == NumberOfJoinedPlayers() && NumberOfJoinedPlayers() != 0;
		}

		void MakeEveryoneReady() {
			foreach(var player in players)
				if(!player.Ready)
					player.MakeReady();
		}
	}
}

// The rest of these classes probably need to be moved to different files
// but for now they reside here.

class PlayerToMenuInput : MenuInput {
	PlayerInput input;
	
	public PlayerToMenuInput(PlayerInput input) {
		this.input = input;
	}
	
	public bool GetButton(MenuButton button) {
		return input.GetPressed(MenuButtonToPlayerControl(button));
	}
	
	int MenuButtonToPlayerControl(MenuButton button) {
		switch(button) {
		case MenuButton.Left:
			return PlayerInput.Left;
			
		case MenuButton.Right:
			return PlayerInput.Right;
			
		case MenuButton.Up:
			return PlayerInput.Up;
			
		case MenuButton.Down:
			return PlayerInput.Down;
			
		case MenuButton.Enter:
			return PlayerInput.Fire1;
			
		case MenuButton.Back:
			return PlayerInput.Fire2;
			
		default:
			AyloDebug.Assert(false);
			return PlayerInput.Left;
		}
	}
}

public class NumberFunctions {
	public static void WrapAround(ref int i, int n) {
		i %= n;

		if(i < 0)
			i += n;
	}
}