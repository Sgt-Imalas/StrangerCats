using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class DestructibleTerrain : MonoBehaviour
{
	Tilemap tileMap;
	public float penetrationTest = -0.1f;

	public HashSet<Vector3Int> queuedToDamage;
	private bool dirty;

	void Start()
	{
		tileMap = GetComponent<Tilemap>();
		queuedToDamage = new HashSet<Vector3Int>();
	}

	void Update()
	{
		if (dirty)
		{
			foreach (var cell in queuedToDamage)
			{
				tileMap.SetTile(cell, null);
			}

			tileMap.RefreshAllTiles();
			tileMap.CompressBounds();

			dirty = false;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var point = collision.GetContact(0).point;
		var normal = collision.GetContact(0).normal;

		var insetHitPosition = point - normal * penetrationTest;

		var cell = tileMap.WorldToCell(insetHitPosition);

		if (collision.collider.TryGetComponent(out Attributes attributes))
		{
			var radius = (int)attributes.Get(Assets.Scripts.AttributeType.ExplosionRadius);
			if (radius > 0)
			{
				// TODO: manhattan or smth
				for (var x = -radius; x < radius; x++)
				{
					for (var y = -radius; y < radius; y++)
					{
						queuedToDamage.Add(cell + new Vector3Int(x, y));
					}
				}
			}
		}

		queuedToDamage.Add(cell);
		dirty = true;
	}
}
