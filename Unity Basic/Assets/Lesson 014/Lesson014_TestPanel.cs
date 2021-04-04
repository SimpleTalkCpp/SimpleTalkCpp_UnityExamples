using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lesson014_TestPanel : MonoBehaviour
{
	public Dropdown testDropdown;

	private void Start()
	{
		if (!testDropdown)
		{
			Debug.LogError("missing testDropdown");
		}
		else
		{
			testDropdown.AddOptions(new List<string> { "Test1", "Test2" });
			testDropdown.onValueChanged.AddListener(OnDropdownChanged);
		}
	}

	public void OnTestButtonClick() {
		Debug.Log("OnTestButtonClick");
	}

	public void OnDropdownChanged(int v) {
		Debug.Log($"OnDropdownChanged {v}");
	}

	private void OnGUI()
	{
//		Debug.Log($"Frame[{Time.frameCount}] OnGUI Event={Event.current.type}");
	}
}
