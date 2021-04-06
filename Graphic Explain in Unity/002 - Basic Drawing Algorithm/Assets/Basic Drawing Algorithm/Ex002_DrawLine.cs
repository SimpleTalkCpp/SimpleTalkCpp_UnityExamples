using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex002_DrawLine : MyShape
{
	public Vector2Int a = new Vector2Int(120, 150);
	public Vector2Int b = new Vector2Int(180, 300);

	public Color color = new Color(1,0,0,1);

	public enum Type {
		Naive,
		Simple,
		Bresenham,
	}

	public Type drawAlgorithm = Type.Bresenham;

	public override void OnDraw(MyCanvas canvas)
	{
		switch (drawAlgorithm) {
			case Type.Naive:		DrawNaiveLine	 (canvas, a, b, color); break;
			case Type.Simple:		DrawSimpleLine	 (canvas, a, b, color); break;
			case Type.Bresenham:	DarwBresenhamLine(canvas, a, b, color); break;
		}
	}

	static void swap(ref Vector2Int a, ref Vector2Int b) {
		var tmp = a;
		a = b;
		b = tmp;
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

	static void DarwBresenhamLine(MyCanvas canvas, Vector2Int a, Vector2Int b, in Color color) {
		// Bresenham's line algorithm - https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

		int dx =  System.Math.Abs(b.x - a.x);
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
}

