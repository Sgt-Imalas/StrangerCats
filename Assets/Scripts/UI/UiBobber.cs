using UnityEngine;

public class UiBobber : MonoBehaviour
{
	[SerializeField] private float amplitude = 0.25f;   // How high it moves
	[SerializeField] private float frequency = 3f;      // How fast it moves

	private Vector3 startPosition;

	private void Start()
	{
		startPosition = transform.localPosition;
	}

	private void Update()
	{
		float offset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude;
		transform.localPosition = startPosition + transform.up * offset;
	}
}
