using Assets.Scripts;
using UnityEngine;

public class CellEnemy : MonoBehaviour
{
	private Camera mainCamera;
	private PersistentPlayer player;
	private Rigidbody2D rb;
	public Transform sprite;

	public PlayerTracker playerTracker;

	public float acceleration = 50.0f;
	public float speed = 50.0f;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		mainCamera = Camera.main;
		player = PersistentPlayer.Instance;
	}

	private bool CanSeePlayer()
	{
		var hitP = Physics2D.Raycast(player.transform.position, Vector2.zero, 0.0f);

		if (hitP.collider != null)
		{
			Debug.Log(hitP.collider.name);
		}

		return true;
	}

	void Update()
	{
		if (playerTracker == null)
			return;

		if (player == null)
			player = PersistentPlayer.Instance;

		if (playerTracker.isInRange) // && CanSeePlayer())
		{
			var movementDirection = (player.transform.position - transform.position).normalized;
			rb.AddForce(acceleration * rb.mass * movementDirection);

			if (rb.linearVelocity.magnitude > speed)
			{
				rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speed);
			}

			transform.LookAt(player.transform.position, Vector3.up);
		}
	}
}
