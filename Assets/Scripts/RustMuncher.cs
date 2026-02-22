using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[DefaultExecutionOrder(4000)]
[RequireComponent(typeof(AudioSource))]
public class RustMuncher : MonoBehaviour, ISpawnRules
{
	private static readonly WaitForSecondsRealtime _waitForSecondsRealtime1_0 = new(1.0f);
	public Animator animator;
	public ParticleSystem deathSplat;
	public AudioSource audioSource;
	public Vector3Int homeTile;
	private Vector3Int looking;
	private static readonly Vector3Int[] offsets =
	{
		Vector3Int.up,
		Vector3Int.left,
		Vector3Int.down,
		Vector3Int.right,
	};

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		animator.SetFloat("Offset", Random.Range(0.0f, 1.0f));
		GlobalEvents.Instance.OnTileDestroyed += OnTileDestroyed;
		animator.Play("rust muncher idle");


		GetComponent<Health>().OnDeath += OnDeath;
	}

	private void OnDestroy()
	{
		GlobalEvents.Instance.OnTileDestroyed -= OnTileDestroyed;
	}

	private void OnDeath()
	{
		audioSource.Play();
		deathSplat.Emit(20);
		animator.gameObject.SetActive(false);
		GetComponent<BoxCollider2D>().enabled = false;
		GlobalEvents.Instance.OnTileDestroyed -= OnTileDestroyed;
		StartCoroutine(DieLater());

		GlobalEvents.Instance.OnEnemyKilled?.Invoke(transform.position, ResourceType.Rust);
	}


	private IEnumerator DieLater()
	{
		yield return _waitForSecondsRealtime1_0;

		Object.Destroy(gameObject);
	}

	private void OnTileDestroyed(Vector3Int coords, int _)
	{
		if (coords == homeTile)
		{
			var rb = GetComponent<Rigidbody2D>();
			rb.simulated = true;
			rb.AddForce(-(Vector3)looking * 200.0f);
			rb.AddTorque(10f);
		}
	}

	public bool CanSpawnHere(Vector3Int coord, Dictionary<Vector3Int, int> materials, out object data)
	{
		data = null;

		// not inside tile
		if (materials.ContainsKey(coord))
			return false;

		// adjacent to some other tile
		foreach (var offset in offsets)
		{
			var cell = coord + offset;
			if (materials.ContainsKey(cell))
			{
				data = offset;
				return true;
			}
		}

		return false;
	}

	public void ConfigureSpawn(Vector3Int coord, Dictionary<Vector3Int, int> materials, HashSet<Vector3Int> claimedPositions, object data)
	{
		claimedPositions.Add(coord);
		//Debug.Log($"configuring spawn {((Vector3Int)data)}");

		var offset = (Vector3Int)data;
		homeTile = coord + offset;

		this.looking = offset;

		if (offset == Vector3Int.up)
		{
			transform.Rotate(Vector3.forward, 180.0f);
			transform.position += (Vector3)offset * 1.0f;
		}
		else if (offset == Vector3Int.left)
		{
			transform.Rotate(Vector3.forward, -90.0f);
			transform.position += new Vector3(-0.5f, 0.5f);
		}
		else if (offset == Vector3Int.right)
		{
			transform.Rotate(Vector3.forward, 90.0f);
			transform.position += new Vector3(0.5f, 0.5f);
		}
	}
}
