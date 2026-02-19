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

	public float boneNoiseScale = 1.0f, boneAmpl = 1.0f, boneThreshold = 0.1f, pingpong = 8.0f;

	public Texture terrainTexture;
	public Texture2D boneMap;
	public Texture2D backgroundTextureMask;


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

		/*		var boneNoise = new FastNoiseLite(seed + 2);
				boneNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
				boneNoise.SetFractalType(FastNoiseLite.FractalType.PingPong);
				boneNoise.SetFractalOctaves(1);
				boneNoise.SetFractalPingPongStrength(pingpong);*/
		var boneNoise = new FastNoiseLite(1337);

		boneNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
		boneNoise.SetFrequency(0.02f);

		boneNoise.SetFractalType(FastNoiseLite.FractalType.PingPong);
		boneNoise.SetFractalOctaves(3);
		boneNoise.SetFractalPingPongStrength(2.0f);

		var boneNoiseMask = new FastNoiseLite(seed + 2);
		boneNoise.SetFractalOctaves(0);

		var bonenoiseShadow = new FastNoiseLite(seed + 1);

		tileMap.ClearAllTiles();

		var size = radius * 2 + 10;
		var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
		backgroundTextureMask = new Texture2D(size, size, TextureFormat.RGBA32, false);


		var tilePositions = new List<Vector3Int>();

		var meat = new Color32(0, 0, 0, 0);
		var bone = new Color32(1, 0, 0, 0);

		var center = new Vector2(radius + 5, radius + 5);

		//var test = new Texture2D(size, size, TextureFormat.RGBA32, false);

		if (boneMap.width != boneMap.height)
			Debug.LogWarning("Bonemap needs to be square");


		var maxOffset = boneMap.width - size - 1;
		var boneOffsetX = Random.Range(0, maxOffset);
		var boneOffsetY = Random.Range(0, maxOffset);

		for (var x = 0; x < size; x++)
		{
			for (var y = 0; y < size; y++)
			{
				var scaledX = x * noiseScale;
				var scaledY = y * noiseScale;

				var dist = Vector2.Distance(new Vector2(x, y), center);

				var noise2 = noise.GetNoise(x * bumpinessFrequency, y * bumpinessFrequency) * bumpinessAmplitude;
				if (dist + noise2 > radius)
					continue;

				backgroundTextureMask.SetPixel(x, y, Color.white);

				var value = noise.GetNoise(x * noiseScale, y * noiseScale);
				value *= value;

				//var boneValue = noise.GetNoise((float)(x * boneNoiseScale), (float)(y * boneNoiseScale)) * boneAmpl;

				var bx = (int)Mathf.Clamp(x * boneNoiseScale, 0, boneMap.width - 1);
				var by = (int)Mathf.Clamp(y * boneNoiseScale, 0, boneMap.height - 1);
				var boneValue = boneMap.GetPixel(boneOffsetX + bx, boneOffsetY + by).r;
				//boneValue = Mathf.Min(boneNoiseMask.GetNoise(scaledX, scaledY));
				//boneValue = Mathf.Min(bonenoiseShadow.GetNoise(scaledX, scaledY));

				if (value > noiseThreshold)
				{
					//tilePositions.Add(new Vector3Int(x, y, 0));
					tileMap.SetTile(new Vector3Int(x, y, 0), terrainTile);

				}

				//test.SetPixel(x, y, new Color(boneValue, -boneValue, boneValue, 1.0f));

				if (boneValue > boneThreshold)
					tex.SetPixel(x, y, bone);
				else
					tex.SetPixel(x, y, meat);
			}
		}

		tex.Apply();
		//test.Apply();
		backgroundTextureMask.Apply();

		Shader.SetGlobalTexture("_AsteroidBGMask", backgroundTextureMask);

		tileMap.CompressBounds();
		var bounds = tileMap.localBounds;

		var min = Vector3.zero;
		//tileMap.transform.TransformPoint(bounds.min);
		var max = tileMap.transform.TransformPoint(bounds.max);

		var material = tileMap.GetComponent<TilemapRenderer>().material;

		Shader.SetGlobalVector("_TilemapMin", new Vector4(min.x, min.y, 0, 0));
		Shader.SetGlobalVector("_TilemapMax", new Vector4(max.x + 5, max.y + 5, 0, 0));

		//material.SetVector("_TilemapMin", new Vector4(min.x, min.y, 0, 0));
		//material.SetVector("_TilemapMax", new Vector4(max.x + 5, max.y + 5, 0, 0));

		// dont have to worry about adding more later, we assume we can only destroy, not place
		material.SetTexture("_TerrainIndex", tex);

		var itemBGBytes = backgroundTextureMask.EncodeToPNG();
		System.IO.File.WriteAllBytes("C:/Users/Aki/Documents/Unity Projects/StrangerCats/Assets/Resources/Textures/bg.png", itemBGBytes);

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
