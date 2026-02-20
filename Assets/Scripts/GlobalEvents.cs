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

		private void OnDestroy()
		{
			if (Instance != null)
				Instance = null;
		}

		private void Awake()
		{
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
