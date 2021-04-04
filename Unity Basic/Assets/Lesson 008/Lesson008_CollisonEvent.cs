using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson008_CollisonEvent : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 180;
	public bool useRigidBodyMove = true;

	Rigidbody _rigidbody;

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log($"CollisionEnter {collision.gameObject.name}");
		foreach (var c in collision.contacts) {
			Debug.DrawRay(c.point, c.normal, Color.red, 1);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		Debug.Log($"CollisionExit {collision.gameObject.name}");
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log($"TriggerEnter {other.gameObject.name}");
		Debug.DrawRay(other.transform.position, Vector3.up, Color.yellow, 1);
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.Log($"TriggerExit {other.gameObject.name}");
	}

	private void Update()
	{
		if (!useRigidBodyMove) {
			float deltaTime = Time.deltaTime;

			float moveDir = 0;
			float rotAmount = 0;

			if (Input.GetKey(KeyCode.W)) moveDir += 1;
			if (Input.GetKey(KeyCode.S)) moveDir -= 1;

			if (Input.GetKey(KeyCode.A)) rotAmount -= 1;
			if (Input.GetKey(KeyCode.D)) rotAmount += 1;

			if (moveDir != 0) {
				var t = new Vector3(0,0, moveDir * _speed * deltaTime);
				transform.Translate(t);
			}

			if (rotAmount != 0) {
				var eular = new Vector3(0, rotAmount * _rotateSpeed * deltaTime, 0);
				transform.Rotate(eular);
			}
		}
	}

	private void FixedUpdate()
	{
		if (useRigidBodyMove) {
			float deltaTime = Time.fixedDeltaTime;

			float moveDir = 0;
			float rotAmount = 0;

			if (Input.GetKey(KeyCode.W)) moveDir += 1;
			if (Input.GetKey(KeyCode.S)) moveDir -= 1;

			if (Input.GetKey(KeyCode.A)) rotAmount -= 1;
			if (Input.GetKey(KeyCode.D)) rotAmount += 1;

			if (moveDir != 0) {
				var t = new Vector3(0,0, moveDir * _speed * deltaTime);
				t = _rigidbody.rotation * t;
				_rigidbody.MovePosition(_rigidbody.position + t);
			}

			if (rotAmount != 0) {
				var eular = new Vector3(0, rotAmount * _rotateSpeed * deltaTime, 0);
				_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(eular));
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, transform.forward);
	}
}
