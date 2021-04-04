using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson013_TestAttribute : PropertyAttribute
{
	public string message;
}

[System.Serializable]
public class Lesson013_TestClass {
	public float child0;
	public float child1;
	public float child2;
}

public class Lesson013_TestComponent : MonoBehaviour
{
	[Lesson013_Test(message = "Hello World")]
	public int testInt = 0;

	public string testString;

	public Lesson013_TestClass testClass;

	public int lastValue;
}