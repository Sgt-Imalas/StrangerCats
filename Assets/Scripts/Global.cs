using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Global
{
	private static Global _instance;

	public static Global Instance
	{
		get
		{
			_instance ??= new Global();
			return _instance;
		}
	}

	public StarmapShip Spaceship = new();
	public MiningResourceStorage SpaceshipResources = new();
	public GameUpgrades Upgrades = new GameUpgrades();

	public bool LoadingScene = false;
	public bool InCameraTransition = false;
	public bool InMenu = false;
	public bool LockedInputs => InCameraTransition || InMenu;
	public int WorldSeed;
	public PlanetDescriptor generateWorld;
	public PlanetDescriptor loadPlanet;
}

public class StarmapShip
{
	public Vector2 Position = new(0, 0);
	public Quaternion Rotation = new();

	public FlightStats PrecisionMode = new()
	{
		Unlocked = true,
		MaxVelocity = 30f,
		Accelleration = 26f,
		RotationSpeed = 240f,
		LinearDampening = 0.5f,
		CameraOffset = -25
	};
	public FlightStats CruiseMode = new()
	{
		MaxVelocity = 100f,
		Accelleration = 100f,
		RotationSpeed = 180f,
		LinearDampening = 0.5f,
		CameraOffset = -45
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
	public bool Unlocked;
	public float MaxVelocity;
	public float Accelleration;
	public float RotationSpeed;
	public float LinearDampening;
	public float CameraOffset = -20;
}

public enum ResourceType
{
	A, B, C, D
}

//public record ResourceInfo(ResourceType Type, string Name, Color color);

public class MiningResourceStorage
{
	public static Dictionary<ResourceType, Color> ResourceInfos = new()
	{
		{ResourceType.A, Color.red},
		{ResourceType.B, Color.yellow},
		{ResourceType.C, Color.green},
		{ResourceType.D, Color.blue}
	};

	public static HashSet<ResourceType> Discovered = new();
	public Dictionary<ResourceType, uint> Current = new();

	public event Action<ResourceType> OnResourceDiscovered;
	public event Action<ResourceType, uint> OnResourceCollected;
	public event Action<ResourceType, uint> OnResourceSpent;
	public event Action<ResourceType, int> OnResourceAmountChanged;

	public void CollectResource(ResourceType type, uint amount)
	{
		if (!Discovered.Contains(type))
		{
			Discovered.Add(type);
			Current[type] = 0;
			OnResourceDiscovered(type);
		}
		var collected = Math.Clamp(amount + Current[type], 0, uint.MaxValue);
		Current[type] = collected;
		OnResourceCollected(type, collected);
		OnResourceAmountChanged(type, (int)collected);
	}
	public void SpendResource(ResourceType type, uint amount)
	{
		if (!Current.ContainsKey(type))
			return;
		var spent = Math.Clamp(Current[type] - amount, 0, uint.MaxValue);
		Current[type] = spent;
		OnResourceSpent(type, spent);
		OnResourceAmountChanged(type, -((int)spent));
	}
	public bool CanAfford(ResourceType type, uint amount)
	{
		if(amount == 0)
			return true;
		Debug.Log("can afford check for " + type + " with amount " + amount + ", current is " + (Current.ContainsKey(type) ? Current[type].ToString() : "0"));
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
		return (Discovered.Contains(type));
	}
	public uint GetResourceAmount(ResourceType type)
	{
		return Current.ContainsKey(type) ? Current[type] : 0;
	}

	internal bool AnyResourceDiscovered()
	{
		return Discovered.Any();
	}
}


public class GameUpgrades
{
	public bool RadarUnlocked = false;
	public bool SuperCruiseUnlocked = false;

	public BuyableUpgrade RadarRange = new BuyableUpgrade("Radar Range")
		.Max(5)
		.Modifier(AttributeType.RadarRange, 2f, true)
		.LevelPrice(new(10))
		.LevelPrice(new(25, 25))
		.LevelPrice(new(100, 100, 100))
		.LevelPrice(new(250, 250, 250, 250))
		.UnlockCondition(() => Global.Instance.Upgrades.RadarUnlocked)
		;


	public BuyableUpgrade SuperCruise = new BuyableUpgrade("Supercruise Speed")
		.Modifier(AttributeType.SpaceShipSuperCruiseAccelleration, 1.1f, true)
		.Modifier(AttributeType.SpaceShipSuperCruiseSpeed, 1.25f, true)
		.LevelPrice(new(25))
		.LevelPrice(new(50, 50))
		.LevelPrice(new(100, 100, 100))
		.LevelPrice(new(200, 200, 200, 200))
		.Scale(2f)
		.UnlockCondition(() => Global.Instance.Upgrades.SuperCruiseUnlocked);

	public BuyableUpgrade RotationSpeed = new BuyableUpgrade("Rotation Speed")
		.Modifier(AttributeType.SpaceShipRotationSpeed, 0.15f)
		.LevelPrice(new(100, 100))
		.LevelPrice(new(200, 200))
		.LevelPrice(new(400, 400, 400))
		.LevelPrice(new(800, 800, 800, 800))
		.Scale(2f);


	public BuyableUpgrade LifeSupport = new BuyableUpgrade("Lifesupport")
		.LevelPrice(new(100))
		.Scale(2f);

	public BuyableUpgrade LaserDamage = new BuyableUpgrade("Lifesupport")
		.LevelPrice(new(100))
		.Scale(2f);
}

public class BuyableUpgrade
{
	public ResourceLevel GetCurrentScaledCosts()
	{
		if (LevelCosts == null)
			return new ResourceLevel();

		int count = LevelCosts.Count;
		if (Level >= count)
		{
			var last = LevelCosts.Last();
			return new ResourceLevel(last, Mathf.Pow(CostScaling, Level - count));
		}
		return LevelCosts[Level];
	}

	public BuyableUpgrade(string name)
	{
		Name = name;
	}
	public BuyableUpgrade LevelPrice(ResourceLevel costs)
	{
		if (LevelCosts == null)
			LevelCosts = new();
		LevelCosts.Add(costs);

		return this;
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
	public BuyableUpgrade Scale(float scaling)
	{
		CostScaling = scaling;
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
		if(IsMaxed())
			return "Max Level Reached";

		if (!UnlockedCondition())
			return "Not found yet";

		if(!Global.Instance.SpaceshipResources.CanAfford(GetCurrentScaledCosts()))
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
		if(Modifiers != null)
		{
			foreach(var mod in Modifiers)
			{
				PersistentPlayer.AddModifier(mod);
			}
		}
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
	public float CostScaling = 1.5f;
	public List<ResourceLevel> LevelCosts;
	public List<AttributeModifier> Modifiers;
	public Action OnPurchase;
}
public struct ResourceLevel : IEnumerable
{
	public uint A, B, C, D;

	public ResourceLevel(ResourceLevel prev, float scaling)
	{
		A = (uint)(prev.A * scaling);
		B = (uint)(prev.B * scaling);
		C = (uint)(prev.C * scaling);
		D = (uint)(prev.D * scaling);
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
		yield return new(ResourceType.A, A);
		yield return new(ResourceType.B, B);
		yield return new(ResourceType.C, C);
		yield return new(ResourceType.D, D);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
