using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringDemo : MonoBehaviour {
	public Material mat;
	public Texture2D tex;

	const float MeshSize = 20;

	const int texWidth  = 64;

	public Player player;
	
	// Control Vertex
	struct CV { 
		public Vector3 pos; // world position
		public Vector4 rot; // quaternion
		public Vector4 velocity;
	}
	CV[] m_cv;

	void GetDumping(ref Vector4 pos, ref Vector4 v, Vector4 target, float dumpingRatio, float angularFreq, float deltaTime)
	{
		float f = 1.0f + 2.0f * deltaTime * dumpingRatio * angularFreq;
		float a2 = angularFreq * angularFreq;
		float ta2 = deltaTime * a2;
		float t2a2 = deltaTime * ta2;
		float detInv = 1.0f / (f + t2a2);
		var detX = pos * f + deltaTime * v + t2a2 * target;
		var detV = v + ta2 * (target - pos);
		pos = detX * detInv;
		v   = detV * detInv;
	}

	Vector4 ToVector4(Quaternion q) {
		return new Vector4(q.x, q.y, q.z, q.w);
	}

	Color ToColor(Vector4 v) {
		return new Color(v.x, v.y, v.z, v.w);
	}
	
	public float maxDistance = 5;
	public float dumpingRatio = 0.05f;
	public float angularFreq = 15;
	public float awayAngle = 30;

	void Start() {
		{
			m_cv = new CV[texWidth * texWidth];
			{
				int i = 0;
				for (int y = 0; y < texWidth; y++) {
					for (int x = 0; x < texWidth; x++) {
						var pos = (new Vector3(x, 0, y) / texWidth - Vector3.one * 0.5f) * MeshSize;
						pos.x = -pos.x;
						m_cv[i].pos = pos;
						m_cv[i].rot = new Vector4(0,0,0,1);
						i++;
					}
				}
			}
		}

		{
			tex = new Texture2D(texWidth, texWidth, TextureFormat.RGBAFloat, false);
			tex.name = "QuatTex";
		}	}
	void Update () {


		if (player == null)
			return;

		var deltaTime = Time.deltaTime;
		var playerPos = player.transform.position;
		var quatIden = new Vector4(0,0,0,1);
		
		var colors = new Color[texWidth * texWidth];
		const int n = texWidth * texWidth;
		for (int i = 0; i < n; i++) {
			var d = playerPos - m_cv[i].pos;
			var dis = d.magnitude;

			dis = Mathf.Clamp01((dis - maxDistance) / maxDistance);
			//dis = Mathf.Clamp01((maxDistance - dis) / maxDistance);
			dis = 1 - dis;

			var dir = d.normalized;

			var rot = m_cv[i].rot;

			var axis = Vector3.Cross(dir, Vector3.up);
			var target = ToVector4(Quaternion.AngleAxis(awayAngle, axis));

			target = Vector4.Lerp(target, quatIden, dis);

			GetDumping(ref m_cv[i].rot, ref m_cv[i].velocity, target, dumpingRatio, angularFreq, deltaTime);

			colors[i] = ToColor(m_cv[i].rot);
			//colors[i] = new Color(dis, 0, 0);
		}

		tex.SetPixels(colors);
		tex.Apply();

		if (mat) {
			mat.SetTexture("_QuatTex", tex);
		}
	}
}
