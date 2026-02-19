using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShipControllerStarmap : MonoBehaviour
{
	private PlayerControls controls;
	public Camera mainCamera;
	public float RotationSpeed = 180;
	public float AccellerationSpeed = 28f;
	public float MaxVelocity = 60f;
	public bool PrecisionFlyMode = true;
	public Vector2 movementDirection, LookPosition;
	public CameraAnimator CameraAnimator;

	Rigidbody2D rb;
	Vector2 CurrentThrust;

	List<ParticleSystem> CruiseEngineEmissions;
	List<ParticleSystem> PrecisionEngineEmissions;

	public float stickDeadzone = 0.3f;
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

		controls.Player.Jump.performed += OnModeChange;

		ApplyModeChanges(Global.Instance.Spaceship.PrecisionMode);
		PrecisionEngineEmissions = transform.GetComponentsInChildren<ParticleSystem>()
			.Where(ps => ps.tag == "PrecisionModeEngine")
			.ToList();
		CruiseEngineEmissions = transform.GetComponentsInChildren<ParticleSystem>()
			.Where(ps => ps.tag == "CruiseModeEngine")
			.ToList();
	}

	private void Start()
	{
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
		if (context.canceled || value.magnitude < stickDeadzone)
		{
			movementDirection = Vector2.zero;
		}
		else
		{
			movementDirection = context.ReadValue<Vector2>();
		}
	}

	void OnLook(InputAction.CallbackContext context)
	{
		ControllerAim = context.control.device is Gamepad;
		//Debug.Log("Look triggered: " + context.ReadValue<Vector2>()+", device: "+ context.control.device+" is controller: "+ ControllerAim);
		var value = context.ReadValue<Vector2>();
		if (context.canceled || value.magnitude < stickDeadzone)
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
		if (!Global.Instance.Spaceship.CruiseMode.Unlocked || Global.Instance.LockedInputs)
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
	void ApplyModeChanges(Global.FlightStats mode)
	{
		RotationSpeed = mode.RotationSpeed;
		AccellerationSpeed = mode.Accelleration;
		MaxVelocity = mode.MaxVelocity;

		rb.linearDamping = mode.LinearDampening;

		Global.Instance.Spaceship.CurrentMode = mode;

		if (CameraAnimator != null)
			CameraAnimator.AnimateOffsetChange(mode.CameraOffset, 0.25f);
	}

	private void FixedUpdate()
	{
		if (Global.Instance.LockedInputs) return;

		// rotation
		Vector2 direction = LookPosition;
		if (PrecisionFlyMode)
		{
			if (!ControllerAim)
			{
				Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(new(LookPosition.x, LookPosition.y, -mainCamera.transform.position.z));
				direction = mouseWorld - transform.position;
			}
		}
		else
		{
			direction = movementDirection;
		}

			bool stillRotating = false;
		if (direction != Vector2.zero)
		{
			float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			float currentAngle = transform.eulerAngles.z;
			float diff = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
			if (diff > 45f)
				stillRotating = true;
			float newAngle = Mathf.MoveTowardsAngle(
				currentAngle,
				targetAngle,
				RotationSpeed * Time.fixedDeltaTime
			);
			transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

		}

		//movement
		CurrentThrust = Vector2.zero;
		if (!PrecisionFlyMode && !stillRotating)
		{
			if (movementDirection != Vector2.zero)
				CurrentThrust = 60 * Time.fixedDeltaTime * AccellerationSpeed * rb.mass * movementDirection;
			else
				rb.linearVelocity *= 1f - 0.5f * Time.fixedDeltaTime;
		}
		else if (PrecisionFlyMode)
		{
			var rotatedDirection = Quaternion.Euler(0, 0, -90) * (transform.rotation * movementDirection);
			CurrentThrust = 60 * Time.fixedDeltaTime * AccellerationSpeed * rb.mass * rotatedDirection;
		}

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

	}
	void RefreshEngineParticles()
	{
		float emissionRate = CurrentThrust.magnitude;
		for (int i = 0; i < PrecisionEngineEmissions.Count; i++)
		{
			var emission = PrecisionEngineEmissions[i].emission;
			emission.rateOverTime = 0; // PrecisionFlyMode ? emissionRate : 0;
			emission.rateOverDistance = PrecisionFlyMode ? emissionRate : 0;
		}
		for (int i = 0; i < CruiseEngineEmissions.Count; i++)
		{
			var emission = CruiseEngineEmissions[i].emission;
			emission.rateOverTime = 0; // PrecisionFlyMode ? 0 : emissionRate;
			emission.rateOverDistance = PrecisionFlyMode ? 0 : emissionRate;
		}
	}
}
