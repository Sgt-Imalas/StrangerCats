using UnityEngine;

public class TextStabilizer : MonoBehaviour
{

	RectTransform parent;
	void Start()
	{
		parent = transform.parent.GetComponent<RectTransform>();
	}

	void LateUpdate()
	{
		if (parent == null) return;

		float parentRotation = parent.eulerAngles.z;
		transform.localRotation = Quaternion.Euler(0f, 0, -parentRotation);
	}
}
