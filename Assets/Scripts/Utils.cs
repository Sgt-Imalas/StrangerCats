using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class Utils
	{
		public const char CENTER = 'O';
		public const char FILLED = 'X';
		public const char REMOVE = ',';

		public static List<Vector3Int> MakeOffsetsFromMap(bool fillCenter, params string[] pattern)
		{
			var xCenter = 0;
			var yCenter = 0;
			var result = new List<Vector3Int>();

			for (var y = 0; y < pattern.Length; y++)
			{
				var line = pattern[y];
				for (var x = 0; x < line.Length; x++)
				{
					if (line[x] == CENTER)
					{
						xCenter = x;
						yCenter = y;

						break;
					}
				}
			}

			for (var y = 0; y < pattern.Length; y++)
			{
				var line = pattern[y];
				for (var x = 0; x < line.Length; x++)
				{
					if (line[x] == FILLED
						|| (fillCenter && line[x] == CENTER))
						result.Add(new Vector3Int(x - xCenter, y - yCenter));
				}
			}

			return result;
		}

		public static Dictionary<Vector3Int, int> MakeOffsetsFromMap(bool fillCenter, int xValue, params string[] pattern)
		{
			var xCenter = 0;
			var yCenter = 0;
			var result = new Dictionary<Vector3Int, int>();

			// unity is upside down
			pattern = pattern.Reverse().ToArray();

			for (var y = 0; y < pattern.Length; y++)
			{
				var line = pattern[y];
				for (var x = 0; x < line.Length; x++)
				{
					if (line[x] == CENTER)
					{
						xCenter = x;
						yCenter = y;

						break;
					}
				}
			}

			for (var y = 0; y < pattern.Length; y++)
			{
				var line = pattern[y];
				for (var x = 0; x < line.Length; x++)
				{
					var filled = line[x] == FILLED || (fillCenter && line[x] == CENTER);
					var remove = line[x] == REMOVE;

					var value = filled ? xValue : remove ? -1 : 0;

					result.Add(new Vector3Int(x - xCenter, y - yCenter), value);
				}
			}

			return result;
		}
	}
}
