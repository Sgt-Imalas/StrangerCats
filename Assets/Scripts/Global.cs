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
	public bool InCameraTransition = false;
	public bool LockedInputs =>  InCameraTransition;

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
				if(_currentMode == null)
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
}
