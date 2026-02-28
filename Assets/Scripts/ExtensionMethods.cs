using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
	public static void PlayRandom(this AudioSource source, AudioClip[] clips, bool randomizePitch = true, float minPitch = 0.9f, float maxPitch = 1.1f)
	{
		if (clips == null || clips.Length == 0)
		{
			Debug.LogWarning("Trying to play a random sound but no clips were provided.");
			return;
		}

		source.clip = clips.GetRandom();

		if (randomizePitch)
			source.pitch = Random.Range(minPitch, maxPitch);

		source.Play();
	}

	public static T GetRandom<T>(this IEnumerable<T> collection)
	{
		if (collection == null)
			return default;

		return collection.ElementAt(Random.Range(0, collection.Count() - 1));
	}

	public static T GetRandom<T>(this T[] collection)
	{
		if (collection == null || collection.Length == 0)
			return default;

		return collection[Random.Range(0, collection.Length - 1)];
	}
}

