using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(AudioSource))]
	internal class Satellite : MonoBehaviour, ISpawnRules
	{
		private Dictionary<Vector3Int, int> materialOverrides;

		private AudioSource audio;

		public AudioClip[] pickSounds;

		private Dictionary<Vector3Int, int> GetMaterialOverrides()
		{
			if (materialOverrides != null)
				return materialOverrides;

			materialOverrides = new Dictionary<Vector3Int, int>();

			string[] map = {
				"       XX       ",
				"      XXXX      ",
				"     XXXXXXX    ",
				"    XXX,,XXXX   ",
				"   XXX,,,,XXXX  ",
				"  XXX,,,,,,,XXX ",
				"XXX,,,,,,,,,,XXX",
				"XXXX,,,,,,,,,XX ",
				" XXX,,,O,,,,XXX ",
				" XXXXXXXXXXXXX  ",
				"  XXXXXXXXXXX   ",
				"    XXXXXXXX    ",
				"      XXXX      ",
				"       XX       " };

			materialOverrides = Utils.MakeOffsetsFromMap(false, Materials.SteelSheets, map);

			return materialOverrides;
		}

		public void Start()
		{
			audio = GetComponent<AudioSource>();
			GetComponent<Health>().OnDeath += OnDeath;
			GetComponent<Health>().OnHurt += OnHurt;
		}

		private void OnHurt(bool fatal)
		{
			var idx = Random.Range(0, pickSounds.Length - 1);
			audio.clip = pickSounds[idx];
			audio.pitch = Random.Range(0.9f, 1.1f);
			audio.Play();
		}

		private void OnDeath()
		{
			UnityEngine.Object.Destroy(gameObject);
		}

		public bool CanSpawnHere(Vector3Int coord, Dictionary<Vector3Int, int> materials, out object data)
		{
			data = null;
			return true;
		}

		public void ConfigureSpawn(Vector3Int coord, Dictionary<Vector3Int, int> materials, HashSet<Vector3Int> claimedPositions, object data)
		{
			claimedPositions.Add(coord);

			for (var x = -3; x <= 3; x++)
			{
				for (var y = -3; y <= 3; y++)
				{
					var offsetCoord = coord + new Vector3Int(x, y);
					claimedPositions.Add(offsetCoord);
				}
			}

			foreach (var tile in GetMaterialOverrides())
			{
				var offsetCoord = coord + tile.Key;

				if (tile.Value == -1)
					materials.Remove(offsetCoord);
				else if (tile.Value != 0)
					materials[offsetCoord] = tile.Value;

			}
		}
	}
}
