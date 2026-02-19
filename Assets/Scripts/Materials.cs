using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[DefaultExecutionOrder(0)]
	public class Materials : MonoBehaviour
	{
		public static Materials Instance; // TODO move to global

		public Dictionary<int, TerrainMaterial> materials;

		public static int
			Bone = "Bone".GetHashCode(),
			Meat = "Meat".GetHashCode();

		private void Awake()
		{
			Instance = this;
			Load();
		}

		public TerrainMaterial GetMaterial(int id)
		{
			if (materials.TryGetValue(id, out var material)) return material;

			Debug.LogWarning("No material with id " + id);

			return null;
		}

		public TerrainMaterial GetMaterial(string name)
		{
			return GetMaterial(name.GetHashCode());
		}

		public void Load()
		{
			var materials = Resources.LoadAll<TerrainMaterial>("TerrainMaterials");
			this.materials = new Dictionary<int, TerrainMaterial>();

			foreach (var material in materials)
			{
				material.Initialize();
				this.materials[material.hash] = material;
			}
		}
	}
}
