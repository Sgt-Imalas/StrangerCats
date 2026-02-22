using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class MapGenerator : MonoBehaviour
{
	private FastNoiseLite noise;
	public Tilemap tileMap;
	public TileBase terrainTile;
	public bool refresh;
	public MeshRenderer asteroidBg;

	public Texture terrainTexture;
	public Texture2D boneMap;
	public Texture2D backgroundTextureMask;

	public PlanetDescriptor debugPlanetGen;

	public bool generateEnemies = false;

	public void Apply(Dictionary<Vector3Int, int> materials, Vector2 center, int size, PlanetDescriptor descriptor)
	{
		if (Materials.materials == null)
		{
			Materials.Load();
		}

		tileMap.ClearAllTiles();

		var tex = new Texture2D(size, size, TextureFormat.R16, false, true);
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;

		//var tilePositions = new List<Vector3Int>();

		if (generateEnemies && descriptor.enemyPreset != null)
			SpawnEnemies(materials, tileMap, center, size, descriptor);

		foreach (var mat in materials)
		{
			var coords = mat.Key;

			tileMap.SetTile(mat.Key, terrainTile);


			var terrain = Materials.GetMaterial(mat.Value);
			if (terrain == null)
			{
				Debug.LogWarning("material is null");
				continue;
			}

			tex.SetPixel(coords.x, coords.y, new Color32(terrain.textureIdx, 0, 0, 255));
		}

		tex.Apply(false, true);

		//backgroundTextureMask = new Texture2D(size, size, TextureFormat.RGBA32, false);

		//backgroundTextureMask.Apply();

		//Shader.SetGlobalTexture("_AsteroidBGMask", backgroundTextureMask);

		//tileMap.SetTiles(tilePositions.ToArray(), new TileBase[] { terrainTile });



		tileMap.CompressBounds();

		var bounds = tileMap.localBounds;

		var min = Vector3.zero;
		var max = new Vector3(size, size, 0);

		var material = tileMap.GetComponent<TilemapRenderer>().material;

		// dont have to worry about adding more later, we assume we can only destroy, not place
		material.SetTexture("_TerrainIndex", tex);

		Shader.SetGlobalVector("_TilemapMin", new Vector4(min.x, min.y, 0, 0));
		Shader.SetGlobalVector("_TilemapMax", new Vector4(max.x, max.y, 0, 0));

		if (asteroidBg != null)
		{
			if (descriptor.bg != null)
			{
				asteroidBg.gameObject.SetActive(true);
				asteroidBg.material.mainTexture = descriptor.bg;
			}
			else
			{
				asteroidBg.gameObject.SetActive(false);
			}
		}

		if (GlobalEvents.Instance != null)
			GlobalEvents.Instance.OnNewMapGenerated?.Invoke(materials);

		//DestructibleTerrain.Instance.ApplyNewMap(materials);
	}

	private void SpawnEnemies(Dictionary<Vector3Int, int> materials, Tilemap tilemap, Vector2 center, int size, PlanetDescriptor descriptor)
	{
		var enemiesSpawned = 0;
		var attempts = 255;

		HashSet<Vector3Int> claimedPositions = new();

		if (descriptor.gizmoPrefab != null)
		{
			var randomSpot = 0.35f * size * Random.insideUnitCircle;
			randomSpot += center;
			var position = new Vector3Int((int)randomSpot.x, (int)randomSpot.y);

			if (descriptor.gizmoPrefab.TryGetComponent(out ISpawnRules rules))
			{
				if (rules.CanSpawnHere(position, materials, out var data))
				{
					var enemy = Object.Instantiate(descriptor.gizmoPrefab);
					enemy.transform.position = tileMap.CellToWorld(position) + new Vector3(0.5f, 0);
					enemy.gameObject.SetActive(true);

					enemy.GetComponent<ISpawnRules>().ConfigureSpawn(position, materials, claimedPositions, data);
					Global.Instance.entities.Add(enemy);
				}
			}
		}

		while (attempts-- > 0 && enemiesSpawned < descriptor.enemiesToSpawn)
		{
			var randomSpot = 0.35f * size * Random.insideUnitCircle;
			randomSpot += center;
			var position = new Vector3Int((int)randomSpot.x, (int)randomSpot.y);

			if (claimedPositions.Contains(position))
				continue;

			claimedPositions.Add(position);

			if (descriptor.enemyPreset.TryGetComponent(out ISpawnRules rules))
			{
				if (rules.CanSpawnHere(position, materials, out var data))
				{
					var enemy = Object.Instantiate(descriptor.enemyPreset);
					enemy.transform.position = tileMap.CellToWorld(position) + new Vector3(0.5f, 0);
					enemy.gameObject.SetActive(true);

					enemy.GetComponent<ISpawnRules>().ConfigureSpawn(position, materials, claimedPositions, data);
					Global.Instance.entities.Add(enemy);
				}
			}
		}
	}

	void Update()
	{
		if (Global.Instance != null && Global.Instance.loadPlanet != null)
		{
			Global.Instance.loadPlanet.GenerateWorld(Random.Range(0, 999999), out var mats, out var size, out var center, this);
			Apply(mats, center, size, Global.Instance.loadPlanet);

			Global.Instance.loadPlanet = null;
		}

		if (refresh)
		{
			//Generate(Random.Range(0, 999999), 64, out var materials);
			if (debugPlanetGen != null)
			{
				debugPlanetGen.GenerateWorld(Random.Range(0, 999999), out var mats, out var size, out var center, this);
				Apply(mats, center, size, debugPlanetGen);
			}
			else
				Debug.LogWarning("no debug world defined.");

			refresh = false;
		}
	}
}
