using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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

	Rigidbody2D rb;
	Follower CamFollower;

	void OnMove(InputAction.CallbackContext context)
	{
		if (context.canceled)
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
		if (context.canceled)
		{
			LookPosition = Vector2.zero;
		}
		else
		{
			LookPosition = context.ReadValue<Vector2>();
		}
	}

	void OnModeChange(InputAction.CallbackContext context)
	{
		Debug.Log("Mode change");
		if (!Global.Instance.Spaceship.CruiseMode.Unlocked)
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

		if (CamFollower != null)
			StartCoroutine(ChangeCameraZoom(mode.CameraOffset));
	}

	IEnumerator ChangeCameraZoom(float targetOffset)
	{
		if (CamFollower == null)
			yield break;

		float currentOffset = CamFollower.Offset;
		if(currentOffset == targetOffset)
			yield break;
		if (currentOffset < targetOffset)
		{
			while(CamFollower.Offset < targetOffset)
			{
				CamFollower.Offset++;
				yield return new WaitForSeconds(0.01f);
			}
		}
		else if (currentOffset > targetOffset)
		{
			while (CamFollower.Offset > targetOffset)
			{
				CamFollower.Offset--;
				yield return new WaitForSeconds(0.01f);
			}
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		CamFollower = mainCamera.GetComponent<Follower>();

		controls = new();
		controls.Player.Look.performed += OnLook;
		controls.Player.Look.canceled += OnLook;

		controls.Player.Move.performed += OnMove;
		controls.Player.Move.canceled += OnMove;

		controls.Player.Jump.performed += OnModeChange;

		ApplyModeChanges(Global.Instance.Spaceship.PrecisionMode);
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

	private void FixedUpdate()
	{
		// rotation
		Vector2 direction = movementDirection;
		if (PrecisionFlyMode)
		{
			Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(new(LookPosition.x, LookPosition.y, -mainCamera.transform.position.z));
			direction = mouseWorld - transform.position;
		}

		bool stillRotating = false;
		if (direction != Vector2.zero)
		{
			float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			float currentAngle = transform.eulerAngles.z;
			if (Mathf.Abs((currentAngle - targetAngle) % 360) > 45f)
				stillRotating = true;
			float newAngle = Mathf.MoveTowardsAngle(
				currentAngle,
				targetAngle,
				RotationSpeed * Time.fixedDeltaTime
			);
			transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

		}

		//movement
		if (!PrecisionFlyMode && !stillRotating)
		{
			if (movementDirection != Vector2.zero)
				rb.AddForce(60*Time.fixedDeltaTime * AccellerationSpeed * rb.mass * movementDirection);
			else
				rb.linearVelocity *= 1f - 0.5f * Time.fixedDeltaTime;
		}
		else if (PrecisionFlyMode)
		{
			var rotatedDirection = Quaternion.Euler(0, 0, -90) * (transform.rotation * movementDirection);
			rb.AddForce(AccellerationSpeed * rb.mass * rotatedDirection);
		}


		if (rb.linearVelocity.magnitude > MaxVelocity)
		{
			rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
		}

	}
}
