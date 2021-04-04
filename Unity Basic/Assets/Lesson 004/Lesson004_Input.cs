using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Lesson004_Input : MonoBehaviour
{
    public float _speed = 5;
    public float _rotateSpeed = 180;

    // Update is called once per frame
    void Update()
    {
        float moveDir = 0;
        float rotAmount = 0;

        if (Input.GetKey(KeyCode.W)) moveDir += 1;
        if (Input.GetKey(KeyCode.S)) moveDir -= 1;

        if (Input.GetKey(KeyCode.A)) rotAmount -= 1;
        if (Input.GetKey(KeyCode.D)) rotAmount += 1;

        if (moveDir != 0) {
            transform.Translate(0, 0, moveDir * _speed * Time.deltaTime, Space.Self);
        }

        if (rotAmount != 0) {
            transform.Rotate(0, rotAmount * _rotateSpeed * Time.deltaTime, 0);
        }
    }

    public string text = "Please Input ...";

	private void OnGUI()
	{
        float y = 10;
        GUI.color = Color.red;
	    GUI.Box(new Rect(10, y, 200, 80), $" Frame#{Time.frameCount}\n Mouse={Input.mousePosition}\n MouseDown={Input.GetMouseButton(0)}");
        y += 80 + 10;
        GUI.color = Color.white;

        
        if (GUI.Button(new Rect(10, y, 200, 40), "Button")) {
            Debug.Log("Button !!");
        }
        y += 40 + 10;

        text = GUI.TextField(new Rect(10, y, 200, 40), text);
        y += 40 + 10;
	}

	private void OnDrawGizmos()
	{
        Gizmos.matrix = transform.localToWorldMatrix;

        var pt = new Vector3(0,0,2);
        var size = new Vector3(1, 0.5f, 1);

        Gizmos.color = new Color(1,1,0, 0.1f);
		Gizmos.DrawCube(pt, size);

        Gizmos.color = new Color(1,1,0);
		Gizmos.DrawWireCube(pt, size);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, pt);
	}
}
