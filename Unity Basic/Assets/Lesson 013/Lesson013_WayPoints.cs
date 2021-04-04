using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson013_WayPoints : MonoBehaviour
{
	public Vector3[] points;

	private void OnDrawGizmos()
	{
        if (points != null) {
            var lastPos = Vector3.zero;

            Gizmos.matrix = transform.localToWorldMatrix;

            for (int i = 0; i < points.Length; i++) {
                var pos = points[i];
                if (i > 0) {
                    Gizmos.DrawLine(lastPos, pos);
                }
                lastPos = pos;
            }
        }
	}
}
