using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex001_DrawRect : MyShape
{
	public Vector2Int offset = new Vector2Int(130, 20);
	public Vector2Int size   = new Vector2Int(120, 100);

	public bool fill = true;
	public Color fillColor = new Color(.5f, 1, .5f, 1);

	public bool outline = true;
	public Color outlineColor = new Color(0,0,0,1);

	public override void OnDraw(MyCanvas canvas)
	{
		if (fill)    FillRect   (canvas, offset.x, offset.y, size.x, size.y, fillColor);
		if (outline) DrawOutline(canvas, offset.x, offset.y, size.x, size.y, outlineColor);
	}

	static void FillRect(MyCanvas canvas, int offsetX, int offsetY, int width, int height, in Color color) {
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				canvas.SetPixel(offsetX + x, offsetY + y, color);
			}
		}
	}

	static void DrawOutline(MyCanvas canvas, int offsetX, int offsetY, int width, int height, in Color color) {
		for (int x = 0; x < width; x++) {			
			canvas.SetPixel(offsetX + x, offsetY,              color); // top
			canvas.SetPixel(offsetX + x, offsetY + height - 1, color); // bottom
		}

		for (int y = 0; y < height; y++) {			
			canvas.SetPixel(offsetX,             offsetY + y, color); // Left
			canvas.SetPixel(offsetX + width - 1, offsetY + y, color); // Right
		}
	}
}
