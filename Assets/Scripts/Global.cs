using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

	public bool LoadingScene = false;
	public bool InCameraTransition = false;
	public bool InMenu = false;
	public bool LockedInputs => InCameraTransition || InMenu;
	public DestructibleTerrain activeTerrain;
	public int WorldSeed;
	public PlanetDescriptor generateWorld;
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
		Unlocked = true,//tmp
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


	const int UpgradeIncrease = 25;
	public int MaxMaterialCap = 50;
	public int NumberOfUpgradesPurchased = 0;

	public static HashSet<ResourceType> Discovered = new();
	public Dictionary<ResourceType, int> Current = new();
	public Dictionary<ResourceType, int> Max = new();

	public event Action<ResourceType> OnResourceDiscovered;
	public event Action<ResourceType, int> OnResourceCollected;
	public event Action<ResourceType, int> OnResourceSpent;
	public void CollectResource(ResourceType type, int amount)
	{
		if (!Discovered.Contains(type))
		{
			Discovered.Add(type);
			Current[type] = 0;
			Max[type] = MaxMaterialCap + NumberOfUpgradesPurchased * UpgradeIncrease;
			OnResourceDiscovered(type);
		}
		var collected = Mathf.Clamp(amount + Current[type], 0, Max[type]);
		Current[type] = collected;
		OnResourceCollected(type, collected);
	}
	public void SpendResource(ResourceType type, int amount)
	{
		if (!Current.ContainsKey(type))
			return;
		var spent = Mathf.Clamp(Current[type] - amount, 0, Max[type]);
		Current[type] = spent;
		OnResourceSpent(type, spent);
	}
	public bool CanAfford(ResourceType type, int amount)
	{
		return Current.ContainsKey(type) && Current[type] >= amount;
	}
	public bool ResourceDiscovered(ResourceType type)
	{
		return (Discovered.Contains(type));
	}
	public int GetResourceAmount(ResourceType type)
	{
		return Current.ContainsKey(type) ? Current[type] : 0;
	}

	internal bool AnyResourceDiscovered()
	{
		return Discovered.Any();
	}
}
