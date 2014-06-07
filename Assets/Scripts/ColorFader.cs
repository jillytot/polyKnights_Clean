using UnityEngine;

public class ColorFader : MonoBehaviour {
	new Camera camera;
	Texture2D texture;
	Color color = Color.black;
	float fadeAmount = 0;
	bool fading;
	float fadeStartTime;
	float fadeTime;

	public static ColorFader Create(Camera camera) {
		var colorFader = new GameObject("colorFader", typeof(ColorFader)).GetComponent<ColorFader>();
		colorFader.SetCamera(camera);

		return colorFader;
	}

	public void SetColor(Color color) {
		this.color = color;

		UpdateTexture();
	}

	public float FadeAmount {
		set {
			fadeAmount = Mathf.Clamp(value, 0, 1);
			UpdateTexture();
		}
	}

	public void BeginFade(float time) {
		fadeTime = time;
		fading = true;
		fadeStartTime = Time.time;
	}

	public bool Fading {
		get {
			return fading;
		}
	}

	void Start() {
		texture = new Texture2D(1, 1);
		texture.alphaIsTransparency = true;
		UpdateTexture();
	}
	
	void OnGUI() {
		if(camera == null)
			return;

		if(fading) {
			float timeSinceStart = Time.time - fadeStartTime;

			if(timeSinceStart <= fadeTime)
				FadeAmount = timeSinceStart / fadeTime;
			else
				FadeAmount = 1;
		}

		if(fadeAmount != 0)
			GUI.DrawTexture(camera.pixelRect, texture);
	}

	void UpdateTexture() {
		var textureColor = color;
		textureColor.a = fadeAmount;
		texture.SetPixel(0, 0, textureColor);
		texture.Apply();
	}

	void SetCamera(Camera camera) {
		this.camera = camera;
	}
}