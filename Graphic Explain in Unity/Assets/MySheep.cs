using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DefaultExecutionOrder(10)]
public class MySheep : MonoBehaviour
{
    public float speed = 5;
    public float collisionRadius = 2;

    public Vector3 lastPosition;
    public Vector3 lastVelocity;

	public void Update()
	{
        var pos = transform.localPosition;
        var forward = transform.forward;

        var hit = new RaycastHit();
        if (Physics.Raycast(pos, forward, out hit, collisionRadius)) {
            var dot = Vector3.Dot(forward, hit.normal);

            if (dot < -0.99f) {
                forward = Random.Range(0,1) == 0 ? transform.right : -transform.right;
            } else {
                forward -= hit.normal * dot;
            }

            transform.localRotation = Quaternion.LookRotation(forward);
            return;
        }

        pos += forward * (speed * Time.deltaTime);
        transform.localPosition = pos;
	}
}
