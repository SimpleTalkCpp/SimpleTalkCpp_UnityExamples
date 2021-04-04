using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson015_LegacyInputManager : MonoBehaviour
{
	private void OnGUI()
	{
		GUI.Box( new Rect(10, 10, 300, 100),
				"== Lesson015_LegacyInputManager ==\n"
				+ $"MousePos = {Input.mousePosition}\n"
				+ $"Vertical = {Input.GetAxis("Vertical")}\n"
				+ $"Fire1 = {Input.GetAxis("Fire1")}\n"
				+ $"Fire1 Button = {Input.GetButton("Fire1")}");
    }
}
