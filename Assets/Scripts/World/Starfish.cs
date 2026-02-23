using UnityEngine;

[ExecuteAlways]
public class Starfish : MonoBehaviour
{
	public Transform body;
	public Transform eye;
	public LineRenderer laser;

	public float idleRotationSpeed = -0.69f;
	public float engagedRotationpeed = -13f;

	float targetRotationSpeed;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
	}
}
