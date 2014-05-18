using UnityEngine;

public class AyloDebug {
	public static void Assert(bool condition) {
		if(!condition)
			UnityEngine.Debug.LogError("Assertion failed!\n" + System.Environment.StackTrace);
	}
}