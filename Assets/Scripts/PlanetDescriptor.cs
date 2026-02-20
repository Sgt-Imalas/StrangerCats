using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[CreateAssetMenu(fileName = "Meat", menuName = "Planet")]
	public class PlanetDescriptor : ScriptableObject
	{
		public Texture2D icon;
		public ResourceType resourceType;
		public float noiseScale = 1.2f;
		public float noiseThreshold = 0.01f;
		public float bumpinessFrequency = 1.0f;
		public float bumpinessAmplitude = 2.97f;
		public int radius = 64;
		public GenerationPreset generationPreset;

		public float boneNoiseScale = 1.45f, boneAmpl = 7.78f, boneThreshold = 0.1f;

		public enum GenerationPreset
		{
			Meat
		}

		public void GenerateWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size)
		{
			GenerateMeatWorld(seed, out materials, out size);
		}

		public void GenerateMeatWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size)
		{
			materials = new Dictionary<Vector3Int, int>();
			var generator = MapGenerator.Instance;

			var noise = new FastNoiseLite(seed);
			noise.SetFractalType(FastNoiseLite.FractalType.Ridged);

			var boneNoise = new FastNoiseLite(1337);

			boneNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
			boneNoise.SetFrequency(0.02f);

			boneNoise.SetFractalType(FastNoiseLite.FractalType.PingPong);
			boneNoise.SetFractalOctaves(3);
			boneNoise.SetFractalPingPongStrength(2.0f);

			size = radius * 2 + 10;

			var center = new Vector2(radius + 5, radius + 5);

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

					var bx = (int)Mathf.Clamp(x * boneNoiseScale, 0, generator.boneMap.width - 1);
					var by = (int)Mathf.Clamp(y * boneNoiseScale, 0, generator.boneMap.height - 1);
					var boneValue = generator.boneMap.GetPixel(boneOffsetX + bx, boneOffsetY + by).r;

					if (value > noiseThreshold)
						generator.tileMap.SetTile(new Vector3Int(x, y, 0), generator.terrainTile);

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

