using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex002_DrawLine : MyCanvasDrawBase
{
	public int x0 = 20;
	public int y0 = 50;

	public int x1 = 400;
	public int y1 = 300;

	public Color color = new Color(1,0,0,1);

	public enum DrawAlgorithm {
		Simple,
		Bresenham,
	}

	public DrawAlgorithm drawAlgorithm = DrawAlgorithm.Bresenham;

	public override void OnDraw(MyCanvas canvas)
	{
		switch (drawAlgorithm) {
			case DrawAlgorithm.Simple:		DrawLine(canvas); break;
			case DrawAlgorithm.Bresenham:	DarwBresenhamLine(canvas); break;
		}
	}

	void DrawLine(MyCanvas canvas) {
		int dx = System.Math.Abs(x1 - x0);
		int dy = System.Math.Abs(y1 - y0);

		if (dx == 0 || dy == 0) return;

		if (dx > dy) {
			float m = (float)dy / dx;
			for (int x = 0; x < dx; x++) {
				int y = (int)(m * x);
				canvas.SetPixel(x + x0, y + y0, color);
			}
		} else {
			float m = (float)dx / dy;
			for (int y = 0; y < dy; y++) {
				int x = (int)(m * y);
				canvas.SetPixel(x + x0, y + y0, color);
			}
		}
	}

	void DarwBresenhamLine(MyCanvas canvas) {
		// Bresenham's line algorithm - https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

		int dx =  System.Math.Abs(x1 - x0);
		int dy = -System.Math.Abs(y1 - y0);

		int signX = x0 < x1 ? 1 : -1;
		int signY = y0 < y1 ? 1 : -1;

		int err = dx + dy;

		int x = x0;
		int y = y0;

		while (true) {
			canvas.SetPixel(x, y, color);
			if (x == x1 && y == y1) break;

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

