using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public interface ISpawnRules
	{
		public bool CanSpawnHere(Vector3Int coord, Dictionary<Vector3Int, int> materials, out object data);

		public void ConfigureSpawn(Vector3Int coord, Dictionary<Vector3Int, int> materials, HashSet<Vector3Int> claimedPositions, object data);
	}
}
