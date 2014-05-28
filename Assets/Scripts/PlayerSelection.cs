using UnityEngine;
using System.Collections.Generic;
using Gui;

// Takes care of the functionality on the player selection screen.
namespace PlayerSelection {
	public class PlayerSelection : MonoBehaviour {
		public Material disabledMaterial;
		public Color blinkyColor;

		IList<Player> players;

		void Start() {
			CreatePlayers(disabledMaterial);
		}

		void Update() {
			foreach(var player in players)
				player.Update();

			// Blinking stuff
			var baseColor = GameObjectFunctions.GetChild("pressStartToJoin", "press").GetComponent<TextMesh>().color;
			GameObjectFunctions.GetChild("pressStartToJoin", "start").GetComponent<TextMesh>().color = Color.Lerp(baseColor, blinkyColor, Mathf.Abs(Mathf.Sin(Time.time * 3)));
		}

		void CreatePlayers(Material disabledMaterial) {
			PlayerInputs inputs = GameObject.Find("playerInputs").GetComponent<PlayerInputs>();

			players = new List<Player>();

			for(int i = 0; i < 8; i++)
				players.Add(new Player(inputs.GetInput(i), GameObject.Find("player" + (i + 1)), disabledMaterial));
		}
	}

	// Represents the appearance and functionality (including input) of a single player object on the player selection screen.
	//
	// Note: It might be a good idea to seperate the stuff related to graphics into a seperate class.
	class Player : MenuObserver {
		GameObject playerObject;
		Menu menu;
		int menuItemColor, menuItemClass, menuItemReady;
		IDictionary<int, GameObject> menuItems = new Dictionary<int, GameObject>();
		bool enabled;
		int activeMenuItem;
		int currentColorIndex = -1;
		int currentPlayerClass;
		PlayerInput input;

		SkinnedMeshRenderer playerMesh;
		MeshRenderer leftArrow, rightArrow;
		MeshRenderer colorRectangle;
		TextMesh classText;
		TextMesh readyText;
		Material disabledMaterial;

		static CyclicAllocator<PlayerColor> colorAllocator;/**/
		static string[] playerClasses = {"Swordy", "Mage", "Rawrior", "Bunny", "Kardashian", "Dalmatian", "Robot", "Tim Schafer", "Aylo Dev", "Luigi", "Biatch"};/**/

		public Player(PlayerInput input, GameObject playerObject, Material disabledMaterial) {
			this.playerObject = playerObject;
			this.input = input;
			this.disabledMaterial = disabledMaterial;

			if(colorAllocator == null)/**/
				colorAllocator = new CyclicAllocator<PlayerColor>(PlayerColors.GetColors());/**/

			playerMesh = GameObjectFunctions.GetChild(playerObject, "chickFillet", "chickFillet_mesh").GetComponent<SkinnedMeshRenderer>();
			leftArrow = GameObjectFunctions.GetChild(playerObject, "left").GetComponent<MeshRenderer>();
			rightArrow = GameObjectFunctions.GetChild(playerObject, "right").GetComponent<MeshRenderer>();
			colorRectangle = GameObjectFunctions.GetChild(playerObject, "color").GetComponent<MeshRenderer>();
			classText = GameObjectFunctions.GetChild(playerObject, "class").GetComponent<TextMesh>();
			readyText = GameObjectFunctions.GetChild(playerObject, "ready").GetComponent<TextMesh>();

			CreateMenu();
			Enable(false);
		}

		public void Update() {
			if(input.GetPressed(PlayerInput.Start) && !enabled) {
				Enable(true);
				menu.SetActiveItem(0);
			}

			menu.Update();
		}

		// Enables or disables the player.
		public void Enable(bool enable) {
			menu.Enabled = enable;

			if(enable)
				CycleMenuItem(menuItemColor, true);
			else
				playerMesh.material = disabledMaterial;

			Animate(enable);
			leftArrow.enabled = enable;
			rightArrow.enabled = enable;
			colorRectangle.enabled = enable;
			GameObjectFunctions.GetChild(playerObject, "class").GetComponent<MeshRenderer>().enabled = enable;
			GameObjectFunctions.GetChild(playerObject, "ready").GetComponent<MeshRenderer>().enabled = enable;

			enabled = enable;
		}

		void CreateMenu() {
			menu = new Menu(this, new PlayerToMenuInput(input), false);/**/

			menuItemColor = AddMenuItem("color");
			menuItemClass = AddMenuItem("class");
			menuItemReady = AddMenuItem("ready");

			menu.AddCycleItem(menuItemColor, MenuButton.Left, MenuButton.Right);
			menu.AddCycleItem(menuItemClass, MenuButton.Left, MenuButton.Right);
			menu.AddButtonItem(menuItemReady);

			menu.SetItemDirectionItems(menuItemColor, MenuButton.Down, menuItemClass);
			menu.SetItemDirectionItems(menuItemClass, MenuButton.Down, menuItemReady);
			menu.SetItemDirectionItems(menuItemClass, MenuButton.Up, menuItemColor);
			menu.SetItemDirectionItems(menuItemReady, MenuButton.Up, menuItemClass);
		}

