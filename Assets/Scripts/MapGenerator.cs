using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class MapGenerator : MonoBehaviour
{
	private FastNoiseLite noise;
	public int radius;
	public Tilemap tileMap;
	public float noiseScale;
	public float noiseThreshold;
	public TileBase terrainTile;
	public bool refresh;
	public float bumpinessFrequency = 1.0f;
	public float bumpinessAmplitude = 1.0f;

	public Texture terrainTexture;


	public void Generate(string map)
	{
		if (map == "MeatPlanet")
		{

		}
	}

	public void Generate(int seed)
	{
		noise = new FastNoiseLite(seed);
		noise.SetFractalType(FastNoiseLite.FractalType.Ridged);

		var bonenoise = new FastNoiseLite(seed + 1);
		bonenoise.SetFractalType(FastNoiseLite.FractalType.Ridged);

		tileMap.ClearAllTiles();

		var size = radius * 2 + 10;
		var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);



		var tilePositions = new List<Vector3Int>();

		var meat = new Color32(0, 0, 0, 0);
		var bone = new Color32(1, 0, 0, 0);

		var center = new Vector2(radius + 5, radius + 5);

		for (var x = 0; x < size; x++)
		{
			for (var y = 0; y < size; y++)
			{
				var dist = Vector2.Distance(new Vector2(x, y), center);

				var noise2 = noise.GetNoise(x * bumpinessFrequency, y * bumpinessFrequency) * bumpinessAmplitude;
				if (dist + noise2 > radius)
					continue;

				var value = noise.GetNoise(x * noiseScale, y * noiseScale);
				value *= value;

				var boneValue = bonenoise.GetNoise(x * noiseScale, y * noiseScale);
				boneValue *= boneValue;

				if (value > noiseThreshold)
				{
					//tilePositions.Add(new Vector3Int(x, y, 0));
					tileMap.SetTile(new Vector3Int(x, y, 0), terrainTile);

				}
				if (boneValue < noiseThreshold)
					tex.SetPixel(x, y, bone);
				else
					tex.SetPixel(x, y, meat);
			}
		}

		tex.Apply();

		tileMap.CompressBounds();
		var bounds = tileMap.localBounds;

		var min = Vector3.zero;
		//tileMap.transform.TransformPoint(bounds.min);
		var max = tileMap.transform.TransformPoint(bounds.max);

		var material = tileMap.GetComponent<TilemapRenderer>().material;

		material.SetVector("_TilemapMin", new Vector4(min.x, min.y, 0, 0));
		material.SetVector("_TilemapMax", new Vector4(max.x + 5, max.y + 5, 0, 0));

		// dont have to worry about adding more later, we assume we can only destroy, not place
		material.SetTexture("_TerrainIndex", tex);

		//var itemBGBytes = tex.EncodeToPNG();
		//System.IO.File.WriteAllBytes("C:/Users/Aki/Documents/Unity Projects/StrangerCats/Assets/Resources/Textures/terrain_test_texture.png", itemBGBytes);

		Debug.Log("generated map");
		tileMap.SetTiles(tilePositions.ToArray(), new TileBase[] { terrainTile });
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (refresh)
		{
			Generate(Random.Range(0, 999999));
			refresh = false;
		}
	}
}
