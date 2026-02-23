using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class Global
{
	public const float StickDeadzone = 0.3f;
	private static Global _instance;

	public static Global Instance
	{
		get
		{
			_instance ??= new Global();
			return _instance;
		}
	}
	const string saveKey = "SaveGame";
	public static void DeleteSaveFile()
	{
		Debug.Log("Deleting Savefile");
		PlayerPrefs.DeleteKey(saveKey);
		_instance = null;
	}
	public static void WriteSaveFile()
	{
		Debug.Log("Saving the Game");
		//var saveData = JsonUtility.ToJson(Instance);
		//PlayerPrefs.SetString(saveKey, saveData);
	}
	internal void LoadAndApplyAttributes(Attributes attributes)
	{
		//Todo: store attribute modifiers in the serialized global item, and apply them here
	}
	internal static void LoadPersistantInstance()
	{

		//this is broken af lol


		//if (PlayerPrefs.HasKey(saveKey))
		//{
		//	Debug.Log("Loading savefile");
		//	string data = PlayerPrefs.GetString(saveKey);
		//	_instance = JsonUtility.FromJson<Global>(data);
		//}
		//else

		//	Debug.Log("No save file found!");
	}

	public List<GameObject> entities = new();

	public StarmapShip Spaceship = new();
	public MiningResourceStorage SpaceshipResources = new();
	public GameUpgrades Upgrades = new GameUpgrades();

	public bool LoadingScene = false;
	public bool InCameraTransition = false;
	public bool InPauseMenu = false;
	public bool InUpgradeMenu = false;
	public bool InDialogue = false;
	public bool LockedInputs => InCameraTransition || InPauseMenu || InUpgradeMenu || InDialogue;
	public int WorldSeed;
	public PlanetDescriptor generateWorld;
	public PlanetDescriptor loadPlanet;
	public bool PortalOpened = false;


	public void StartLoadingStarmapScene()
	{
		if (Global.Instance != null && Global.Instance.entities != null)
		{
			for (var i = Global.Instance.entities.Count - 1; i >= 0; i--)
			{
				var entity = Global.Instance.entities[i];
				UnityEngine.Object.Destroy(entity);
			}
		}

		LoadOverlay.ShowOverlay();
		LoadingScene = true;
		SceneManager.sceneLoaded += OnSceneLoadFinished;
		SceneManager.LoadScene("Starmap");
	}

	internal void UpgradePurchased()
	{
		OnUpgradePurchased?.Invoke();
		WriteSaveFile();
	}
	public event Action OnUpgradePurchased;
	public void StartLoadingMainMenu()
	{
		Debug.Log("Loading Main Menu");
		LoadOverlay.ShowOverlay();
		LoadingScene = true;
		SceneManager.sceneLoaded += OnSceneLoadFinished;
		SceneManager.LoadScene("MainMenu");
	}
	void OnSceneLoadFinished(Scene s, LoadSceneMode mode)
	{
		LoadOverlay.ShowOverlay(false);
		LoadingScene = false;
		SceneManager.sceneLoaded -= OnSceneLoadFinished;
	}
	public static string ToShortNumber(double number)
	{
		string[] suffixes = { "", "K", "M", "B", "T" };
		var suffixIndex = 0;

		while (number >= 1000 && suffixIndex < suffixes.Length - 1)
		{
			number /= 1000;
			suffixIndex++;
		}

		return $"{number:0.#}{suffixes[suffixIndex]}";
	}
}

public class StarmapShip
{
	public Vector2 Position = new(0, 0);
	public Quaternion Rotation = new();

	public FlightStats PrecisionMode = new()
	{
		MaxVelocity = 30f,
		Accelleration = 20f,
		RotationSpeed = 240f,
		LinearDampening = 0.75f,
		CameraOffset = -15
	};
	public FlightStats CruiseMode = new()
	{
		MaxVelocity = 100f,
		Accelleration = 30f,
		RotationSpeed = 135f,
		LinearDampening = 0.25f,
		CameraOffset = -25
	};
	public FlightStats PodMode = new()
	{
		MaxVelocity = 15f,
		Accelleration = 10f,
		LinearDampening = 3f
	};

