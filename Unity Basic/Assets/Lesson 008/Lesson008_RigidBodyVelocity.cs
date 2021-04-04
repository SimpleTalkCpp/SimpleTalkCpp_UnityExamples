using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson008_RigidBodyVelocity : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 90;

	Rigidbody _rigidbody;

	private void Start()
	{
		Application.targetFrameRate = 60;
		_rigidbody = GetComponent<Rigidbody>();
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
		_rigidbody.velocity = t;

		var eular = new Vector3(0, rotAmount * _rotateSpeed * Time.fixedDeltaTime, 0);
		_rigidbody.angularVelocity = eular;
	}
}
