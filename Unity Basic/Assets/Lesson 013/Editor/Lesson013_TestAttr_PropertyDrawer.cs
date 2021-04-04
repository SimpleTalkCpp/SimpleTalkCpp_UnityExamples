using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Lesson013_TestAttribute), true)]
public class Lesson013_TestAttr_PropDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		var buttonRect = position;
		buttonRect.width  = 50;
		buttonRect.x = position.xMax - buttonRect.width;

		if (GUI.Button(buttonRect, "+1")) {
			var msg = new StringBuilder();
			msg.Append("-- +1 Button --\n");
			msg.Append($"path = [{prop.propertyPath}]\n");
			msg.Append($"type = [{prop.type}]\n");
			msg.Append($"hasMultipleDifferentValues = {prop.hasMultipleDifferentValues}]\n");

			msg.Append($"NicifyVariableName = [{ObjectNames.NicifyVariableName("m_testABC")}\n");

			var attr = this.attribute as Lesson013_TestAttribute;
			if (attr != null) {
				msg.Append($"Attribute message = [{attr.message}]\n");
			}

			if (prop.type == "int") {
				prop.intValue = prop.intValue + 1;
			}

			Debug.Log(msg.ToString());
		}

		position.width -= buttonRect.width;
		EditorGUI.PropertyField(position, prop, true);
	}
}