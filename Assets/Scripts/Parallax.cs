using UnityEngine;

public class Parallax : MonoBehaviour
{
	public Transform cam;
	Vector2 previousCamPos;

	public Vector3 distance = Vector3.zero;

	void Update()
	{
		return;
		var parallax = (previousCamPos - (Vector2)cam.position) * distance;
		var offset = (Vector2)transform.position + parallax;

		var backgroundTargetPosX = new Vector3(offset.x, offset.y, transform.position.z);

		transform.position = Vector3.Lerp(transform.position, backgroundTargetPosX, Time.deltaTime);

		previousCamPos = cam.position;
	}
}