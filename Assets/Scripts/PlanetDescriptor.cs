using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[CreateAssetMenu(fileName = "Meat", menuName = "Planet")]
	public class PlanetDescriptor : ScriptableObject
	{
		public Texture2D icon;
		public Texture2D bg;
		public ResourceType resourceType;
		public float noiseScale = 1.2f;
		public float noiseThreshold = 0.01f;
		public float bumpinessFrequency = 1.0f;
		public float bumpinessAmplitude = 2.97f;
		public int radius = 64;
		public int innerRadius = 40;
		public GenerationPreset generationPreset;
		public GameObject enemyPreset;
		public GameObject gizmoPrefab;
		public int enemiesToSpawn = 32;
		public Vector3 playerSpawnPoint;
		public float fractalGain = 1.0f;
		public int octaves = 3;


		public float boneNoiseScale = 1.45f, boneAmpl = 7.78f, boneThreshold = 0.1f;

		public int MusicIndex = 0;

		public enum GenerationPreset
		{
			Meat,
			Plastic,
			Tutorial,
			Iridium
		}

		public void GenerateWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, out Vector2 center, MapGenerator generator)
		{
			materials = null;
			size = 0;
			center = Vector2.zero;

			switch (generationPreset)
			{
				case GenerationPreset.Meat:
					GenerateMeatWorld(seed, out materials, out size, generator, out center);
					break;
				case GenerationPreset.Plastic:
					GeneratePlasticWorld(seed, out materials, out size, generator, out center);
					break;
				case GenerationPreset.Tutorial:
					GenerateTutorialWorld(seed, out materials, out size, generator, out center);
					break;
				case GenerationPreset.Iridium:
					GenerateIridiumWorld(seed, out materials, out size, generator, out center);
					break;
			}

			playerSpawnPoint = center + new Vector2(0, size / 2.0f + 10);

			// clear out any accidental terrain overlap
			for (var x = -2; x <= 2; x++)
			{
				for (var y = -2; y <= 2; y++)
				{
					var tile = new Vector3Int(x + (int)playerSpawnPoint.x, y + (int)playerSpawnPoint.y, 0);
					materials.Remove(tile);
				}
			}

			MusicManager.PlayNewSong(MusicIndex);
		}

		public void GenerateTutorialWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, MapGenerator generator, out Vector2 center)
		{
			materials = new Dictionary<Vector3Int, int>();

			size = radius * 2 + 10;

			center = new Vector2(radius + 5, radius + 5);

			var islandsNoise = new FastNoiseLite(seed);
			islandsNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
			islandsNoise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);

			var noise = new FastNoiseLite(seed + 1);
			islandsNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
			islandsNoise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);


			for (var x = 0; x < size; x++)
			{
				for (var y = 0; y < size; y++)
				{
					var dist = Vector2.Distance(new Vector2(x, y), center);

					var noise2 = noise.GetNoise(x * bumpinessFrequency, y * bumpinessFrequency) * bumpinessAmplitude;
					if (dist + noise2 > radius)
						continue;

					var value = islandsNoise.GetNoise(x * noiseScale, y * noiseScale);
					value *= value;

					if (value > noiseThreshold)
					{
						var metalSelect = Random.value > 0.6f;

						metalSelect |= noise.GetNoise(x * noiseScale, y * noiseScale) > 0.2f;

						materials[new Vector3Int(x, y)] = metalSelect ? Materials.RustedMetal : Materials.SteelSheets;
					}
				}
			}
		}

		public void GeneratePlasticWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, MapGenerator generator, out Vector2 center)
		{
			materials = new Dictionary<Vector3Int, int>();

			var noise = new FastNoiseLite(seed);
			noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
			noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
			noise.SetFractalOctaves(1);

			size = radius * 2 + 1;
			center = new Vector2(radius, radius);

			var ring = 24;
			var rubberRadius = radius - ring;
			var emptyRadius = innerRadius;

			for (var x = 0; x < size; x++)
			{
				for (var y = 0; y < size; y++)
				{
					var dist = Vector2.Distance(new Vector2(x, y), center);

					if (dist < radius)
					{
						if (dist > rubberRadius)
						{
							var noiseSample = noise.GetNoise(x * noiseScale, y * noiseScale);
							if (noiseSample < noiseThreshold)
								materials[new Vector3Int(x, y)] = Materials.Rubber;
							else
								materials[new Vector3Int(x, y)] = Materials.Plastic;
						}
						else if (dist > emptyRadius)
						{
							materials[new Vector3Int(x, y)] = Materials.Plastic;
						}
					}
				}
			}
		}
		public void GenerateIridiumWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, MapGenerator generator, out Vector2 center)
		{
			materials = new Dictionary<Vector3Int, int>();

			var noise = new FastNoiseLite(seed);
			noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
			noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2);
			noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Manhattan);

			noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.BasicGrid);
			noise.SetDomainWarpAmp(58);
			size = radius * 2 + 10;

			center = new Vector2(radius + 5, radius + 5);

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

					if (value > noiseThreshold)
					{
						var noiseDust = noise.GetNoise(x * boneNoiseScale + 200, y * boneNoiseScale + 200);

						materials[new Vector3Int(x, y)] = noiseDust > boneThreshold ? Materials.StarDust : Materials.IridiumOre;
					}
				}
			}
		}

		public void GenerateMeatWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, MapGenerator generator, out Vector2 center)
		{
			materials = new Dictionary<Vector3Int, int>();

			var noise = new FastNoiseLite(seed);
			noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
			noise.SetFractalGain(fractalGain);
			noise.SetFractalOctaves(octaves);

			size = radius * 2 + 10;

			center = new Vector2(radius + 5, radius + 5);

			if (generator.boneMap.width != generator.boneMap.height)
				Debug.LogWarning("Bonemap needs to be square");

			var maxOffset = generator.boneMap.width - size - 1;
			var boneOffsetX = UnityEngine.Random.Range(0, maxOffset);
			var boneOffsetY = UnityEngine.Random.Range(0, maxOffset);

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

					if (value > noiseThreshold)
					{
						var bx = (int)Mathf.Clamp(x * boneNoiseScale, 0, generator.boneMap.width - 1);
						var by = (int)Mathf.Clamp(y * boneNoiseScale, 0, generator.boneMap.height - 1);
						var boneValue = generator.boneMap.GetPixel(boneOffsetX + bx, boneOffsetY + by).r;

						if (boneValue > boneThreshold)
						{
							materials[new Vector3Int(x, y)] = Materials.Bone;
						}
						else
						{
							materials[new Vector3Int(x, y)] = Materials.Meat;
						}
					}
				}
			}
		}
	}
}

