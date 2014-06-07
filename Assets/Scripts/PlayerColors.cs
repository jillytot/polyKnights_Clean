using UnityEngine;
using System.Collections;

public class PlayerColor {
	public Material[] materials;
	public Color color;

	public PlayerColor() {
		materials = new Material[2];
	}
}

public class PlayerColors : MonoBehaviour {
	public Material[] swordyMaterials;
	public Color[] colors;
	PlayerColor[] playerColors;

	public static PlayerColor[] GetColors() {
		GameObject instance = GameObject.Find("playerColors");

		return instance.GetComponent<PlayerColors>().NonstaticGetColors();
	}

	PlayerColor[] NonstaticGetColors() {
		AyloDebug.Assert(swordyMaterials.Length == 8);
		AyloDebug.Assert(colors.Length == 8);

		// Create PlayerColor array
		if(playerColors == null) {
			playerColors = new PlayerColor[swordyMaterials.Length];

			for(uint i = 0; i < 8; i++) {
				var playerColor = new PlayerColor();

				playerColor.materials[0] = swordyMaterials[i];
				playerColor.materials[1] = swordyMaterials[i];/**/
				playerColor.color = colors[i];

				playerColors[i] = playerColor;
			}
		}

		return playerColors;
	}
}
