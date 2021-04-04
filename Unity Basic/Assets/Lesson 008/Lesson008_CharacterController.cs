using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson008_CharacterController : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 90;

	CharacterController _ctrl;

	private void Start()
	{
		GetComponent<Rigidbody>().isKinematic = true;
		_ctrl = GetComponent<CharacterController>();
	}

	void FixedUpdate()
    {
		float moveDir = 0;
		float rotAmount = 0;

		if (Input.GetKey(KeyCode.W)) moveDir += 1;
		if (Input.GetKey(KeyCode.S)) moveDir -= 1;

		if (Input.GetKey(KeyCode.A)) rotAmount -= 1;
		if (Input.GetKey(KeyCode.D)) rotAmount += 1;

		var t = new Vector3(0,0, moveDir * _speed);
		t = transform.rotation * t;

		_ctrl.SimpleMove(t);

		var eular = new Vector3(0, rotAmount * _rotateSpeed * Time.fixedDeltaTime, 0);
		transform.Rotate(eular);
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, transform.forward);
	}
}
