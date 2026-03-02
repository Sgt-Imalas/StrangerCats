using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipControllerStarmap2 : MonoBehaviour
{
	private PlayerControls controls;
	public Camera mainCamera;
	public float RotationSpeed = 180;
	public float AccellerationSpeed = 28f;
	public float MaxVelocity = 60f;
	public bool PrecisionFlyMode = true;
	public Vector2 movementInput, LookPosition;
	CameraAnimator CameraAnimator;
	public GameObject CruiseFire, SmolFire;

	Rigidbody2D rb;
	public Vector2 CurrentThrust;

	List<ParticleSystem> CruiseEngineEmissions;
	List<ParticleSystem> PrecisionEngineEmissions;

	public bool ControllerAim;
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		CameraAnimator = mainCamera.GetComponent<CameraAnimator>();

		controls = new();
		controls.Player.Look.performed += OnLook;
		controls.Player.Look.canceled += OnLook;
		controls.Player.LookController.performed += OnLook;
		controls.Player.LookController.canceled += OnLook;

		controls.Player.Move.performed += OnMove;
		controls.Player.Move.canceled += OnMove;

		controls.Player.ToggleEngineMode.performed += OnModeChange;

		ApplyModeChanges(Global.Instance.Spaceship.PrecisionMode);
		PrecisionEngineEmissions = transform.GetComponentsInChildren<ParticleSystem>()
			.Where(ps => ps.tag == "PrecisionModeEngine")
			.ToList();
		CruiseEngineEmissions = transform.GetComponentsInChildren<ParticleSystem>()
			.Where(ps => ps.tag == "CruiseModeEngine")
			.ToList();



	}

	private void OnDestroy()
	{
		GlobalEvents.Instance.OnPlayerAttributesChanged -= OnPlayerAttributesChanged;
	}
	public float CachedRotationSpeedMultiplier = 1;
	public float CachedSupercruiseBoostMultiplier = 1;

	private void OnPlayerAttributesChanged(AttributeType type, float finalValue)
	{
		switch (type)
		{
			case AttributeType.SpaceShipRotationSpeed:
				CachedRotationSpeedMultiplier = finalValue;
				ApplyModeChanges(Global.Instance.Spaceship.CurrentMode);
				break;
			case AttributeType.SpaceShipSuperCruiseSpeed:
				CachedSupercruiseBoostMultiplier = finalValue;
				ApplyModeChanges(Global.Instance.Spaceship.CurrentMode);
				break;

		}
	}

	IEnumerator FetchInitialAttributes()
	{
		yield return null;
		OnPlayerAttributesChanged(AttributeType.SpaceShipSuperCruiseSpeed, PersistentPlayer.GetAttribute(AttributeType.SpaceShipSuperCruiseSpeed));
		OnPlayerAttributesChanged(AttributeType.SpaceShipRotationSpeed, PersistentPlayer.GetAttribute(AttributeType.SpaceShipRotationSpeed));
	}


	private void Start()
	{
		GlobalEvents.Instance.OnPlayerAttributesChanged += OnPlayerAttributesChanged;
		StartCoroutine(FetchInitialAttributes());
		CameraAnimator.SetCameraOffset(-0.3f);
		CameraAnimator.AnimateOffsetChange(Global.Instance.Spaceship.CurrentMode.CameraOffset, 0.5f);
	}
	private void OnEnable()
	{
		controls.Player.Enable();
		LoadPosition();
	}

	private void OnDisable()
	{
		SavePosition();
		controls.Player.Disable();
	}
	void LoadPosition()
	{
		transform.localPosition = Global.Instance.Spaceship.Position;
		transform.rotation = Global.Instance.Spaceship.Rotation;
	}
	void SavePosition()
	{
		Global.Instance.Spaceship.Position = transform.localPosition;
		Global.Instance.Spaceship.Rotation = transform.rotation;
	}
	void OnMove(InputAction.CallbackContext context)
	{
		var value = context.ReadValue<Vector2>();
		if (context.canceled || value.magnitude < Global.StickDeadzone)
		{
			movementInput = Vector2.zero;
		}
		else
		{
			movementInput = context.ReadValue<Vector2>();
		}
	}

	void OnLook(InputAction.CallbackContext context)
	{
		ControllerAim = context.control.device is Gamepad;
		//Debug.Log("Look triggered: " + context.ReadValue<Vector2>()+", device: "+ context.control.device+" is controller: "+ ControllerAim);
		var value = context.ReadValue<Vector2>();
		if (context.canceled || value.magnitude < Global.StickDeadzone)
		{
			LookPosition = Vector2.zero;
		}
		else
		{

			LookPosition = value;
		}
	}

	void OnModeChange(InputAction.CallbackContext context)
	{
		if (!Global.Instance.Upgrades.SuperCruiseUnlocked || Global.Instance.LockedInputs)
			return;

		if (!PrecisionFlyMode && Global.Instance.Spaceship.CurrentVelocity > Global.Instance.Spaceship.PrecisionMode.MaxVelocity)
			return;


		PrecisionFlyMode = !PrecisionFlyMode;
		Global.Instance.Spaceship.InPrecisionFlightMode = PrecisionFlyMode;
		if (PrecisionFlyMode)
		{
			ApplyModeChanges(Global.Instance.Spaceship.PrecisionMode);
		}
		else
		{
			ApplyModeChanges(Global.Instance.Spaceship.CruiseMode);
		}
	}
	void ApplyModeChanges(FlightStats mode)
	{
		RotationSpeed = mode.RotationSpeed * CachedRotationSpeedMultiplier;
		AccellerationSpeed = mode.Accelleration;
		MaxVelocity = mode.MaxVelocity;

		if (!PrecisionFlyMode)
		{
			AccellerationSpeed *= CachedSupercruiseBoostMultiplier;
			MaxVelocity *= CachedSupercruiseBoostMultiplier;
		}


		rb.linearDamping = mode.LinearDampening;
		if (Global.Instance.Spaceship.CurrentMode != mode)
		{
			Global.Instance.Spaceship.CurrentMode = mode;
			if (CameraAnimator != null)
				CameraAnimator.AnimateOffsetChange(mode.CameraOffset, 0.25f);
		}
	}

	private void FixedUpdate()
	{
		if (Global.Instance.LockedInputs) return;


		// rotation
		float rotateTowards = -movementInput.x;

		if (rotateTowards != 0)
		{
			float rotationDelta = rotateTowards * RotationSpeed * Time.fixedDeltaTime;
			transform.Rotate(Vector3.forward, rotationDelta);
		}

		//movement
		float thrustStrength = movementInput.y;
		if (thrustStrength < 0)
			thrustStrength /= 4f;

		var direction = transform.right * thrustStrength;

		CurrentThrust = 60 * Time.fixedDeltaTime * AccellerationSpeed * rb.mass * direction;


		if (CurrentThrust != Vector2.zero)
		{
			rb.AddForce(CurrentThrust);
		}
		RefreshEngineParticles();

		if (rb.linearVelocity.magnitude > MaxVelocity)
		{
			rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
		}
		Global.Instance.Spaceship.CurrentVelocity = rb.linearVelocity.magnitude;
		Global.Instance.Spaceship.CurrentVelocityVectorClamped = rb.linearVelocity;

	}
	void RefreshEngineParticles()
	{
		var emissionRate = Mathf.Clamp(CurrentThrust.magnitude, 0, 100);
		for (var i = 0; i < PrecisionEngineEmissions.Count; i++)
		{
			var emission = PrecisionEngineEmissions[i].emission;
			emission.rateOverTime = 0; // PrecisionFlyMode ? emissionRate : 0;
			emission.rateOverDistance = PrecisionFlyMode ? emissionRate : 0;
		}
		for (var i = 0; i < CruiseEngineEmissions.Count; i++)
		{
			var emission = CruiseEngineEmissions[i].emission;
			emission.rateOverTime = 0; // PrecisionFlyMode ? 0 : emissionRate;
			emission.rateOverDistance = PrecisionFlyMode ? 0 : emissionRate;
		}
		if (CruiseFire != null)
			CruiseFire.SetActive(!PrecisionFlyMode && emissionRate > 0);
		if (SmolFire != null)
			SmolFire.SetActive(PrecisionFlyMode && emissionRate > 0);
	}
}
