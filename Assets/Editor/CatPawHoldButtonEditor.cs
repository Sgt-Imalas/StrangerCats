using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(HoldButton))]
public class CatPawHoldButtonEditor : CatPawButtonEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();

		SerializedProperty fillImage =
			serializedObject.FindProperty("FillImage");

		EditorGUILayout.PropertyField(fillImage);

		serializedObject.ApplyModifiedProperties();
	}
}
