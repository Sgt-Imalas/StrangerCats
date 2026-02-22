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
	public AudioClip deathSound, slideSound, hitSound;
	public float slideCoolDown = 4.0f;
	public LayerMask layerMask;
	public ResourceType resourceType = ResourceType.Meat;

	public PlayerDetection detection;

	public Transform overrideTarget;
	public Transform body;

	private Rigidbody2D rb;

	private float lastSlide = 0.0f;
	private bool isChasing;
	public float chaseRange = 7.0f;

	public float slideForce = 10.0f;
	public float idleSlideForce = 5.0f;

	private Vector3 targetVelocity;

	public bool starfish;

	bool isFollowingPlayer;

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
		if (animator != null)
			animator.Play("cell enemy");
		rb = GetComponent<Rigidbody2D>();

		GetComponent<Health>().OnDeath += OnDeath;
	}

	private void OnDeath()
	{
		audioSource.pitch = Random.Range(0.9f, 1.1f);
		audioSource.Play();
		deathSplat.Emit(20);
		if (animator != null)
			animator.gameObject.SetActive(false);
		GetComponent<CircleCollider2D>().enabled = false;

		StartCoroutine(DieLater());

		GlobalEvents.Instance.OnEnemyKilled?.Invoke(transform.position, resourceType);
	}


	private IEnumerator DieLater()
	{
		yield return _waitForSecondsRealtime1_0;

		Object.Destroy(gameObject);
	}

	private void TrySlide()
	{
		isFollowingPlayer = false;

		if (detection.currentPlayer != null)
		{
			var dir = (Vector2)(detection.currentPlayer.transform.position - transform.position);

			var hit = Physics2D.Raycast(
						transform.position,
						dir,
						16f,
						layerMask
					);

			Debug.DrawLine(transform.position, transform.position + (Vector3)dir.normalized * 16.0f);
			if (hit.collider != null && (hit.collider.CompareTag("Player")))
			{
				//rb.AddForce(dir.normalized * slideForce);
				targetVelocity = dir.normalized * slideForce;
				isFollowingPlayer = true;
			}

		}

		if (!isFollowingPlayer && lastSlide > slideCoolDown)
		{
			var direction = (Vector3)Random.insideUnitCircle.normalized;
			//rb.AddForce(direction * idleSlideForce);
			targetVelocity = direction * idleSlideForce;

			lastSlide = 0.0f;
		}

		rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 0.5f);
	}


	private void FixedUpdate()
	{
		lastSlide += Time.fixedDeltaTime;

		//
		TrySlide();
		//}



		if (body != null)
			body.Rotate(Vector3.forward, isFollowingPlayer ? 3f : 10f);
		else
		{
			var angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
		}
	}
}