	public float CurrentVelocity;
	public bool InPrecisionFlightMode = true;
	public bool CanLand = true;
	public bool TooFastToLand => CurrentVelocity > 15f;
	public bool BlockedFromLanding => !CanLand || !InPrecisionFlightMode || TooFastToLand;

	//starts off in precision mode, set by the shipcontroller
	private FlightStats _currentMode;
	public FlightStats CurrentMode
	{
		get
		{
			if (_currentMode == null)
			{
				_currentMode = PrecisionMode;
			}
			return _currentMode;
		}
		set => _currentMode = value;
	}
}
public class FlightStats
{
	public float MaxVelocity;
	public float Accelleration;
	public float RotationSpeed;
	public float LinearDampening;
	public float CameraOffset = -20;
}

/// <summary>
/// !!none is only used in the configurator of drops
/// </summary>
public enum ResourceType
{
	Meat, Rust, Ball, Dust, None = -1
}
public enum FindableItem
{
	Radar, SuperCruise, Meat, Tennis, Desert, None = -1
}

//public record ResourceInfo(ResourceType Type, string Name, Color color);

public class MiningResourceStorage
{
	public static Dictionary<ResourceType, Color> ResourceInfos = new()
	{
		{ResourceType.Meat, new Color32(185, 0, 0,byte.MaxValue)},
		{ResourceType.Rust,new Color32(242, 134, 0,byte.MaxValue)},
		{ResourceType.Ball, new Color32(13, 255, 0,byte.MaxValue)},
		{ResourceType.Dust, new Color32(0, 64, 255,byte.MaxValue)}
	};

	public Dictionary<ResourceType, uint> Current = new()
	//DEBUG!!
	//{ { ResourceType.Meat,999999 },{ ResourceType.Rust,999999 },{ ResourceType.Ball,999999 },{ ResourceType.Dust,999999 }, }
	;

	public event Action<ResourceType> OnResourceDiscovered;
	public event Action<ResourceType, uint> OnResourceCollected;
	public event Action<ResourceType, uint> OnResourceSpent;
	public event Action<ResourceType, int> OnResourceAmountChanged;

	public void CollectResource(ResourceType type, uint amount)
	{
		if (Current == null)
		{
			Debug.LogWarning("Current is null");
			return;
		}

		if (!Current.ContainsKey(type))
		{
			Current[type] = 0;
			OnResourceDiscovered?.Invoke(type);
		}

		var collected = Math.Clamp(amount + Current[type], 0, uint.MaxValue);
		Current[type] = collected;
		OnResourceCollected?.Invoke(type, collected);
		OnResourceAmountChanged?.Invoke(type, (int)collected);
	}
	public void SpendResource(ResourceType type, uint amount)
	{
		if (!Current.ContainsKey(type))
			return;
		var spent = Math.Clamp(Current[type] - amount, 0, uint.MaxValue);
		Current[type] = spent;
		OnResourceSpent?.Invoke(type, spent);
		OnResourceAmountChanged?.Invoke(type, -((int)spent));
	}
	public bool CanAfford(ResourceType type, uint amount)
	{
		if (amount == 0)
			return true;
		return Current.ContainsKey(type) && Current[type] >= amount;
	}
	public bool CanAfford(ResourceLevel costs)
	{
		foreach (var cost in costs)
		{
			if (!CanAfford(cost.Key, cost.Value))
				return false;
		}
		return true;
	}
	public bool ResourceDiscovered(ResourceType type)
	{
		return (Current.ContainsKey(type));
	}
	public uint GetResourceAmount(ResourceType type)
	{
		return Current.ContainsKey(type) ? Current[type] : 0;
	}

	internal bool AnyResourceDiscovered()
	{
		return Current.Any();
	}
}


public class GameUpgrades
{
	public bool RadarUnlocked = false;
	public bool SuperCruiseUnlocked = false;
	public bool MeatWorldItemFound = false;
	public bool TennisWorldItemFound = false;
	public bool DesertWorldItemFound = false;

