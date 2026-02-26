using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class MapGenerator : MonoBehaviour
{
	private FastNoiseLite noise;
	public Tilemap tileMap;
	public TileBase terrainTile;
	public bool refresh;
	public Transform asteroidBg;

	public Texture terrainTexture;
	public Texture2D boneMap;

	public PlanetDescriptor debugPlanetGen;

	public bool generateEnemies = false;
	public GroundRadarTarget RadarTargetPrefab;

	public Volume volume;
	public Light2D globalLight;

	public void Apply(Dictionary<Vector3Int, int> materials, Vector2 center, int size, PlanetDescriptor descriptor)
	{
		if (Materials.materials == null)
		{
			Materials.Load();
		}

		if (Global.Instance != null && Global.Instance.entities != null && !Application.isEditor)
		{
			for (var i = Global.Instance.entities.Count - 1; i >= 0; i--)
			{
				var entity = Global.Instance.entities[i];
				Object.Destroy(entity);
			}
		}

		tileMap.ClearAllTiles();

		var tex = new Texture2D(size, size, TextureFormat.R16, false, true);
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;

		//var tilePositions = new List<Vector3Int>();

		if (generateEnemies && (descriptor.enemyPreset != null || descriptor.gizmoPrefab != null))
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

		SpawnRadarPointer(tileMap, center, size);

		if (asteroidBg != null)
		{
			for (var i = asteroidBg.childCount - 1; i >= 0; --i)
			{
				var child = asteroidBg.GetChild(i).gameObject;
				if (Application.isEditor)
					DestroyImmediate(child);
				else
					Destroy(child);
			}

			if (descriptor.backGround != null)
			{
				asteroidBg.gameObject.SetActive(true);
				var bg = Instantiate(descriptor.backGround, asteroidBg);
				bg.SetActive(true);
				bg.transform.position = center;
			}
			else
			{
				asteroidBg.gameObject.SetActive(false);
			}
		}

		var profile = volume.profile;

		if (volume.profile.TryGet(out Bloom bloom))
		{
			bloom.tint.overrideState = true;
			bloom.tint.value = descriptor.postFxBloomTint;
		}

		globalLight.color = descriptor.globalLightColor;
		globalLight.intensity = descriptor.glovalLightIntensity;

		if (GlobalEvents.Instance != null)
			GlobalEvents.Instance.OnNewMapGenerated?.Invoke(materials, descriptor);

		//DestructibleTerrain.Instance.ApplyNewMap(materials);
	}

	private void SpawnRadarPointer(Tilemap tilemap, Vector2 center, int size)
	{
		if (RadarTargetPrefab == null)
		{
			Debug.LogWarning(name + " tried spawning a radar target, but the prefab was null!");
			return;
		}


		//RadarController.AddPointer
		var radarTarget = Object.Instantiate(RadarTargetPrefab);
		radarTarget.transform.position = tileMap.CellToWorld(new((int)center.x, (int)center.y, 0)) + new Vector3(0.5f, 0);
		radarTarget.CutoffDistanceThreshold = size / 2 + 8;
		radarTarget.gameObject.SetActive(true);
	}

	bool ShouldSpawnGizmo(PlanetDescriptor descriptor)
	{
		if (descriptor == null)
			return false;
		if (descriptor.gizmoPrefab == null)
			return false;

		switch (descriptor.resourceType)
		{
			case ResourceType.Meat:
				return !Global.Instance.Upgrades.MeatWorldItemFound;
			case ResourceType.Rust:
				return !Global.Instance.Upgrades.RadarUnlocked;
			case ResourceType.Ball:
				return !Global.Instance.Upgrades.TennisWorldItemFound;
			case ResourceType.Dust:
				return !Global.Instance.Upgrades.DesertWorldItemFound;
		}
		return true;
	}

	private void SpawnEnemies(Dictionary<Vector3Int, int> materials, Tilemap tilemap, Vector2 center, int size, PlanetDescriptor descriptor)
	{
		var enemiesSpawned = 0;
		var attempts = 255;

		HashSet<Vector3Int> claimedPositions = new();

		if (ShouldSpawnGizmo(descriptor))
		{
			var randomSpot = 0.35f * size * Random.insideUnitCircle;
			randomSpot += center;
			var position = new Vector3Int((int)randomSpot.x, (int)randomSpot.y);

			if (descriptor.generationPreset == PlanetDescriptor.GenerationPreset.Plastic)
				position = new Vector3Int((int)center.x, (int)center.y); ;

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

		if (descriptor.enemyPreset != null)
		{

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
						enemy.SetActive(true);

						enemy.GetComponent<ISpawnRules>().ConfigureSpawn(position, materials, claimedPositions, data);
						Global.Instance.entities.Add(enemy);

						enemiesSpawned++;
					}
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
