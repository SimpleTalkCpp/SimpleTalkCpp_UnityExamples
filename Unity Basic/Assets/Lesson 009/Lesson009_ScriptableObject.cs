using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lesson009_ScriptableObject_DefaultName", menuName = "Lesson009/Create ScriptableObjects", order = 10000)]
public class Lesson009_ScriptableObject : ScriptableObject
{
	public float hp;
	public float mp;

	public Color gizmosColor = Color.blue;
}
