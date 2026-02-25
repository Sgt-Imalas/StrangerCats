using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
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

