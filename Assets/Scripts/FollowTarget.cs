using UnityEngine;

public class FollowTarget : MonoBehaviour
{
	public Transform target;
	public float maxDistanceDelta = 10.0f;

	void Update()
	{
		if (target == null)
			return;

		var targetPos = target.transform.position;
		targetPos.z = this.transform.position.z;

		this.transform.position = Vector3.Lerp(transform.position, targetPos, maxDistanceDelta);
	}
}
