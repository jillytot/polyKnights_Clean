using UnityEngine;
using System.Collections;

public class PlayerColor {
	public Material material;
	public Color color;

	public PlayerColor(Material material, Color color) {
		this.material = material;
		this.color = color;
	}
}

public class PlayerColors : MonoBehaviour {
	public Material[] materials;
	public Color[] colors;
	PlayerColor[] playerColors;

	public static PlayerColor[] GetColors() {
		GameObject instance = GameObject.Find("playerColors");

		AyloDebug.Assert(instance != null); // Make sure the scene contains a PlayerColors prefab called "playerColors"

		return instance.GetComponent<PlayerColors>().NonstaticGetColors();
	}

	PlayerColor[] NonstaticGetColors() {
		AyloDebug.Assert(materials.Length == colors.Length);

		if(playerColors == null) {
			playerColors = new PlayerColor[materials.Length];

			for(uint i = 0; i < materials.Length; i++)
				playerColors[i] = new PlayerColor(materials[i], colors[i]);
		}

		return playerColors;
	}
}
