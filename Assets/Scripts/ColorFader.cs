

public class ColorFader : MonoBehaviour {
	new Camera camera;
	Texture2D texture;
	Color color = Color.black;
	float fadeAmount = 0;

	public static ColorFader Create(Camera camera) {
		var colorFader = new GameObject("colorFader", typeof(ColorFader)).GetComponent<ColorFader>();
		colorFader.SetCamera(camera);

		return colorFader;
	}

	public void SetColor(Color color) {
		this.color = color;

		UpdateTexture();
	}

	void Start() {
		texture = new Texture2D(1, 1);
		texture.alphaIsTransparency = true;
		UpdateTexture();
	}
	
	void OnGUI() {
		if(fadeAmount != 0 && camera != null)
			GUI.DrawTexture(camera.pixelRect, texture);
	}

	public void SetFadeAmount(float amount) {
		fadeAmount = Mathf.Clamp(amount, 0, 1);
		
		UpdateTexture();
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

