using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Burglar : MonoBehaviour, ISpawnRules
{
	private static readonly WaitForSecondsRealtime _waitForSecondsRealtime1_0 = new(1.0f);
	public Animator animator;
	public ParticleSystem deathSplat;
	public AudioSource audioSource;
	public AudioClip deathSound, slideSound;
	public AudioClip[] hitSounds;
	public float slideCoolDown = 4.0f;
	public ResourceType resourceType = ResourceType.Meat;



	private Rigidbody2D rb;

	[Header("Chasing Parameters")]
	public Transform overrideTarget;
	public Transform body;
	public LayerMask layerMask;

	public PlayerDetection detection;
	private TagHandle _targetTag;

	public float chaseRange = 7.0f;

	public float slideForce = 10.0f;
	public float idleSlideForce = 5.0f;
	public float movementSmoothing = 0.5f;


	public bool starfish;

	private Vector3 _targetVelocity;
	private bool _isFollowingPlayer;
	private float _lastSlide = 0.0f;
	private Vector2 _lastDamageDir;

	private static readonly int DEAD = Animator.StringToHash("Dead");

	public bool CanSpawnHere(Vector3Int coord, Dictionary<Vector3Int, int> materials, out object data)
	{
		data = null;
		return !materials.ContainsKey(coord);
	}

	public void ConfigureSpawn(Vector3Int coord, Dictionary<Vector3Int, int> materials, HashSet<Vector3Int> claimedPositions, object data)
	{
		claimedPositions.Add(coord);
	}

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		//animator.SetFloat("Offset", Random.Range(0.0f, 1.0f));
		animator = GetComponent<Animator>();
		if (animator != null)
			animator.Play("cell enemy");
		rb = GetComponent<Rigidbody2D>();


		var health = GetComponent<Health>();
		health.OnDeath += OnDeath;
		health.OnHurt += OnHurt;

		_targetTag = TagHandle.GetExistingTag("Player");
	}

	private void OnHurt(bool fatal, Health.DamageInfo data)
	{
		audioSource.clip = hitSounds.GetRandom();
		audioSource.pitch = Random.Range(0.9f, 1.1f);
		audioSource.volume = 0.5f;
		audioSource.Play();

		_lastDamageDir = data.direction;
	}

	public void Pop()
	{
		audioSource.clip = deathSound;
		audioSource.pitch = Random.Range(0.9f, 1.1f);
		audioSource.volume = 0.85f;
		audioSource.Play();

		//var force = deathSplat.forceOverLifetime;
		//var dir = _lastDamageDir.normalized;

		//force.x = dir.x * 5.0f;
		//force.y = dir.y * 5.0f;

		deathSplat.Emit(20);

		StartCoroutine(DieLater());
	}

	private void OnDeath()
	{
		GetComponent<CircleCollider2D>().enabled = false;
		animator.SetBool(DEAD, true);
	}


	private IEnumerator DieLater()
	{
		yield return _waitForSecondsRealtime1_0;

		GlobalEvents.Instance.OnEnemyKilled?.Invoke(transform.position, resourceType);
		Object.Destroy(gameObject);
	}

	private void TrySlide()
	{
		_isFollowingPlayer = false;

		if (detection.currentPlayer != null)
		{
			var dir = (Vector2)(detection.currentPlayer.transform.position - transform.position);

			var hit = Physics2D.Raycast(
						transform.position,
						dir,
						16f,
						layerMask
					);

			if (hit.collider != null && (hit.collider.CompareTag("Player")))
			{
				//rb.AddForce(dir.normalized * slideForce);
				_targetVelocity = dir.normalized * slideForce;
				_isFollowingPlayer = true;
			}

			Debug.DrawLine(transform.position, transform.position + (Vector3)dir.normalized * 16.0f, _isFollowingPlayer ? Color.green : Color.red);

		}

		if (!_isFollowingPlayer && _lastSlide > slideCoolDown)
		{
			var direction = (Vector3)Random.insideUnitCircle.normalized;
			//rb.AddForce(direction * idleSlideForce);
			_targetVelocity = direction * idleSlideForce;

			_lastSlide = 0.0f;
		}

		rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, _targetVelocity, movementSmoothing * Time.fixedDeltaTime);
	}


	private void FixedUpdate()
	{
		_lastSlide += Time.fixedDeltaTime;

		//
		TrySlide();
		//}



		if (body != null)
			body.Rotate(Vector3.forward, _isFollowingPlayer ? 3f : 10f);
		else
		{
			var angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
		}
	}
}
