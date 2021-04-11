using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ex004_DrawTriangle : MyShape
{
	public Vector2Int a = new Vector2Int(320, 10);
	public Vector2Int b = new Vector2Int(270, 150);
	public Vector2Int c = new Vector2Int(400, 100);

	public Color color = new Color(.3f, .3f, 1);

	[Range(0, 10)]
	public int pointSize = 6;

	static void swap(ref Vector2Int a, ref Vector2Int b) {
		var tmp = a;
		a = b;
		b = tmp;
	}

	public override void OnDraw(MyCanvas canvas) {
		DrawTriangle(canvas, a, b, c, color, pointSize);
	}

	static void DrawTriangle(MyCanvas canvas, Vector2Int a, Vector2Int b, Vector2Int c, in Color color, int pointSize) {
		if (b.y < a.y) swap(ref a, ref b);
		if (c.y < a.y) swap(ref a, ref c);
		if (c.y < b.y) swap(ref b, ref c);

		int dx = c.x - b.x;
		int dy = c.y - b.y;

		if (b.y == c.y) {
			_DrawFlatTrangle(canvas, a, b, c, color);

		} else if (a.y == b.y) {
			_DrawFlatTrangle(canvas, c, a, b, color);

		} else {
			var mid = new Vector2Int();
			mid.x = lineIntersect(a, c, b.y);
			mid.y = b.y;

			_DrawFlatTrangle(canvas, a, b, mid, color);
			_DrawFlatTrangle(canvas, c, b, mid, color);

			canvas.DrawPoint(mid, pointSize, new Color(1,0,1));
		}

		if (pointSize > 0) {
			canvas.DrawPoint(a, pointSize, new Color(1,0,0));
			canvas.DrawPoint(b, pointSize, new Color(0,1,0));
			canvas.DrawPoint(c, pointSize, new Color(0,0,1));
		}
	}

	static int lineIntersect(in Vector2Int a, in Vector2Int b, int y) {
 		int dx = b.x - a.x;
		int dy = b.y - a.y;

		if (dy == 0) return a.x;
		return (int)((y - a.y) * ((float)dx/dy) + a.x);
	}

	static void _DrawFlatTrangle(MyCanvas canvas, in Vector2Int a, in Vector2Int b, in Vector2Int c, in Color color) {
		if (b.y != c.y) {
			Debug.LogError("");
		}

		int dy = b.y - a.y;
		int step = 1;
		if (dy < 0) {
			dy = -dy;
			step = -1;
		}

		for (int i = 0; i <= dy; i++) {
			int y = a.y + i * step;

			var start = lineIntersect(a, b, y);
			var end   = lineIntersect(a, c, y);

			if (start > end) {
				var tmp = start;
				start = end;
				end = tmp;
			}

			for (int x = start; x <= end; x++) {
				canvas.BlendPixel(x, y, color);
			}
		}
	}
}


