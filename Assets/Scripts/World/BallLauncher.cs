using UnityEngine;

public class BallLauncher : MonoBehaviour
{
	public GameObject ballPrefab;
	public Vector2 angle;
	public float force;
	public float interval;
	public Transform marker;
	[SerializeField] private float _offset;

	private Animator _animator;
	private AudioSource _audioSource;

	void Start()
	{
		_animator = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();

		_animator.Play("Idle");
	}

	public void TriggerManually()
	{
		_animator.SetTrigger("Shoot");
	}

	private void Shoot()
	{
		var ball = Object.Instantiate(ballPrefab);

		ball.transform.position = marker.transform.position;

		var rb = ball.GetComponent<Rigidbody2D>();

		rb.AddForce((Vector2)(transform.rotation * angle) * force);

		_audioSource.pitch = Random.Range(0.9f, 1.1f);
		_audioSource.Play();

	}
}
