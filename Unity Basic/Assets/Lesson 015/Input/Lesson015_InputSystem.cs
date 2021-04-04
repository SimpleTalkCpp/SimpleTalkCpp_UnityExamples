using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson015_InputSystem : MonoBehaviour {

	UnityEngine.InputSystem.PlayerInput m_playerInput;
	
	private void Start()
	{
		var kb = UnityEngine.InputSystem.Keyboard.current;
		//kb.onTextInput += Kb_onTextInput;
	}

	private void Kb_onTextInput(char obj)
	{
		Debug.Log("Kb_onTextInput " + obj);
	}

	void OnGUI()
    {
		var mouse = UnityEngine.InputSystem.Mouse.current;
		var kb = UnityEngine.InputSystem.Keyboard.current;

		GUI.Box( new Rect(10, 120, 300, 100),
				"== Lesson015_InputSystem ==\n"
				+ $"MousePos = {mouse.position.ReadValue()}\n"
				+ $"Down = {kb.downArrowKey.isPressed}\n"
				+ $"Ctrl Key = {kb.leftCtrlKey.isPressed}\n");
    }
}
