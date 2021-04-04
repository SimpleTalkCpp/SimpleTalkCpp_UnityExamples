using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Lesson007_Raycast : MonoBehaviour
{
    public float maxDistance = 10;
    public LayerMask layerMask = -1; // everything


    Ray ray;
    bool bHit;
    RaycastHit hit;

	void Update()
    {

        ray = new Ray(transform.position, transform.forward);
        hit = new RaycastHit();
        bHit = Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }

	private void OnDrawGizmos()
	{
        MyGizmos.Label(transform.position + Vector3.up * 0.35f, $"LayerMask (int) = {layerMask.value}");
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.25f);
		Gizmos.DrawLine(ray.origin, ray.GetPoint(maxDistance));

        if (bHit) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hit.point, hit.point + hit.normal);

            Gizmos.DrawCube(hit.point, Vector3.one * 0.25f);
        }
	}
}
