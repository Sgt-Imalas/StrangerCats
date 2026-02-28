using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Attributes))]
	public class PersistentPlayer : MonoBehaviour
	{
		public Attributes attributes;

		public static PersistentPlayer Instance { get; private set; }

		public bool testMod;


		public float EnergyPercentage => LanderEnergy / MaxLanderEnergy;
		public float LanderEnergy = 120;
		float MaxLanderEnergy = 120;
		float LanderEnergyDecayPerSecond = 1f;
		float LaserEnergyDrainPerSecond = 0.75f;

		public bool InLander, LaserFiring, InHangar;

		public float iframes = 0.5f;
		public float lastDamageTaken;


		private void Awake()
		{
			//first player instantiated is kept across scenes, duplicates are destroyed
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);
			Instance = this;

			attributes = GetComponent<Attributes>();
			SceneManager.sceneLoaded += OnSceneLoaded;


			Global.Instance.LoadAndApplyAttributes(attributes);
		}

		void Start()
		{
			attributes.OnAttributeChanged += OnAttributesChanged;
			LanderEnergy = GetAttribute(AttributeType.LifeTime, MaxLanderEnergy);
		}


		void OnSceneLoaded(Scene s, LoadSceneMode mode)
		{
			InLander = s.name == "MineableTerrain";

			if (s.name == "Starmap")
				Global.WriteSaveFile();
		}

		private void OnAttributesChanged(AttributeType type, float finalValue)
		{
			GlobalEvents.Instance.OnPlayerAttributesChanged.Invoke(type, finalValue);

			if (type == AttributeType.LifeTime)
			{
				MaxLanderEnergy = finalValue;
				LanderEnergy = Mathf.Clamp(LanderEnergy, 0, MaxLanderEnergy);
			}
		}


		public void DamageEnergyPercentage(float damagePercentage)
		{
			if (iframes < lastDamageTaken)
			{
				var damage = damagePercentage * MaxLanderEnergy;

				LanderEnergy = Mathf.Clamp(LanderEnergy - damage, 0, MaxLanderEnergy);
				lastDamageTaken = 0.0f;
			}
		}
		public void DamageEnergy(float damage)
		{
			if (damage <= 0)
				return;

			if (iframes < lastDamageTaken)
			{

				LanderEnergy = Mathf.Clamp(LanderEnergy - damage, 0, MaxLanderEnergy);
				lastDamageTaken = 0.0f;
			}
		}

		private void Update()
		{
			if (!InLander || InHangar)
			{
				var rechargeAmount = MaxLanderEnergy * 0.334f * Time.deltaTime;
				LanderEnergy = Mathf.Clamp(LanderEnergy + rechargeAmount, 0, MaxLanderEnergy);
			}
			else
			{
				var decayAmount = LanderEnergyDecayPerSecond;
				if (LaserFiring)
					decayAmount += LaserEnergyDrainPerSecond;

				decayAmount *= Time.fixedDeltaTime;

				LanderEnergy = Mathf.Clamp(LanderEnergy - decayAmount, 0, MaxLanderEnergy);
				if (LanderEnergy == 0)
					LoadStarmap();
			}

			if (testMod)
			{
				// adding a mod here
				// randomizing the id, if the id is shared it overrides the previous mod
				attributes.AddMod("TestMod" + Random.value.ToString(), AttributeType.FireRate, -0.1f);
				attributes.AddMod("TestMod2" + Random.value.ToString(), AttributeType.PodSpeed, 10f);
				testMod = false;
			}

			lastDamageTaken += Time.deltaTime;
		}

		public static void LoadStarmap()
		{

			Global.StartLoadingStarmapScene();
		}

		public static void AddModifier(AttributeModifier mod)
		{
			if (Instance == null)
			{
				Debug.LogWarning("Player Instance is null");
				return;
			}
			Instance.attributes.AddMod(mod.id, mod.attributeId, mod.value, mod.multiplier);
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