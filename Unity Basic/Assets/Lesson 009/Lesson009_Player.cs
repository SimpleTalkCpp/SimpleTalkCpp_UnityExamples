using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson009_Player : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 180;

	private void Start()
	{
		DontDestroyOnLoad(gameObject); // add this to avoid destrou when load level
	}

	void Update()
    {
        UpdateMovement();
    }

	void UpdateMovement() {
        float moveDir = 0;
        float rotAmount = 0;

        if (Input.GetKey(KeyCode.W)) moveDir += 1;
        if (Input.GetKey(KeyCode.S)) moveDir -= 1;

        if (Input.GetKey(KeyCode.A)) rotAmount -= 1;
        if (Input.GetKey(KeyCode.D)) rotAmount += 1;

        if (moveDir != 0) {
            transform.Translate(0, 0, moveDir * _speed * Time.deltaTime);
        }

        if (rotAmount != 0) {
            transform.Rotate(0, rotAmount * _rotateSpeed * Time.deltaTime, 0);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        Debug.Log($"OnTriggerEnter {other.name}");

		var portal = other.GetComponent<Lesson009_Portal>();
        if (portal) {
            portal.LoadLevel();
        }
	}

	private void OnDrawGizmos()
	{
		var box = GetComponent<BoxCollider>();
		if (box) {
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color(1,0,0, 1);
			Gizmos.DrawCube(box.center, box.size);
		}
	}
}
