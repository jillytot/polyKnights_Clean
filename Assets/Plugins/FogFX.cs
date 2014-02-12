using UnityEngine;

[ExecuteInEditMode]
public class FogFX : CustomBehaviour {

	public Color color = Color.gray;
	public float distance = 16f;
	public float intensity = 0.05f;
	
	void Start() {
		Shader.SetGlobalColor("_FogColor", color);
		Shader.SetGlobalFloat("_FogDistance", distance);
		Shader.SetGlobalFloat("_FogIntensity", intensity);
	}
	
	#if UNITY_EDITOR
	void Update() {
		Shader.SetGlobalColor("_FogColor", color);
		Shader.SetGlobalFloat("_FogDistance", distance);
		Shader.SetGlobalFloat("_FogIntensity", intensity);
	}
	#endif
	
}
