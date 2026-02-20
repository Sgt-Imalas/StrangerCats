using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceTopBar))]
public class ResourceTopBarEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ResourceTopBar myScript = (ResourceTopBar)target;

		if (GUILayout.Button("Collect Random Resource"))
		{
			myScript.CollectRandomResource();
		}
	}
}