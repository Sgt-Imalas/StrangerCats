using System;
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
		public Action<Vector3Int, int> OnTileDestroyed;
		public Action<Vector3, ResourceType> OnEnemyKilled;

		public delegate void OnNewMapGeneratedDelegate(Dictionary<Vector3Int, int> materials, PlanetDescriptor descriptor);

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
