using System;
using UnityEngine;

namespace Assets.Scripts
{
	[CreateAssetMenu(fileName = "TerrainMaterial", menuName = "ScriptableObjects/TerrainMaterial", order = 1)]
	public class TerrainMaterial : ScriptableObject
	{
		public string materialName;
		public int hardness;
		public byte textureIdx;
		public ResourceType resourceType;
		public int resourceAmount = 1;
		public Color particlesColor;

		[NonSerialized] public int hash;

		public void Initialize()
		{
			hash = materialName.GetHashCode();
		}
	}
}
