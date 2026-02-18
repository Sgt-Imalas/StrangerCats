using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class MapGenerator : MonoBehaviour
{
	private FastNoiseLite noise;
	public Vector2Int size;
	public Tilemap tileMap;
	public float noiseScale;
	public float noiseThreshold;
	public TileBase terrainTile;
	public bool refresh;

	public void Generate(int seed)
	{
		noise = new FastNoiseLite(seed);

		noise.SetFractalType(FastNoiseLite.FractalType.Ridged);

		tileMap.ClearAllTiles();

		var tilePositions = new List<Vector3Int>();

		for (var x = 0; x < size.x; x++)
		{
			for (var y = 0; y < size.y; y++)
			{
				var value = noise.GetNoise(x * noiseScale, y * noiseScale);
				value *= value;
				if (value > noiseThreshold)
				{
					//tilePositions.Add(new Vector3Int(x, y, 0));
					tileMap.SetTile(new Vector3Int(x, y, 0), terrainTile);
				}
			}
		}

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
