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

	public static MapGenerator Instance;

	void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void Apply(Dictionary<Vector3Int, int> materials, int size)
	{
		if (Materials.Instance == null)
		{
			Debug.LogError("Materials instance is null");
			return;
		}

		tileMap.ClearAllTiles();
		var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
		backgroundTextureMask = new Texture2D(size, size, TextureFormat.RGBA32, false);

		var tilePositions = new List<Vector3Int>();

		foreach (var mat in materials)
		{
			var coords = mat.Key;

			tileMap.SetTile(mat.Key, terrainTile);


			var terrain = Materials.Instance.GetMaterial(mat.Value);
			if (terrain == null)
			{
				Debug.LogWarning("material is null");
				continue;
			}

			tex.SetPixel(coords.x, coords.y, new Color32(terrain.textureIdx, 0, 0, 0));
		}

		tex.Apply();
		backgroundTextureMask.Apply();

		Shader.SetGlobalTexture("_AsteroidBGMask", backgroundTextureMask);

		tileMap.SetTiles(tilePositions.ToArray(), new TileBase[] { terrainTile });


		tileMap.CompressBounds();

		var bounds = tileMap.localBounds;

		var min = Vector3.zero;
		var max = tileMap.transform.TransformPoint(bounds.max);

		var material = tileMap.GetComponent<TilemapRenderer>().material;

		// dont have to worry about adding more later, we assume we can only destroy, not place
		material.SetTexture("_TerrainIndex", tex);

		Shader.SetGlobalVector("_TilemapMin", new Vector4(min.x, min.y, 0, 0));
		Shader.SetGlobalVector("_TilemapMax", new Vector4(max.x + 5, max.y + 5, 0, 0));

		//if (GlobalEvents.Instance != null)
		//	GlobalEvents.Instance.OnNewMapGenerated?.Invoke(materials);

		DestructibleTerrain.Instance.ApplyNewMap(materials);
	}

	void Update()
	{
		if (refresh)
		{
			//Generate(Random.Range(0, 999999), 64, out var materials);

			refresh = false;
		}
	}
}
