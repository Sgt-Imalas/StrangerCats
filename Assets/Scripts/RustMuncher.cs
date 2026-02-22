using Assets.Scripts;
using UnityEngine;

public class RustMuncher : MonoBehaviour
{
	public Animator animator;
	public Vector3Int homeTile;

	void Start()
	{
		animator.SetFloat("Offset", Random.Range(0.0f, 1.0f));
		GlobalEvents.Instance.OnTileDestroyed += OnTileDestroyed;
		animator.Play("rust muncher idle");
	}

	private void OnTileDestroyed(Vector3Int coords, int _)
	{
		if (coords == homeTile)
		{
			Die(false);
		}
	}

	private void Die(bool byPlayer)
	{
		Object.Destroy(gameObject);
	}

	internal void SetHome(Vector3Int cell, Vector3Int offset)
	{
		homeTile = cell;
		if (offset == Vector3Int.up)
		{
			transform.Rotate(Vector3.forward, 180.0f);
			//transform.position += (Vector3)offset * 1.0f;
		}
		else if (offset == Vector3Int.left)
		{
			transform.Rotate(Vector3.forward, -90.0f);
			//transform.position += (Vector3)offset * 0.5f;
		}
		else if (offset == Vector3Int.right)
		{
			transform.Rotate(Vector3.forward, 90.0f);
			//transform.position += (Vector3)offset * 0.5f;
		}
	}
}