	public void CollectFindableItem(FindableItem item)
	{
		switch (item)
		{
			case FindableItem.Radar:
				if (!RadarUnlocked)
				{
					RadarUnlocked = true;
					PersistentPlayer.AddModifier(new AttributeModifier("RadarUnlock", AttributeType.RadarRange, 500));
				}
				break;
			case FindableItem.SuperCruise:
				SuperCruiseUnlocked = true;
				break;
			case FindableItem.Meat:
				MeatWorldItemFound = true;
				break;
			case FindableItem.Tennis:
				TennisWorldItemFound = true;
				break;
			case FindableItem.Desert:
				DesertWorldItemFound = true;
				break;
		}
		OnItemCollected?.Invoke(item);

		Global.WriteSaveFile();
	}

	public event Action<FindableItem> OnItemCollected;



	public BuyableUpgrade RadarRange = new BuyableUpgrade("Radar Range", 50, 1.2f)
		.Modifier(AttributeType.RadarRange, 250f)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Meat, 10)
		.IncrementalCostThreshold(ResourceType.Ball, 20)
		.UnlockCondition(() => Global.Instance.Upgrades.RadarUnlocked)
		;

	//flat value is mult
	public BuyableUpgrade SuperCruise = new BuyableUpgrade("Supercruise Speed", 75, 1.2f)
		.Modifier(AttributeType.SpaceShipSuperCruiseSpeed, 0.10f)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Meat, 0)
		.IncrementalCostThreshold(ResourceType.Dust, 20)
		.UnlockCondition(() => Global.Instance.Upgrades.SuperCruiseUnlocked);

	//flat value is mult
	public BuyableUpgrade RotationSpeed = new BuyableUpgrade("Rotation Speed", 40, 1.125f)
		.Modifier(AttributeType.SpaceShipRotationSpeed, 0.10f)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Ball, 10)
		.IncrementalCostThreshold(ResourceType.Dust, 20);

	//flat +60s
	public BuyableUpgrade LifeSupport = new BuyableUpgrade("Lifesupport", 40, 1.3f)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Meat, 3)
		.IncrementalCostThreshold(ResourceType.Dust, 15)
		.Modifier(AttributeType.LifeTime, 60f);

	//flat +0.25
	public BuyableUpgrade PodSpeed = new BuyableUpgrade("Thruster Strength", 40, 1.125f)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Ball, 10)
		.Modifier(AttributeType.PodSpeed, 0.25f);

	//flat +2 tiles
	public BuyableUpgrade LaserRange = new BuyableUpgrade("Laser Reach", 40, 1.2f)
		.IncrementalCostThreshold(ResourceType.Ball, 0)
		.IncrementalCostThreshold(ResourceType.Dust, 0)
		.Modifier(AttributeType.LaserRange, 2)
		.Modifier(AttributeType.ExplosionRadius, 0.25f);

	//funny scaling
	public BuyableUpgrade LaserDamage = new BuyableUpgrade("Laser Damage", 50, 1.2f)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Meat, 10)
		.IncrementalCostThreshold(ResourceType.Dust, 15)
		.Modifier(AttributeType.DigDamage, 1)
		.Modifier(AttributeType.DigDamage, 1.15f, true);

	//funny scaling
	public BuyableUpgrade LaserSpeed = new BuyableUpgrade("Laser Fire Rate", 50, 1.2f)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Meat, 10)
		.IncrementalCostThreshold(ResourceType.Ball, 15)
		.Modifier(AttributeType.FireRate, -0.025f)
		.Modifier(AttributeType.FireRate, 0.9f, true);

	//funny scaling
	public BuyableUpgrade ResourceYield = new BuyableUpgrade("Resource Yield", 250, 1.5f)
		.IncrementalCostThreshold(ResourceType.Meat, 3)
		.IncrementalCostThreshold(ResourceType.Rust, 0)
		.IncrementalCostThreshold(ResourceType.Ball, 6)
		.IncrementalCostThreshold(ResourceType.Dust, 9)
		.Modifier(AttributeType.ResourceTileMultiplier, 1)
		.Modifier(AttributeType.ResourceTileMultiplier, 1.10f, true);
}

