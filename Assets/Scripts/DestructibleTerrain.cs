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

	public Dictionary<Vector3Int, float> queuedToDamage;
	public HashSet<Vector3Int> queuedToDestroy;
	private bool dirty;
	public float updateFrequency = 0.05f;

	public Dictionary<Vector3Int, MaterialData> materials;

	private float elapsed = 0;

	public static DestructibleTerrain Instance;

	public Vector3 GetTileCenter(Vector3 pos)
	{
		var cell = tileMap.WorldToCell(new Vector3(pos.x, pos.y));
		return tileMap.GetCellCenterWorld(cell);
	}

	void Awake()
	{

		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}
	void Start()
	{
		GlobalEvents.Instance.OnNewMapGenerated += OnNewMapGenerated;
		queuedToDamage = new Dictionary<Vector3Int, float>();
		queuedToDestroy = new HashSet<Vector3Int>();
		materials = new Dictionary<Vector3Int, MaterialData>();

	}

	private void OnNewMapGenerated(Dictionary<Vector3Int, int> materials)
	{
		this.materials.Clear();

		foreach (var mat in materials)
		{
			var material = Materials.GetMaterial(mat.Value);
			if (material == null)
			{
				Debug.LogWarning($"Not a valid element {mat.Value}");
				continue;
			}

			this.materials[mat.Key] = new MaterialData()
			{
				idx = material.hash,
				hp = material.hardness,
				currentHp = material.hardness,
				particleColor = material.particlesColor
			};
		}

		queuedToDamage.Clear();
		queuedToDestroy.Clear();

		elapsed = 0.0f;
		dirty = false;
	}

	public class MaterialData
	{
		public int idx;
		public float hp;
		public float currentHp;
		public Color particleColor;
	}

	void Update()
	{
		if (dirty)
		{
			foreach (var tile in queuedToDamage)
			{
				if (!materials.TryGetValue(tile.Key, out var mat))
				{
					Debug.LogWarning($"trying to damage a tile that doesn't exist {tile.Key}");
					continue;
				}

				mat.currentHp -= tile.Value;

				if (mat.currentHp <= 0.0f)
					queuedToDestroy.Add(tile.Key);
				else
				{
					var idx = (int)((crackTile.Length) * (1.0f - (mat.currentHp / mat.hp)));
					idx = Mathf.Clamp(idx, 0, crackTile.Length - 1);
					cracksTileMap.SetTile(tile.Key, crackTile[idx]);
				}

				digParticles.Configure(mat.particleColor, 1, 1);
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
					cracksTileMap.SetTile(cell, null);
				}

				tileMap.SetTiles(data, false);
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

		var insetHitPosition = point - normal * -0.1f;

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

	internal void DamageTileAt(Vector2 pos, float damage, int radius = 0)
	{
		var cell = tileMap.WorldToCell(pos);
		if (tileMap.HasTile(cell))
		{
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
			else
			{

				queuedToDamage.Add(cell, damage);
			}

			dirty = true;
		}

	}

	internal void ApplyNewMap(Dictionary<Vector3Int, int> materials)
	{
		OnNewMapGenerated(materials);
	}
}
