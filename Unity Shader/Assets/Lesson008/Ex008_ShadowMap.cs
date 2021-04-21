using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ex008_ShadowMap : MonoBehaviour {

	public RenderTexture shadowMap;
	public Shader shader;
	public Texture uvChecker;

	private Vector3[] mainCameraFrustumPoints;
	private Vector3[] shadowCameraFrustumPoints;

	private void UpdatePosToFitFrustum() {
		var main = Camera.main;
		if (!main)
			return;

		var cam = GetComponent<Camera>();
		if (!cam)
			return;

		{
			var m = main.worldToCameraMatrix.inverse * main.projectionMatrix.inverse;
			mainCameraFrustumPoints = GetFrustumPoints(m);
		}

		// reset to zero instead of accumulate
		transform.localPosition = Vector3.zero;

		{
			var m = transform.worldToLocalMatrix;
			var maxPoint = new Vector3(
				GetFrustumMaxDistance(mainCameraFrustumPoints, m, Vector3.right),
				GetFrustumMaxDistance(mainCameraFrustumPoints, m, Vector3.up),
				GetFrustumMaxDistance(mainCameraFrustumPoints, m, Vector3.forward));

			var minPoint = new Vector3(
				-GetFrustumMaxDistance(mainCameraFrustumPoints, m, -Vector3.right),
				-GetFrustumMaxDistance(mainCameraFrustumPoints, m, -Vector3.up),
				-GetFrustumMaxDistance(mainCameraFrustumPoints, m, -Vector3.forward));

			var size = maxPoint - minPoint;
			var center = (maxPoint + minPoint) / 2;

			//Debug.Log("max = " + maxPoint + " min = " + minPoint + " center = " + center);

			cam.orthographicSize = Mathf.Max(size.x, size.y) / 2;
			cam.farClipPlane = size.z;

			var offset = transform.rotation * new Vector3(center.x, center.y, minPoint.z);
			transform.localPosition = offset;
		}

		{
			var m = cam.worldToCameraMatrix.inverse * cam.projectionMatrix.inverse;
			shadowCameraFrustumPoints = GetFrustumPoints(m);
		}
	}

	private float GetFrustumMaxDistance(Vector3[] points, Matrix4x4 m, Vector3 dir) {
		if (points == null || points.Length < 8)
			return 0;

		float o = float.NegativeInfinity;
		foreach (var pt in points) {
			var d = Vector3.Dot(dir, m.MultiplyPoint(pt));
			if (d > o)
				o = d;
		}
		return o;
	}

	private Vector3[] GetFrustumPoints(Matrix4x4 m) {
		var p = new Vector3[8];
		p[0] = m.MultiplyPoint(new Vector3(-1, -1, -1));
		p[1] = m.MultiplyPoint(new Vector3( 1, -1, -1));
		p[2] = m.MultiplyPoint(new Vector3( 1,  1, -1));
		p[3] = m.MultiplyPoint(new Vector3(-1,  1, -1));
		p[4] = m.MultiplyPoint(new Vector3(-1, -1,  1));
		p[5] = m.MultiplyPoint(new Vector3( 1, -1,  1));
		p[6] = m.MultiplyPoint(new Vector3( 1,  1,  1));
		p[7] = m.MultiplyPoint(new Vector3(-1,  1,  1));		
		return p;
	}


	void DrawFrustum(Vector3[] points) {
		if (points == null || points.Length < 8)
			return;

		Gizmos.DrawLine(points[0], points[1]);
		Gizmos.DrawLine(points[1], points[2]);
		Gizmos.DrawLine(points[2], points[3]);
		Gizmos.DrawLine(points[3], points[0]);

		Gizmos.DrawLine(points[4], points[5]);
		Gizmos.DrawLine(points[5], points[6]);
		Gizmos.DrawLine(points[6], points[7]);
		Gizmos.DrawLine(points[7], points[4]);

		Gizmos.DrawLine(points[0], points[4]);
		Gizmos.DrawLine(points[1], points[5]);
		Gizmos.DrawLine(points[2], points[6]);
		Gizmos.DrawLine(points[3], points[7]);
	}

	private void OnDrawGizmos() {
		Gizmos.color = new Color(0,0,1);
		DrawFrustum(mainCameraFrustumPoints);

		Gizmos.color = new Color(1,0,0);
		DrawFrustum(shadowCameraFrustumPoints);
	}

	private void Update() {	
		var cam = GetComponent<Camera>();
		if (!cam)
			return;

		UpdatePosToFitFrustum();

		cam.renderingPath = RenderingPath.Forward;

		Shader.SetGlobalVector("MyLightDir", transform.forward);


		var m = cam.projectionMatrix * cam.worldToCameraMatrix;
		Shader.SetGlobalMatrix("MyShadowVP", m);

		Shader.SetGlobalTexture("uvChecker", uvChecker);
		Shader.SetGlobalTexture("MyShadowMap", shadowMap);

		if (!shader)
			return;

		cam.targetTexture = shadowMap;
		cam.SetReplacementShader(shader, null);

	}
}
