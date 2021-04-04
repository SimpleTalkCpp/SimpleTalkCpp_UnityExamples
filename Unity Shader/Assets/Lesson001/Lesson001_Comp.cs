using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson001_Comp : MonoBehaviour {

	public Material mat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (mat != null) {
			mat.SetFloat("my_time", Time.time);
		}
	}
}
