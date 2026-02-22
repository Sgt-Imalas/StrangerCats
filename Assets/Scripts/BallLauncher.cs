using UnityEngine;

public class BallLauncher : MonoBehaviour
{
	public GameObject ballPrefab;
	public Vector2 angle;
	public float force;
	public float interval;
	public Transform marker;

	private float lastShot;

	void Shoot()
	{

		var ball = Object.Instantiate(ballPrefab);

		ball.transform.position = marker.transform.position;

		var rb = ball.GetComponent<Rigidbody2D>();

		rb.AddForce(angle * force);

	}

	void Update()
	{
		lastShot += Time.deltaTime;

		if (lastShot > interval)
		{
			Shoot();
			lastShot = 0;
		}
	}
}
