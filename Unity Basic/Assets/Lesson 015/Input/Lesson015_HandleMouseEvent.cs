using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lesson015_HandleMouseEvent : MonoBehaviour, IPointerDownHandler
{
	//private void OnMouseDown()
	//{
	//	Debug.Log($"OnMouseDown ${gameObject.name}");
	//}

	public void OnPointerDown(PointerEventData ev)
	{
		Debug.Log($"PointerDown ${ev}");
	}
}