		int AddMenuItem(string gameObjectName) {
			int key = menuItems.Count;
			menuItems.Add(menuItems.Count, GameObjectFunctions.GetChild(playerObject, gameObjectName));
			return key;
		}

		public void ActivateMenuItem(int id, bool activate) {
			activeMenuItem = id;
			UpdateArrowPositions();
		}

		public void SelectMenuItem(int id) {
			if(id == menuItemReady) {
				/**/// ready!
			}
		}

		public void CycleMenuItem(int id, bool next) {
			if(id == menuItemColor)
				SetColor(colorAllocator.Get(ref currentColorIndex, next));
			else if(id == menuItemClass)
				CyclePlayerClass(next);
			else
				AyloDebug.Assert(false);
		}

		void CyclePlayerClass(bool next) {
			currentPlayerClass += next ? 1 : -1;
			NumberFunctions.WrapAround(ref currentPlayerClass, playerClasses.Length);

			classText.text = playerClasses[currentPlayerClass];
			UpdateArrowPositions();
		}

		void UpdateArrowPositions() {
			var gapBetweenItemAndArrow = 0.2f;
			var itemBounds = menuItems[activeMenuItem].GetComponent<Renderer>().bounds;
			var centerX = itemBounds.center.x;
			var x = itemBounds.extents.x + gapBetweenItemAndArrow;
			var y = menuItems[activeMenuItem].transform.position.y;

			if(activeMenuItem == 2)
				x = -x;
			
			leftArrow.transform.position = new Vector3(centerX - x, y, 0);
			rightArrow.transform.position = new Vector3(centerX + x, y, 0);
		}

		void Animate(bool animate) {
			var animator = GameObjectFunctions.GetChild(playerObject, "chickFillet").GetComponent<Animator>();
			animator.enabled = animate;
		}

		void SetColor(PlayerColor color) {
			playerMesh.material = color.material;
			colorRectangle.material.color = color.color;
			classText.color = color.color;
			readyText.color = color.color;
		}
	};

	class CyclicAllocator<T> {
		T[] items;
		bool[] itemIsFree;

		public CyclicAllocator(T[] items) {
			this.items = items;
			itemIsFree = new bool[items.Length];

			for(uint i = 0; i < itemIsFree.Length; i++)
				itemIsFree[i] = true;
		}

		// Finds the next free item and allocates it.
		// The item with index currentItem is deallocated.
		// If currenItem equals -1 no item was previously allocated.
		// next - Determines the direction to search
		public T Get(ref int currentItem, bool next) {
			var oldItem = currentItem;
			int lookedAt = 0;
			bool foundFreeItem = false;
			
			if(currentItem == -1)
				currentItem = 0;
			
			while(lookedAt < items.Length) {
				NumberFunctions.WrapAround(ref currentItem, items.Length);

				if(itemIsFree[currentItem]) {
					itemIsFree[currentItem] = false;
					
					if(oldItem != -1)
						itemIsFree[oldItem] = true;

					foundFreeItem = true;
					break;
				}
				
				lookedAt++;
				currentItem += next ? 1 : -1;
			}

			if(!foundFreeItem)
				currentItem = oldItem;

			AyloDebug.Assert(currentItem != -1);

			return items[currentItem];
		}

		public int GetNumberOfFreeItems() {
			int nFree = 0;
			foreach(var free in itemIsFree)
				if(free)
					nFree++;

			return nFree;
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

public class GameObjectFunctions {
	// Returns a child of a game object with a given name.
	//
	// If more than one child name is given the function will return the nested child.
	//
	// For example the call GetChild(gameObject, "a", "b", "c") returns the object "c"
	// which is a child of "b" which again is a child of "a", e.g., it returns a->b->c.
	//
	// gameObject - The top level game object.
	// names - The names of child game objects.
	public static GameObject GetChild(GameObject gameObject, params string[] names) {
		var currentGameObject = gameObject;

		foreach(var name in names)
			currentGameObject = GetFirstLevelChild(currentGameObject, name);

		return currentGameObject;
	}

	// Same as the above function except the first game object is also given in the names parameter.
	public static GameObject GetChild(params string[] names) {
		string firstName = names[0];
		string[] restOfNames = new string[names.Length - 1];
		for(int i = 1; i < names.Length; i++)
			restOfNames[i - 1] = names[i];

		return GetChild(GameObject.Find(firstName), restOfNames);
	}

	static GameObject GetFirstLevelChild(GameObject gameObject, string name) {
		for(int i = 0; i < gameObject.transform.childCount; i++) {
			var child = gameObject.transform.GetChild(i).gameObject;
			
			if(child.name == name)
				return child;
		}

		AyloDebug.Assert(false); // Not found
		return null;
	}
}