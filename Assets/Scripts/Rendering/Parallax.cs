using UnityEngine;

public class Parallax : MonoBehaviour
{
	public Transform cam;
	public float distance = 1.0f;

	private Vector3 startCamPos, startPos;

	void Start()
	{
		if (cam == null)
			cam = Camera.main.transform;

		startPos = transform.position;
		startCamPos = cam.transform.position;
	}

	void LateUpdate()
	{
		var newPosition = startPos + (cam.position - startCamPos) * distance;
		transform.position = new Vector3(newPosition.x, newPosition.y, startPos.z);
	}
}
