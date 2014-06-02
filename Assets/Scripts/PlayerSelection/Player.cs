using UnityEngine;
using System.Collections.Generic;
using Gui;

namespace PlayerSelection {
	// Represents the appearance and functionality (including input) of a single player object on the player selection screen.
	//
	// Note: It might be a good idea to seperate the stuff related to graphics into a seperate class.
	class Player : MenuObserver {
		GameObject playerObject;
		Menu menu;
		int menuItemColor, menuItemClass, menuItemReady;
		IDictionary<int, GameObject> menuItems = new Dictionary<int, GameObject>();
		bool joined;
		int activeMenuItem;
		int currentColorIndex = -1;
		int currentPlayerClass;
		bool ready;

		SkinnedMeshRenderer playerMesh;
		Animator playerAnimator;
		MeshRenderer leftArrow, rightArrow;
		MeshRenderer colorRectangle;
		TextMesh classText;
		TextMesh readyText;
		Material disabledMaterial;
		
		static CyclicAllocator<PlayerColor> colorAllocator;/**/
		static string[] playerClasses = {"Swordy", "Mage", "Rawrior", "Bunny", "Kardashian", "Dalmatian", "Robot", "Tim Schafer", "Aylo Dev", "Luigi", "Biatch"};/**/
		
		public Player(GameObject playerObject, Material disabledMaterial) {
			this.playerObject = playerObject;
			this.disabledMaterial = disabledMaterial;
			
			if(colorAllocator == null)/**/
				colorAllocator = new CyclicAllocator<PlayerColor>(PlayerColors.GetColors());/**/
			
			playerMesh = GameObjectFunctions.GetChild(playerObject, "chickFillet", "chickFillet_mesh").GetComponent<SkinnedMeshRenderer>();
			playerAnimator = GameObjectFunctions.GetChild(playerObject, "chickFillet").GetComponent<Animator>();
			
			leftArrow = GameObjectFunctions.GetChild(playerObject, "left").GetComponent<MeshRenderer>();
			rightArrow = GameObjectFunctions.GetChild(playerObject, "right").GetComponent<MeshRenderer>();
			
			colorRectangle = GameObjectFunctions.GetChild(playerObject, "color").GetComponent<MeshRenderer>();
			classText = GameObjectFunctions.GetChild(playerObject, "class").GetComponent<TextMesh>();
			readyText = GameObjectFunctions.GetChild(playerObject, "ready").GetComponent<TextMesh>();
			
			CreateMenu();
			Joined = false;
		}
		
		public void Update() {
			menu.Update();
		}
		
		public bool Ready {
			get {
				return ready;
			}
		}

		public void MakeReady() {
			menu.Enabled = false;
			ShowMenu(false);
			playerAnimator.SetTrigger("Ready");
			ready = true;
		}

		public bool Joined {
			set {
				joined = value;
				
				menu.Enabled = value;
				
				if(value)
					CycleMenuItem(menuItemColor, value);
				else
					playerMesh.material = disabledMaterial;
				
				Animate(value);
				ShowMenu(value);
			}
			
			get {
				return joined;
			}
		}

		public void Join(PlayerInput input) {
			menu.Input = new PlayerToMenuInput(input);
			Joined = true;
			menu.SetActiveItem(0);
		}

		void CreateMenu() {
			menu = new Menu(this, false);

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
			if(id == menuItemReady)
				MakeReady();
		}
		
		public void CycleMenuItem(int id, bool next) {
			if(id == menuItemColor)
				CyclePlayerColor(next);
			else if(id == menuItemClass)
				CyclePlayerClass(next);
			else
				AyloDebug.Assert(false);
		}
		
		void CyclePlayerColor(bool next) {
			SetColor(colorAllocator.Get(ref currentColorIndex, next));
		}
		
		void CyclePlayerClass(bool next) {
			currentPlayerClass += next ? 1 : -1;
			NumberFunctions.WrapAround(ref currentPlayerClass, playerClasses.Length);
			
			classText.text = playerClasses[currentPlayerClass];
			UpdateArrowPositions();
		}
		
		void ShowMenu(bool show) {
			leftArrow.enabled = show;
			rightArrow.enabled = show;
			colorRectangle.enabled = show;
			GameObjectFunctions.GetChild(playerObject, "class").GetComponent<MeshRenderer>().enabled = show;
			GameObjectFunctions.GetChild(playerObject, "ready").GetComponent<MeshRenderer>().enabled = show;
		}
		
		void UpdateArrowPositions() {
			var gapBetweenItemAndArrow = 0.2f;
			var itemBounds = menuItems[activeMenuItem].GetComponent<Renderer>().bounds;
			var centerX = itemBounds.center.x;
			var x = itemBounds.extents.x + gapBetweenItemAndArrow;
			var y = menuItems[activeMenuItem].transform.position.y;
			
			if(activeMenuItem == menuItemReady)
				x = -x;
			
			leftArrow.transform.position = new Vector3(centerX - x, y, 0);
			rightArrow.transform.position = new Vector3(centerX + x, y, 0);
		}
		
		void Animate(bool animate) {
			playerAnimator = GameObjectFunctions.GetChild(playerObject, "chickFillet").GetComponent<Animator>();
			playerAnimator.enabled = animate;
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