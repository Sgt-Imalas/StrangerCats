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
				Debug.LogWarning("PersistentPlayer intance was not null. This should not be reassigned!");
				Instance = null;
			}

			Instance = this;

			attributes = GetComponent<Attributes>();

			// initializing an attribute with default values.
			// if modding without calling this first, mods will not take effect
			attributes.SetBaseValue(AttributeType.FireRate, 1.0f, 0.01f, 10f);
			attributes.SetBaseValue(AttributeType.ExplosionRadius, 1.0f, 0f, 10f);
		}

		void Start()
		{

		}

		void Update()
		{
			if (testMod)
			{
				// adding a mod here
				// randomizing the id, if the id is shared it overrides the previous mod
				attributes.AddMod("TestMod" + Random.value.ToString(), AttributeType.FireRate, -0.1f);
				testMod = false;
			}
		}
	}
}