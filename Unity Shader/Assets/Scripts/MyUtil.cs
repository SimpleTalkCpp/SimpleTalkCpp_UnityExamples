using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtil {

	static public void Destroy(ref Material v) {
		if (!v)
			return;
		Object.Destroy(v);
		v = null;
	}

	static public void Destroy(ref RenderTexture v) {
		if (!v)
			return;
		Object.Destroy(v);
		v = null;
	}

	static public void Destroy(ref Mesh v) {
		if (!v)
			return;
		Object.Destroy(v);
		v = null;
	}

	static public void CreateRenderTexture(	ref RenderTexture rt, 
											int width, int height, int depth, 
											RenderTextureFormat format = RenderTextureFormat.ARGB32) 
	{
		if (!rt || rt.width != width || rt.height != height || rt.depth != depth || rt.format != format) {
			Destroy(ref rt);
			rt = new RenderTexture(width, height, depth, format);
		}
	}
}
