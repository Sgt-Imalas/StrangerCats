using UnityEngine;

namespace Assets.Scripts
{
	public class GlobalEvents : MonoBehaviour
	{
		public static GlobalEvents Instance { get; private set; }

		public Attributes.OnAttributeChangedDelegate OnPlayerAttributesChanged;


		private void OnDestroy()
		{
			if (Instance != null)
				Instance = null;
		}

		private void Awake()
		{
			if (Instance != null)
			{
				Debug.LogWarning("GlobalEvents intance was not null. This should not be reassigned!");
				Instance = null;
			}

			Instance = this;

			DontDestroyOnLoad(gameObject);
		}
	}
}
