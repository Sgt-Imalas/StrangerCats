using Unity.VisualScripting;
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
	public bool LoadingScene = false;

	public class StarmapShip
	{
		public Vector2 Position = new(0,0);
		public Quaternion Rotation = new();

		public FlightStats PrecisionMode = new()
		{
			Unlocked = true,
			MaxVelocity = 30f,
			Accelleration = 26f,
			RotationSpeed = 240f,
			LinearDampening = 0.5f,
			CameraOffset = -20
		};
		public FlightStats CruiseMode = new()
		{
			Unlocked = true,//tmp
			MaxVelocity = 100f,
			Accelleration = 100f,
			RotationSpeed = 180f,
			LinearDampening = 0.5f,
			CameraOffset = -50
		};

		public bool InPrecisionFlightMode = true;
		public bool CanLand = true;
		public bool BlockedFromLanding => !CanLand || !InPrecisionFlightMode;
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
}
