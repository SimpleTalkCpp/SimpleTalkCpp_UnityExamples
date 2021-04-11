using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex003_DrawCircle : MyShape
{
	public Vector2Int center = new Vector2Int(60, 60);
	public int radius = 60;

	[Range(1, 50)]
	public float antiAliasOutlineWidth = 1;

	public Color color = new Color(1, 0.5f, 1, 1);

	[Range(0, 100)]
	public int debugStepLimit = 100;

	public enum Type {
		NaiveOutline,
		Outline,
		Fill,
		FillAntiAlias,
		AntiAliasOutline,
	}
	public Type type = Type.FillAntiAlias;

	void DrawFillCircle(MyCanvas canvas, in Vector2Int center, int radius, in Color color) {
		int r2 = radius * radius;
		for (int y = 0; y <= radius; y++) {
			if (y >= debugStepLimit) return;

			float dx = Mathf.Sqrt(r2 - (y * y));

			for (int x = 0; x <= dx; x++) {				
				canvas.SetPixel(center.x + x, center.y + y, color);
				canvas.SetPixel(center.x - x, center.y + y, color);
				canvas.SetPixel(center.x + x, center.y - y, color);
				canvas.SetPixel(center.x - x, center.y - y, color);
			}
		}
	}

	void DrawFillAACircle(MyCanvas canvas, in Vector2Int center, int radius, in Color color) {
		int r2 = radius * radius;
		for (int y = 0; y <= radius; y++) {
			if (y >= debugStepLimit) return;

			float dx = Mathf.Sqrt(r2 - (y * y));
			if (y > dx) return;

			for (int x = 0; x <= dx; x++) {
				float a = dx - x;
				if (a > 1) a = 1;

				var col = color;
				col.a = a;

				canvas.BlendPixel(center.x + x, center.y + y, col);
				canvas.BlendPixel(center.x - x, center.y + y, col);
				canvas.BlendPixel(center.x + x, center.y - y, col);
				canvas.BlendPixel(center.x - x, center.y - y, col);

				canvas.BlendPixel(center.y + y, center.x + x, col);
				canvas.BlendPixel(center.y + y, center.x - x, col);
				canvas.BlendPixel(center.y - y, center.x + x, col);
				canvas.BlendPixel(center.y - y, center.x - x, col);
			}
		}
	}

	void DrawNaiveCircleOutline(MyCanvas canvas, in Vector2Int center, int radius, in Color color) {
		int r2 = radius * radius;
		for (int y = 0; y <= radius; y++) {
			if (y >= debugStepLimit) return;

			float dx = Mathf.Sqrt(r2 - (y * y));

			canvas.SetPixel(center.x + (int)dx, center.y + y, color);
			canvas.SetPixel(center.x - (int)dx, center.y + y, color);
			canvas.SetPixel(center.x + (int)dx, center.y - y, color);
			canvas.SetPixel(center.x - (int)dx, center.y - y, color);
		}
	}

	void DrawCircleOutline(MyCanvas canvas, in Vector2Int center, int radius, in Color color) {
		int r2 = radius * radius;
		for (int y = 0; y <= radius; y++) {
			if (y >= debugStepLimit) return;

			float dx = Mathf.Sqrt(r2 - (y * y));

			if (y >　dx) break;
			int x = (int)dx;

			canvas.SetPixel(center.x + x, center.y + y, color);
			canvas.SetPixel(center.x - x, center.y + y, color);
			canvas.SetPixel(center.x + x, center.y - y, color);
			canvas.SetPixel(center.x - x, center.y - y, color);

			canvas.SetPixel(center.x + y, center.y + x, color);
			canvas.SetPixel(center.x - y, center.y + x, color);
			canvas.SetPixel(center.x + y, center.y - x, color);
			canvas.SetPixel(center.x - y, center.y - x, color);
		}
	}

	void DrawCircleAAOutline(MyCanvas canvas, in Vector2Int center, int radius, float antiAliasOutlineWidth, in Color color) {
		int n = radius + Mathf.CeilToInt(antiAliasOutlineWidth);

		for (int y = 0; y <= n; y++) {
			if (y >= debugStepLimit) return;

			for (int x = 0; x <= n; x++) {
				float r = Mathf.Sqrt((x * x) + (y * y));

				float d = Mathf.Abs(r - radius) - antiAliasOutlineWidth + 1;
				float alpha = 1 - Mathf.Clamp01(d);
				if (alpha <= 0) continue;

				var col = color;
				col.a = alpha;

				canvas.BlendPixel(center.x + x, center.y + y, col);
				canvas.BlendPixel(center.x - x, center.y + y, col);
				canvas.BlendPixel(center.x + x, center.y - y, col);
				canvas.BlendPixel(center.x - x, center.y - y, col);
			}
		}
	}

	public override void OnDraw(MyCanvas canvas) {
		switch (type) {
			case Type.Fill:				DrawFillCircle			(canvas, center, radius, color); break;
			case Type.FillAntiAlias:	DrawFillAACircle		(canvas, center, radius, color); break;
			case Type.NaiveOutline:		DrawNaiveCircleOutline	(canvas, center, radius, color); break;
			case Type.Outline:			DrawCircleOutline		(canvas, center, radius, color); break;
			case Type.AntiAliasOutline:	DrawCircleAAOutline		(canvas, center, radius, antiAliasOutlineWidth, color); break;
		}
	}
}
