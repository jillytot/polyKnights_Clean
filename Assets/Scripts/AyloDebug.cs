using UnityEngine;
using System.Diagnostics;

public class AyloDebug {
	[ConditionalAttribute("UNITY_EDITOR")]
	public static void Assert(bool condition) {
		if(!condition)
			UnityEngine.Debug.LogError("Assertion failed!\n" + System.Environment.StackTrace);
	}
}