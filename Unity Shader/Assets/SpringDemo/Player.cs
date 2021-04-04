using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public float speed = 10;
	// Use this for initialization
	void Start () {
		
	}
	
	void Update() {
		var v = new Vector3();
		if (Input.GetKey("w")) v.z += 1;
		if (Input.GetKey("s")) v.z -= 1;
		if (Input.GetKey("a")) v.x -= 1;
		if (Input.GetKey("d")) v.x += 1;

		transform.Translate(v * Time.fixedDeltaTime * speed, Space.Self);
	}
}
