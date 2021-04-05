﻿using System.Collections;
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
        if (fill)    DrawFill(canvas);
        if (outline) DrawOutline(canvas);
    }

    void DrawFill(MyCanvas canvas) {
        for (int y = 0; y < size.y; y++) {
            for (int x = 0; x < size.x; x++) {
                canvas.SetPixel(offset.x + x, offset.y + y, fillColor);
            }
        }
    }

    void DrawOutline(MyCanvas canvas) {
        for (int x = 0; x < size.x; x++) {
            //top
            canvas.SetPixel(offset.x + x, offset.y, outlineColor);

            // bottom
            canvas.SetPixel(offset.x + x, offset.y + size.y - 1, outlineColor);
        }

        for (int y = 0; y < size.y; y++) {
            // Left
            canvas.SetPixel(offset.x, offset.y + y, outlineColor);

            // Right
            canvas.SetPixel(offset.x + size.x, offset.y + y - 1, outlineColor);
        }
    }
}
