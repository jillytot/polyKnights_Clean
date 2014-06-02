using UnityEngine;

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
	public static GameObject GetChild(GameObject gameObject, params string[] names) {/**/ // omdøb Get eller Find
		var currentGameObject = gameObject;
		
		foreach(var name in names)
			currentGameObject = GetFirstLevelChild(currentGameObject, name);
		
		return currentGameObject;
	}
	
	// Same as the above function except the first game object is also given in the names parameter.
	public static GameObject Find(params string[] names) {
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