using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson008_CapsuleCast : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 90;

	Rigidbody _rigidbody;
	CapsuleCollider _capsule;

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.isKinematic = true;

		_capsule = GetComponent<CapsuleCollider>();
	}

    void FixedUpdate()
    {
		float moveAmount = 0;
		float rotAmount = 0;

		if (Input.GetKey(KeyCode.W)) moveAmount += 1;
		if (Input.GetKey(KeyCode.S)) moveAmount -= 1;

		if (Input.GetKey(KeyCode.A)) rotAmount -= 1;
		if (Input.GetKey(KeyCode.D)) rotAmount += 1;

		if (moveAmount != 0) {
			var p1 = _capsule.center + new Vector3(0, _capsule.height / 2, 0);
			var p2 = _capsule.center - new Vector3(0, _capsule.height / 2, 0);

			var moveDistance = Mathf.Abs(moveAmount * _speed * Time.fixedDeltaTime);

			var pos = _rigidbody.position;
			var dir = _rigidbody.rotation * (Vector3.forward * moveAmount).normalized;

			const int maxIteration = 5;

			for (int i = 0; i < maxIteration; i++) {
				if (moveDistance < Mathf.Epsilon) break;

				var hit = new RaycastHit();
				if (!Physics.CapsuleCast(pos + p1, pos + p2, _capsule.radius, dir, out hit, moveDistance)) {
					var t = dir * moveDistance;
					t.y = 0;
					pos += t;
					break;
				}

				moveDistance -= hit.distance;

				var hitCapsuleCenter = hit.point + hit.normal * (_capsule.radius + 0.01f);
				hitCapsuleCenter.y = pos.y; // keep Y

				pos = hitCapsuleCenter;
				dir -= Vector3.Dot(dir, hit.normal) * hit.normal;
				Debug.DrawRay(pos, dir, Color.red, 1);
			}

			_rigidbody.MovePosition(pos);
		}

		var eular = new Vector3(0, rotAmount * _rotateSpeed * Time.fixedDeltaTime, 0);
		_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(eular));
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, transform.forward);
	}
}
