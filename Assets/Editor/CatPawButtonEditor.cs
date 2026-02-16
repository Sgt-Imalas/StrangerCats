using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CatPawButton))]
public class CatPawButtonEditor : ButtonEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();

		SerializedProperty catPawImage =
			serializedObject.FindProperty("CatPawImage");

		EditorGUILayout.PropertyField(catPawImage);

		serializedObject.ApplyModifiedProperties();
	}
}
