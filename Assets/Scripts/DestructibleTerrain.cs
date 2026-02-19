using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTerrain : MonoBehaviour
{
	public Tilemap tileMap;
	public Tilemap cracksTileMap;
	public TileBase[] crackTile;
	public DigParticles digParticles;

	public float penetrationTest = -0.1f;

	public HashSet<Vector3Int> queuedToDamage;
	public HashSet<Vector3Int> queuedToDestroy;
	private bool dirty;
	public float updateFrequency = 0.05f;

	private float elapsed = 0;

	private Dictionary<Vector3Int, float> damageValues;

	void Start()
	{
		queuedToDamage = new HashSet<Vector3Int>();
		queuedToDestroy = new HashSet<Vector3Int>();
		damageValues = new Dictionary<Vector3Int, float>();
	}

	void Update()
	{
		if (dirty)
		{
			foreach (var tile in queuedToDamage)
			{
				var existingDamage = 0.0f;
				if (damageValues.TryGetValue(tile, out var damage))
					existingDamage += damage;

				existingDamage += 0.2f;

				if (existingDamage > 1.0f)
					queuedToDestroy.Add(tile);
				else
				{
					damageValues[tile] = existingDamage;

					var idx = (int)((crackTile.Length) * existingDamage);
					idx = Mathf.Clamp(idx, 0, crackTile.Length - 1);
					cracksTileMap.SetTile(tile, crackTile[idx]);
					cracksTileMap.SetColor(tile, new Color(1.0f, 1.0f, 1.0f, existingDamage));
				}

				digParticles.transform.position = tile;
				digParticles.Emit();
			}


			if (queuedToDestroy.Count > 0)
			{
				var data = new TileChangeData[queuedToDestroy.Count];
				var i = 0;

				foreach (var cell in queuedToDestroy)
				{
					data[i++] = new TileChangeData(cell, null, Color.white, Matrix4x4.identity);
					//tileMap.SetTile(cell, null);
					//tileMap.RefreshTile(cell);
					cracksTileMap.SetTile(cell, null);
				}

				tileMap.SetTiles(data, false);
				//cracksTileMap.SetTiles(data, false);

				//tileMap.RefreshAllTiles();
				//tileMap.CompressBounds();
			}

			cracksTileMap.RefreshAllTiles();

			queuedToDamage.Clear();
			queuedToDestroy.Clear();

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
