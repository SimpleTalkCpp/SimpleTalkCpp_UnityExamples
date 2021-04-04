using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson005 : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 180;

    public float fireCooldown = 0.1f;
    float _fireCooldownRemain;

    public float trailDampingTime = 0.5f;
    public int trailCount = 5;
    public Lesson005_Trail trailPrefab;

	private void Start()
	{
		if (trailPrefab) {
            GameObject lastObject = gameObject;

            for (int i = 0; i < trailCount; i++) {
                var obj = Instantiate(trailPrefab.gameObject);
                obj.name = $"Trail {i}";
                var comp = obj.GetComponent<Lesson005_Trail>();
                if (!comp) {
                    Debug.LogError("Cannot find Lesson005_Trail");
                    continue;
                }

                comp.target = lastObject;
                comp.dampingTime = trailDampingTime;

                lastObject = obj;
            }
        }
	}

	void Update()
    {
        if (_fireCooldownRemain > 0) {
            _fireCooldownRemain -= Time.deltaTime;
        }

        UpdateMovement();

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            Fire();
        }
    }
	private void OnDrawGizmos()
	{
		MyGizmos.Label(transform.position + new Vector3(0, 2, 0), $"CD={_fireCooldownRemain}");
	}

	void Fire() {
        if (_fireCooldownRemain > 0) return;
        _fireCooldownRemain = fireCooldown;

        var obj = new GameObject("Bullet");
        var bullet = obj.AddComponent<Lesson005_Bullet>();

        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
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
}
