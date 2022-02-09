using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Joint : MonoBehaviour
{
	public void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.right);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up);

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}
}
