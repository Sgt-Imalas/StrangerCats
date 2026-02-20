using UnityEngine;

public class UISpinner : MonoBehaviour
{
	[SerializeField] private float frequency = 3f;      // time for one rotation

	private Vector3 startPosition;

	private void Start()
	{
		startPosition = transform.localPosition;
	}

	private void Update()
	{
		transform.localRotation = Quaternion.AngleAxis(Time.time * 360f / frequency, Vector3.forward);
	}
}
