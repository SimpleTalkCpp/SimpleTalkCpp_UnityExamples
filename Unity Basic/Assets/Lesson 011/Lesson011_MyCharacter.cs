using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson011_MyCharacter : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 180;

    public Animator _animator;

    int attackHash = Animator.StringToHash("Attack");

	private void Start()
	{
		if (!_animator) {
            Debug.LogError("missing Animator");
        }
	}

	void Update()
    {
        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (_animator) {
                _animator.SetTrigger(attackHash);
            }
        }
    }

    void UpdateMovement() {
        float moveDir = 0;
        float rotAmount = 0;

        if (Input.GetKey(KeyCode.W)) moveDir += 1;
        if (Input.GetKey(KeyCode.S)) moveDir -= 1;

        if (Input.GetKey(KeyCode.A)) rotAmount -= 1;
        if (Input.GetKey(KeyCode.D)) rotAmount += 1;

        float aniSpeed = 0;

        if (moveDir != 0) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                aniSpeed = 1;
            } else {
                aniSpeed = 0.5f;
            }

            transform.Translate(0, 0, moveDir * aniSpeed * _speed * Time.deltaTime);
        }

        if (_animator) {
            _animator.SetFloat("Speed", aniSpeed);
        }

        if (rotAmount != 0) {
            transform.Rotate(0, rotAmount * _rotateSpeed * Time.deltaTime, 0);
        }
    }
}
