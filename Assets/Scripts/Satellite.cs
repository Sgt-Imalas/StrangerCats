using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(PolygonCollider2D))]
	internal class Satellite : MonoBehaviour, ISpawnRules
	{
		private Dictionary<Vector3Int, int> materialOverrides;
		private static readonly WaitForSecondsRealtime _waitForSecondsRealtime1_0 = new(1.0f);

		private AudioSource audioSource;

		public AudioClip[] pickSounds;
		public AudioClip deconstructSound;

		public Color dustColor;
		public FindableItem findableItem;
		public bool floating;
		public TerrainMaterial frame;

		public ParticleSystem particles;

		private Dictionary<Vector3Int, int> GetMaterialOverrides()
		{
			if (materialOverrides != null)
				return materialOverrides;

			materialOverrides = new Dictionary<Vector3Int, int>();

			if (frame == null)
				return materialOverrides;

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

			string[] floatymap = {
				"       XX       ",
				"      XXXX      ",
				"     XXXXXXX    ",
				"    XXX,,XXXX   ",
				"   XXX,,,,XXXX  ",
				"  XXX,,,,,,,XXX ",
				"XXX,,,,,,,,,,XXX",
				"XXXX,,,,O,,,,XX ",
				" XXX,,,,,,,,XXX ",
				" XXXXX,,,,,,XXX ",
				"  XXXXXX,,XXX   ",
				"    XXXXXXXX    ",
				"      XXXX      ",
				"       XX       " };
			materialOverrides = Utils.MakeOffsetsFromMap(false, frame.materialName.GetHashCode(), floating ? floatymap : map);

			return materialOverrides;
		}

		public void Start()
		{
			audioSource = GetComponent<AudioSource>();
			GetComponent<Health>().OnDeath += OnDeath;
			GetComponent<Health>().OnHurt += OnHurt;

			var main = particles.main;
			main.startColor = dustColor;
		}

		private void OnHurt(bool fatal)
		{
			var idx = Random.Range(0, pickSounds.Length - 1);
			audioSource.clip = pickSounds[idx];
			audioSource.pitch = Random.Range(0.9f, 1.1f);
			audioSource.Play();

			particles.Emit(5);
		}

		private void OnDeath()
		{
			audioSource.clip = deconstructSound;
			audioSource.pitch = 1.0f;
			audioSource.Play();

			particles.Emit(40);

			GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);

			GetComponent<PolygonCollider2D>().enabled = false;

			StartCoroutine(DieLater());
		}


		private IEnumerator DieLater()
		{
			yield return _waitForSecondsRealtime1_0;

			Object.Destroy(gameObject);
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
