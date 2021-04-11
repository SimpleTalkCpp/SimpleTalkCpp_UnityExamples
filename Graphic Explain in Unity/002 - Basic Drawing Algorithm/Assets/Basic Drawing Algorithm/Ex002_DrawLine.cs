using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex002_DrawLine : MyShape {
	public Vector2Int a = new Vector2Int(120, 150);
	public Vector2Int b = new Vector2Int(180, 300);

	public Color color = new Color(1, 0, 0, 1);

	public enum Type {
		Naive,
		Simple,
		Bresenham,
		AntiAlias,
	}

	public Type drawAlgorithm = Type.Bresenham;

	public override void OnDraw(MyCanvas canvas) {
		switch (drawAlgorithm) {
			case Type.Naive: DrawNaiveLine(canvas, a, b, color); break;
			case Type.Simple: DrawSimpleLine(canvas, a, b, color); break;
			case Type.Bresenham: DrawBresenhamLine(canvas, a, b, color); break;
			case Type.AntiAlias: DrawAntiAliasLine(canvas, a, b, color); break;
		}
	}

	static void swap(ref Vector2Int a, ref Vector2Int b) {
		var tmp = a;
		a = b;
		b = tmp;
	}

	static void swapXY(ref Vector2Int v) {
		var tmp = v.x;
		v.x = v.y;
		v.y = tmp;
	}

	static void DrawNaiveLine(MyCanvas canvas, Vector2Int a, Vector2Int b, in Color color) {
		if (b.x < a.x) {
			swap(ref a, ref b);
		}

		int dx = b.x - a.x;
		int dy = b.y - a.y;

		if (dx == 0 || dy == 0) return;

		float slope = (float)dy / dx;
		for (int x = 0; x < dx; x++) {
			int y = (int)(slope * x);
			canvas.SetPixel(x + a.x, y + a.y, color);
		}
	}

	static void DrawSimpleLine(MyCanvas canvas, Vector2Int a, Vector2Int b, in Color color) {
		int dx = b.x - a.x;
		int dy = b.y - a.y;

		if (dx == 0 || dy == 0) return;

		if (System.Math.Abs(dx) > System.Math.Abs(dy)) {
			if (b.x < a.x) {
				swap(ref a, ref b);
				dx = b.x - a.x;
				dy = b.y - a.y;
			}

			float slope = (float)dy / dx;
			for (int x = 0; x < dx; x++) {
				int y = (int)(slope * x);
				canvas.SetPixel(x + a.x, y + a.y, color);
			}

		} else {
			if (b.y < a.y) {
				swap(ref a, ref b);
				dx = b.x - a.x;
				dy = b.y - a.y;
			}

			float slope = (float)dx / dy;
			for (int y = 0; y < dy; y++) {
				int x = (int)(slope * y);
				canvas.SetPixel(x + a.x, y + a.y, color);
			}
		}
	}

	static void DrawBresenhamLine(MyCanvas canvas, Vector2Int a, Vector2Int b, in Color color) {
		// Bresenham's line algorithm - https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

		int dx = System.Math.Abs(b.x - a.x);
		int dy = -System.Math.Abs(b.y - a.y);

		int signX = a.x < b.x ? 1 : -1;
		int signY = a.y < b.y ? 1 : -1;

		int err = dx + dy;

		int x = a.x;
		int y = a.y;

		while (true) {
			canvas.SetPixel(x, y, color);
			if (x == b.x && y == b.y) break;

			int e2 = 2 * err;
			if (e2 >= dy) {
				err += dy;
				x += signX;
			}

			if (e2 <= dx) {
				err += dx;
				y += signY;
			}
		}
	}

	static float fpart(float f) {
		return f - Mathf.Floor(f);
	}

	static float rfpart(float v) {
		return 1 - fpart(v);
	}

	static void DrawAntiAliasLine(MyCanvas canvas, Vector2Int a, Vector2Int b, in Color color) {
		bool steep = System.Math.Abs(b.y - a.y) > System.Math.Abs(b.x - a.x);
		if (steep) {
			swapXY(ref a);
			swapXY(ref b);
		}
		if (a.x > b.x) {
			swap(ref a, ref b);
		}

		var c = color;

		float dx = b.x - a.x;
		float dy = b.y - a.y;
		float gradient = dy / dx;

		float xEnd = Mathf.Round(a.x);
		float yEnd = a.y + gradient * (xEnd - a.x);
		float xGap = rfpart(a.x + 0.5f);

		{
			int x = (int)xEnd;
			int y = (int)yEnd;

			if (steep) {
				canvas.BlendPixel(y,     x, new Color(c.r, c.g, c.b, rfpart(yEnd) * xGap));
				canvas.BlendPixel(y + 1, x, new Color(c.r, c.g, c.b,  fpart(yEnd) * xGap));

			} else {
				canvas.BlendPixel(x, y,     new Color(c.r, c.g, c.b, rfpart(yEnd) * xGap));
				canvas.BlendPixel(x, y + 1, new Color(c.r, c.g, c.b,  fpart(yEnd) * xGap));
			}
		}
	// -------
		float intery = yEnd + gradient;
		xGap = fpart(b.x + 0.5f);

		{
			xEnd = Mathf.Round(b.x);
			yEnd = b.y + gradient * (xEnd - b.x);

			int x = (int)xEnd;
			int y = (int)yEnd;

			if (steep) {
				canvas.BlendPixel(y,     x, new Color(c.r, c.g, c.b, rfpart(yEnd) * xGap));
				canvas.BlendPixel(y + 1, x, new Color(c.r, c.g, c.b,  fpart(yEnd) * xGap));

			} else {
				canvas.BlendPixel(x, y,     new Color(c.r, c.g, c.b, rfpart(yEnd) * xGap));
				canvas.BlendPixel(x, y + 1, new Color(c.r, c.g, c.b,  fpart(yEnd) * xGap));
			}
		}

		if (steep) {
			for (int x = a.x + 1; x < b.x; x++) {
				canvas.BlendPixel((int)intery,     x, new Color(c.r, c.g, c.b, rfpart(intery) * xGap));
				canvas.BlendPixel((int)intery + 1, x, new Color(c.r, c.g, c.b,  fpart(intery) * xGap));
				intery += gradient;
			}
		} else {
			for (int x = a.x + 1; x < b.x; x++) {
				canvas.BlendPixel(x, (int)intery,     new Color(c.r, c.g, c.b, rfpart(intery) * xGap));
				canvas.BlendPixel(x, (int)intery + 1, new Color(c.r, c.g, c.b,  fpart(intery) * xGap));
				intery += gradient;
			}
		}
	}
}

