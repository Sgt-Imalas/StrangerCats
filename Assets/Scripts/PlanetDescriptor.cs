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
			Meat,
			Plastic
		}

		public void GenerateWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, MapGenerator generator)
		{
			materials = null;
			size = 0;

			switch (generationPreset)
			{
				case GenerationPreset.Meat:
					GenerateMeatWorld(seed, out materials, out size, generator);
					break;
				case GenerationPreset.Plastic:
					GeneratePlasticWorld(seed, out materials, out size, generator);
					break;
			}
		}

		public void GeneratePlasticWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, MapGenerator generator)
		{
			materials = new Dictionary<Vector3Int, int>();

			size = radius * 2 + 1;
			var center = new Vector2(radius, radius);

			var rubberRadius = radius - 12;
			var emptyRadius = radius - 15;

			for (var x = 0; x < size; x++)
			{
				for (var y = 0; y < size; y++)
				{
					var dist = Vector2.Distance(new Vector2(x, y), center);

					if (dist < radius && dist > rubberRadius)
					{
						if (dist > rubberRadius)
						{
							materials[new Vector3Int(x, y)] = Materials.Plastic;
						}
						else if (dist > emptyRadius)
						{
							materials[new Vector3Int(x, y)] = Materials.Rubber;
						}
					}
				}
			}
		}

		public void GenerateMeatWorld(int seed, out Dictionary<Vector3Int, int> materials, out int size, MapGenerator generator)
		{
			materials = new Dictionary<Vector3Int, int>();

			var noise = new FastNoiseLite(seed);
			noise.SetFractalType(FastNoiseLite.FractalType.Ridged);

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

