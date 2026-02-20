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


	public Texture terrainTexture;
	public Texture2D boneMap;
	public Texture2D backgroundTextureMask;

	public PlanetDescriptor debugPlanetGen;

	public void Apply(Dictionary<Vector3Int, int> materials, int size)
	{
		if (Materials.materials == null)
		{
			Materials.Load();
		}

		tileMap.ClearAllTiles();

		backgroundTextureMask = new Texture2D(size, size, TextureFormat.RGBA32, false);

		var tex = new Texture2D(size, size, TextureFormat.R16, false, true);
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;

		//var tilePositions = new List<Vector3Int>();

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
		backgroundTextureMask.Apply();

		Shader.SetGlobalTexture("_AsteroidBGMask", backgroundTextureMask);

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

		if (GlobalEvents.Instance != null)
			GlobalEvents.Instance.OnNewMapGenerated?.Invoke(materials);

		//DestructibleTerrain.Instance.ApplyNewMap(materials);
	}

	void Update()
	{
		if (Global.Instance != null && Global.Instance.loadPlanet != null)
		{
			Global.Instance.loadPlanet.GenerateWorld(Random.Range(0, 999999), out var mats, out var size, this);
			Apply(mats, size);

			Global.Instance.loadPlanet = null;
		}

		if (refresh)
		{
			//Generate(Random.Range(0, 999999), 64, out var materials);
			if (debugPlanetGen != null)
			{
				debugPlanetGen.GenerateWorld(Random.Range(0, 999999), out var mats, out var size, this);
				Apply(mats, size);
			}
			else
				Debug.LogWarning("no debug world defined.");

			refresh = false;
		}
	}
}
