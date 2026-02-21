using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[DefaultExecutionOrder(0)]
	[ExecuteAlways]
	public class Materials : MonoBehaviour
	{
		public static Materials Instance; // TODO move to global

		public static Dictionary<int, TerrainMaterial> materials;

		public static int
			Bone = "Bone".GetHashCode(),
			Meat = "Meat".GetHashCode(),
			Plastic = "Plastic".GetHashCode(),
			Rubber = "Rubber".GetHashCode(),
			RustedMetal = "Rusted Metal".GetHashCode(),
			SteelSheets = "Steel Sheets".GetHashCode(),
			StarDust = "Star Dust".GetHashCode(),
			IridiumOre = "Iridium Ore".GetHashCode();

		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
				Debug.Log("Materials instance created");
			}
			else
			{
				Debug.Log("Materials instance already exists, destroying duplicate");
				Destroy(gameObject);
				return;
			}

			Debug.Log("Materials OnAwake");
			Load();
		}

		public static TerrainMaterial GetMaterial(int id)
		{
			if (materials.TryGetValue(id, out var material)) return material;

			Debug.LogWarning("No material with id " + id);

			return null;
		}

		public TerrainMaterial GetMaterial(string name)
		{
			return GetMaterial(name.GetHashCode());
		}

		public static void Load()
		{
			var terrainMaterials = Resources.LoadAll<TerrainMaterial>("TerrainMaterials");
			materials = new Dictionary<int, TerrainMaterial>();

			foreach (var material in terrainMaterials)
			{
				material.Initialize();
				materials[material.hash] = material;
			}
		}
	}
}
