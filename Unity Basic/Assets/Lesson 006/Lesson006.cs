using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson006 : MonoBehaviour
{
    public float _speed = 20;

    Vector3 _lastMousePosition;
    public float mouseSensitivity = 0.1f;

    public float fireCooldown = 0.1f;
    float _fireCooldownRemain;

    public float bulletVelocity = 50;
    public Vector3 angularVelocity = new Vector3(0, 0, 1000);

    void Update()
    {
        UpdateMouseInput();
        UpdateMovement();
        UpdateFire();
    }

	void UpdateFire() {
        if (_fireCooldownRemain > 0) {
            _fireCooldownRemain -= Time.deltaTime;
            return;
        }

        if (!Input.GetKey(KeyCode.Mouse0))
            return;

        _fireCooldownRemain = fireCooldown;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawLine(ray.origin, ray.GetPoint(100), Color.red, 0.1f);

        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Bullet";

        obj.transform.position = transform.position + ray.direction * 5;
        obj.transform.rotation = transform.rotation;

        Destroy(obj, 5);

        var rb = obj.AddComponent<Rigidbody>();
        rb.velocity = ray.direction * bulletVelocity;
        rb.angularVelocity = transform.rotation * angularVelocity;
    }

    void UpdateMouseInput() {
        if (Input.GetKey(KeyCode.Mouse1)) {
            var deltaPos = Input.mousePosition - _lastMousePosition;

            var e = transform.localRotation.eulerAngles;
            e.x += deltaPos.y * mouseSensitivity;
            e.y -= deltaPos.x * mouseSensitivity;
            transform.localRotation = Quaternion.Euler(e);
        }
        _lastMousePosition = Input.mousePosition;
    }

	private void OnGUI()
	{
		GUILayout.Box("== Input ==\n W: Forward\n S: Backward\n A: Left\n D: Right\n R: Up\n F: Down");
	}

	void UpdateMovement() {
        var moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir.z += 1;
        if (Input.GetKey(KeyCode.S)) moveDir.z -= 1;

        if (Input.GetKey(KeyCode.D)) moveDir.x += 1;
        if (Input.GetKey(KeyCode.A)) moveDir.x -= 1;

        if (Input.GetKey(KeyCode.R)) moveDir.y += 1;
        if (Input.GetKey(KeyCode.F)) moveDir.y -= 1;

        transform.Translate(moveDir * _speed * Time.deltaTime);
    }
}
