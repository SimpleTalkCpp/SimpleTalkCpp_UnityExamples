
using UnityEngine;
using UnityEditor;

public class CreateTexture : MonoBehaviour
{
	[MenuItem("MyMenu/CreateTexture")]
	static void DoCreateTexture()
	{
		var tex = new Texture2D(256, 256, TextureFormat.ARGB32, false);

		var pixels = new Color32[256 * 256];

		for (int y = 0; y < 256; y++) {
			for (int x = 0; x < 256; x++) {
				pixels[y * 256 + x] = new Color32((byte)x, (byte)y, 0, 255);
			}
		}

		tex.SetPixels32(pixels);

		var data = tex.EncodeToPNG();

		System.IO.File.WriteAllBytes("Assets/Textures/MyTexture.png", data);
	}
}
