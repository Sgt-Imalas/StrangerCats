using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[DefaultExecutionOrder(0)]
	public class GlobalEvents : MonoBehaviour
	{
		public static GlobalEvents Instance { get; private set; }

		public Attributes.OnAttributeChangedDelegate OnPlayerAttributesChanged;
		public OnNewMapGeneratedDelegate OnNewMapGenerated;

		public delegate void OnNewMapGeneratedDelegate(Dictionary<Vector3Int, int> materials);

		private void Awake()
		{
			//all instances after the first one are destroyed, the first one is kept across scenes
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}
