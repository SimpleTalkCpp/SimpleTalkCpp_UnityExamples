using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ex009 : MonoBehaviour {
	public Vector3 rotA;
	public Vector3 rotB;

	[Range(0,1)]
	public float weight;

	const int steps = 128;

	void drawVector(Vector3 v, Color color) {
		var old_color = Gizmos.color;
		var old_matrix = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = color;
		Gizmos.DrawSphere(v, 0.05f);
		Gizmos.DrawLine(Vector3.zero, v);
		Gizmos.color = old_color;
		Gizmos.matrix = old_matrix;
	}
	
	void drawLines(Vector3[] v, Color color) {
		if (v == null)
			return;
		if ( v.Length < 2)
			return;

		var old_color = Gizmos.color;
		var old_matrix = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = color;
		for (int i = 1; i < v.Length; i++) {
			Gizmos.DrawLine(v[i-1], v[i]);
		}
		Gizmos.color = old_color;
		Gizmos.matrix = old_matrix;
	}

	Vector3 lerp_quat(float w) {
		var a = Quaternion.Euler(rotA);
		var b = Quaternion.Euler(rotB);

		var q = Quaternion.Lerp(a, b, w);
		var v = q * new Vector3(0,0,1);
		return v;
	}

	Vector3 slerp_quat(float w) {
		var a = Quaternion.Euler(rotA);
		var b = Quaternion.Euler(rotB);

		var q = Quaternion.Slerp(a, b, w);
		var v = q * new Vector3(0,0,1);
		return v;
	}

	Vector3 lerp_euler(float w) {
		var r = Vector3.Lerp(rotA, rotB, w);
		var q = Quaternion.Euler(r);
		var v = q * new Vector3(0,0,1);
		return v;
	}

	static Matrix4x4 lerp(Matrix4x4 a, Matrix4x4 b, float w) {
		var o = new Matrix4x4();
		for (int i =0; i<16; i++) {
			o[i] = Mathf.Lerp(a[i], b[i], w);
		}
		return o;
	}

	Vector3 lerp_matrix(float w) {
		var a = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotA), Vector3.one); // Matrix4x4.Rotate() required Unity 2017.3
		var b = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotB), Vector3.one);
		var v = lerp(a, b, w) * new Vector3(0,0,1);
		return v;
	}

	void example1() {
		var lines_quat   = new Vector3[steps + 1];
		var lines_quat_s = new Vector3[steps + 1];
		var lines_euler  = new Vector3[steps + 1];
		var lines_matrix = new Vector3[steps + 1];

		for (int i = 0; i <= steps; i++) {
			float w = (float)i / steps;
			lines_quat[i]   = lerp_quat(w);
			lines_euler[i]  = lerp_euler(w);
			lines_matrix[i] = lerp_matrix(w);
		}

		drawVector(lerp_quat(weight),   new Color(1,0,0));
		drawVector(slerp_quat(weight),  new Color(1,0,1));
		drawVector(lerp_euler(weight),  new Color(0,1,0));
		drawVector(lerp_matrix(weight), new Color(0,0,1));

		drawLines(lines_quat,   new Color(1,0,0));
		drawLines(lines_quat_s, new Color(1,0,1));
		drawLines(lines_euler,  new Color(0,1,0));
		drawLines(lines_matrix, new Color(0,0,1));
	}

	private void OnDrawGizmos() {
		example1();
	}
}