public class BuyableUpgrade
{
	public ResourceLevel GetCurrentScaledCosts()
	{
		var Level = new ResourceLevel();
		var scale = Mathf.Pow(CostScaling, this.Level);
		foreach (var cost in CostAdditionThresholds)
		{
			if (cost.Value <= this.Level)
			{
				switch (cost.Key)
				{
					case ResourceType.Meat:
						Level.A = (uint)Mathf.Round(BaseCost * scale);
						break;
					case ResourceType.Rust:
						Level.B = (uint)Mathf.Round(BaseCost * scale);
						break;
					case ResourceType.Ball:
						Level.C = (uint)Mathf.Round(BaseCost * scale);
						break;
					case ResourceType.Dust:
						Level.D = (uint)Mathf.Round(BaseCost * scale);
						break;
				}
			}
		}
		return Level;
	}
	public BuyableUpgrade(string name, uint startingCost = 10, float incrementalScaling = 1.25f)
	{
		Name = name;
		BaseCost = startingCost;
		CostScaling = incrementalScaling;
	}
	public BuyableUpgrade Modifier(AttributeType type, float value, bool multiplier = false)
	{
		if (Modifiers == null)
			Modifiers = new();
		Modifiers.Add(new AttributeModifier()
		{
			id = Name + "_" + Modifiers.Count,
			attributeId = type,
			value = value,
			multiplier = multiplier
		});
		return this;
	}
	public BuyableUpgrade IncrementalCostThreshold(ResourceType type, int level = 0)
	{
		CostAdditionThresholds[type] = level;
		return this;
	}
	public BuyableUpgrade Max(int max)
	{
		MaxLevel = max;
		return this;
	}
	public BuyableUpgrade UnlockCondition(Func<bool> condition)
	{
		UnlockedCondition = condition;
		return this;
	}

	internal string GetUpgradeText()
	{
		if (IsMaxed())
			return "Max Level Reached";

		if (!UnlockedCondition())
			return "Not found yet";

		if (!Global.Instance.SpaceshipResources.CanAfford(GetCurrentScaledCosts()))
			return "Can't affort";

		return string.Empty;
	}

	public void PurchaseLevel()
	{
		if (Level >= MaxLevel || !UnlockedCondition())
			return;
		var costs = GetCurrentScaledCosts();
		if (!Global.Instance.SpaceshipResources.CanAfford(costs))
			return;
		foreach (var cost in costs)
		{
			Global.Instance.SpaceshipResources.SpendResource(cost.Key, cost.Value);
		}
		Level++;
		OnPurchase?.Invoke();
		if (Modifiers != null)
		{
			foreach (var mod in Modifiers)
			{
				PersistentPlayer.AddModifier(mod);
			}
		}
		Global.Instance.UpgradePurchased();
	}

	public bool IsMaxed()
	{
		return Level >= MaxLevel;
	}
	public bool IsUnlocked()
	{
		return UnlockedCondition();
	}

	public string Name;
	public Func<bool> UnlockedCondition = () => true;
	public int Level = 0;
	public int MaxLevel = int.MaxValue;
	public float CostScaling = 1.1f;
	public long BaseCost = 10;

	public Dictionary<ResourceType, int> CostAdditionThresholds = new();

	public List<AttributeModifier> Modifiers;
	public Action OnPurchase;
}
public struct ResourceLevel : IEnumerable
{
	public uint A, B, C, D;

	public ResourceLevel(ResourceLevel prev, float scaling)
	{
		A = (uint)Mathf.RoundToInt(prev.A * scaling);
		B = (uint)Mathf.RoundToInt(prev.B * scaling);
		C = (uint)Mathf.RoundToInt(prev.C * scaling);
		D = (uint)Mathf.RoundToInt(prev.D * scaling);
	}
	public ResourceLevel(uint a = 0, uint b = 0, uint c = 0, uint d = 0)
	{
		A = a;
		B = b;
		C = c;
		D = d;
	}

	public IEnumerator<KeyValuePair<ResourceType, uint>> GetEnumerator()
	{
		yield return new(ResourceType.Meat, A);
		yield return new(ResourceType.Rust, B);
		yield return new(ResourceType.Ball, C);
		yield return new(ResourceType.Dust, D);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
