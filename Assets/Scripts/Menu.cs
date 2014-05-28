using System.Collections.Generic;

namespace Gui {
	// Implements functionality for menus.
	// A menu in this case is a collection of items (text, images, etc.) where exactly one of these items is always active (i.e. it's the currently "highlighted" item in the menu).
	//
	// For each item it can be defined what item is to become active when one of the directional buttons is pressed.
	// This means the class can be used for both ordinary menus where the items are arranged in rows but the it is also flexible enough to handle more exotic layouts.
	//
	// A MenuObserver object is used to call back to the user of the class.
	public class Menu {
		readonly MenuObserver observer;
		readonly MenuInput input;
		Dictionary<int, MenuItem> items = new Dictionary<int, MenuItem>();
		MenuItem activeItem;
		bool enabled;

		public Menu(MenuObserver observer, MenuInput input, bool enabled) {
			this.observer = observer;
			this.input = input;
			this.enabled = enabled;
		}

		public bool Enabled {
			get {
				return enabled;
			}

			set {
				enabled = value;
			}
		}

		// Adds an item that can be pressed
		public void AddButtonItem(int id) {
			AddMenuItem(id);
		}

		// Adds an item that allows for cycling through a number of values.
		// It is also possible to press this item.
		public void AddCycleItem(int id, MenuButton previousButton, MenuButton nextButton) {
			AyloDebug.Assert(IsDirectionalButton(nextButton));
			AyloDebug.Assert(IsDirectionalButton(previousButton));
			AyloDebug.Assert(nextButton != previousButton);

			var item = AddMenuItem(id);
			item.isCycleItem = true;
			item.nextButton = nextButton;
			item.previousButton = previousButton;
		}

		MenuItem AddMenuItem(int id) {
			AyloDebug.Assert(id >= 0);
			AyloDebug.Assert(!items.ContainsKey(id));

			var newItem = new MenuItem(id);
			items.Add(id, newItem);

			return newItem;
		}

		public void SetItemDirectionItems(int id, MenuButton button, int directionItem) {
			AyloDebug.Assert(items.ContainsKey(id));
			AyloDebug.Assert((int)button < 4);

			items[id].directionItems[button] = directionItem;
		}

		public void SetActiveItem(int id) {
			AyloDebug.Assert(items.ContainsKey(id));

			if(activeItem != null)
				observer.ActivateMenuItem(activeItem.id, false);

			activeItem = items[id];
			observer.ActivateMenuItem(id, true);
		}

		public void Update() {
			if(!enabled || activeItem == null)
				return;

			if(BackButtonIsPressed()) {
				/**/// aktivér parent menu
				return;
			}

			if(EnterButtonIsPressed()) {
				observer.SelectMenuItem(activeItem.id);
				return;
			}

			if(DirectionalButtonIsPressed()) {
				// Handle cycle item
				if(activeItem.isCycleItem) {
					if(input.GetButton(activeItem.nextButton)) {
						observer.CycleMenuItem(activeItem.id, true);
						return;
					}
					else if(input.GetButton(activeItem.previousButton)) {
						observer.CycleMenuItem(activeItem.id, false);
						return;
					}
				}

				// Move to a different item
				var newActiveItemId = activeItem.directionItems[GetDirectionalButton()];
				if(newActiveItemId != -1)
					SetActiveItem(newActiveItemId);
			}
		}

		bool BackButtonIsPressed() {
			return input.GetButton(MenuButton.Back);
		}

		bool EnterButtonIsPressed() {
			return input.GetButton(MenuButton.Enter);
		}

		bool DirectionalButtonIsPressed() {
			return input.GetButton(MenuButton.Left) ||
				input.GetButton(MenuButton.Right) ||
				input.GetButton(MenuButton.Up) ||
				input.GetButton(MenuButton.Down);
		}

		MenuButton GetDirectionalButton() {
			if(input.GetButton(MenuButton.Left))
				return MenuButton.Left;
			else if(input.GetButton(MenuButton.Right))
				return MenuButton.Right;
			else if(input.GetButton(MenuButton.Up))
				return MenuButton.Up;
			else if(input.GetButton(MenuButton.Down))
				return MenuButton.Down;

			AyloDebug.Assert(false);
			return MenuButton.Left;
		}

		bool IsDirectionalButton(MenuButton button) {
			return button == MenuButton.Left ||
				button == MenuButton.Right ||
				button == MenuButton.Up ||
				button == MenuButton.Down;
		}

		class MenuItem {
			readonly public int id;
			public bool active;
			public Dictionary<MenuButton, int> directionItems; // The id of the item the menu should move to when a certain directional buttons is pressed
			public bool isCycleItem;
			public MenuButton nextButton;
			public MenuButton previousButton;

			public MenuItem(int id) {
				this.id = id;
				active = false;

				directionItems = new Dictionary<MenuButton, int> {{MenuButton.Left, -1}, {MenuButton.Right, -1}, {MenuButton.Up, -1}, {MenuButton.Down, -1}};
			}
		}
	}

	public enum CycleDirection {Next, Previous};

	public interface MenuObserver {
		// Called when the item becomes the active item and when it stops being the active item
		void ActivateMenuItem(int id, bool activate);
		
		// Called when the enter button is pressed while the menu item is active
		void SelectMenuItem(int id);
		
		// Called when the next/previous value of the item should be shown.
		// The next parameter determines the direction.
		// This function is only called on cycle items.
		void CycleMenuItem(int id, bool next);
	}

	public enum MenuButton {
		Left,
		Right,
		Up,
		Down,
		Enter,
		Back
	};

	public interface MenuInput {
		// Should return true when a button is first pressed.
		// Should return true periodically when a button is held. The time period is defined by the implementing class.
		bool GetButton(MenuButton button);
	}
}