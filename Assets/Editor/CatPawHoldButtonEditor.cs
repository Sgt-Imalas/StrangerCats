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

		SerializedProperty holdTime =
			serializedObject.FindProperty("holdTime");

		EditorGUILayout.PropertyField(holdTime);

		SerializedProperty ReleaseToTrigger = serializedObject.FindProperty("ReleaseToTrigger");
		EditorGUILayout.PropertyField(ReleaseToTrigger);

		SerializedProperty BuyingProhibitedNotification = serializedObject.FindProperty("BuyingProhibitedNotification");
		EditorGUILayout.PropertyField(BuyingProhibitedNotification);


		
		serializedObject.ApplyModifiedProperties();
	}
}
