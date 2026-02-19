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

	public Dictionary<Vector3Int, float> queuedToDamage;
	public HashSet<Vector3Int> queuedToDestroy;
	private bool dirty;
	public float updateFrequency = 0.05f;

	private float elapsed = 0;

	private Dictionary<Vector3Int, float> damageValues;

	void Start()
	{
		queuedToDamage = new Dictionary<Vector3Int, float>();
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
				if (damageValues.TryGetValue(tile.Key, out var damage))
					existingDamage += damage;

				existingDamage += tile.Value;

				if (existingDamage > 1.0f)
					queuedToDestroy.Add(tile.Key);
				else
				{
					damageValues[tile.Key] = existingDamage;

					var idx = (int)((crackTile.Length) * existingDamage);
					idx = Mathf.Clamp(idx, 0, crackTile.Length - 1);
					cracksTileMap.SetTile(tile.Key, crackTile[idx]);
					cracksTileMap.SetColor(tile.Key, new Color(1.0f, 1.0f, 1.0f, existingDamage));
				}

				digParticles.transform.position = tile.Key;
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
			var radius = (int)attributes.Get(AttributeType.ExplosionRadius);
			var damage = attributes.Get(AttributeType.DigDamage);
			if (radius > 0)
			{
				// TODO: manhattan or smth
				for (var x = -radius; x < radius; x++)
				{
					for (var y = -radius; y < radius; y++)
					{
						queuedToDamage.Add(cell + new Vector3Int(x, y), damage);
					}
				}
			}
		}
		else
			queuedToDamage.Add(cell, 1.0f);

		dirty = true;
	}
}
