using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
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

	private ObjectPool<Ball> _balls;

	public void SetPool(ObjectPool<Ball> pool)
	{
		_balls = pool;
	}

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
		if (_balls == null)
		{
			Debug.LogError("Pool for ball launcher not configured!");
			return;
		}

		var ball = _balls.Get();

		ball.transform.position = marker.transform.position;
		ball.rb.AddForce((Vector2)(transform.rotation * angle) * force);

		_audioSource.pitch = Random.Range(0.9f, 1.1f);
		_audioSource.Play();

	}
}
