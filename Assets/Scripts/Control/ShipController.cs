using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShipController : MonoBehaviour
{
	private PlayerControls controls;
	public Camera mainCamera;
	public float RotationSpeed = 180;
	public float AccellerationSpeed = 28f;
	public float MaxVelocity = 60f;
	public bool FlyTowardsMouseMode = true;
	public Vector2 movementDirection, LookPosition;

	Rigidbody2D rb;

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

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		controls = new();
		controls.Player.Look.performed += OnLook;
		controls.Player.Look.canceled += OnLook;

		controls.Player.Move.performed += OnMove;
		controls.Player.Move.canceled += OnMove;
	}

	private void OnEnable()
	{
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	private void FixedUpdate()
	{
		// rotation
		Vector2 direction = movementDirection;
		if (FlyTowardsMouseMode)
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
				RotationSpeed * Time.deltaTime
			);
			transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

		}

		//movement
		if (!FlyTowardsMouseMode && !stillRotating)
		{
			rb.AddForce(AccellerationSpeed * rb.mass * movementDirection);
		}
		else if (FlyTowardsMouseMode)
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
