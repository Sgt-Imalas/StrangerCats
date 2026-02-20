using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Attributes))]
	public class PersistentPlayer : MonoBehaviour
	{
		public Attributes attributes;

		public static PersistentPlayer Instance { get; private set; }

		public bool testMod;

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

			DontDestroyOnLoad(gameObject);
			Instance = this;

			attributes = GetComponent<Attributes>();
		}

		void Start()
		{
			attributes.OnAttributeChanged += OnAttributesChanged;
		}

		private void OnAttributesChanged(AttributeType type, float finalValue)
		{
			GlobalEvents.Instance.OnPlayerAttributesChanged.Invoke(type, finalValue);
		}

		void Update()
		{
			if (testMod)
			{
				// adding a mod here
				// randomizing the id, if the id is shared it overrides the previous mod
				attributes.AddMod("TestMod" + Random.value.ToString(), AttributeType.FireRate, -0.1f);
				attributes.AddMod("TestMod2" + Random.value.ToString(), AttributeType.PodSpeed, 10f);
				testMod = false;
			}
		}


		public static float GetAttribute(AttributeType attributeId, float defaultValue = 0.0f)
		{
			if (Instance == null)
			{
				Debug.LogWarning("Player Instance is null");
				return defaultValue;
			}

			return Instance.attributes.Get(attributeId);
		}
	}
}