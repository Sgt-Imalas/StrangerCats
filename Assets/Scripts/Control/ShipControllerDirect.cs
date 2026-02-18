using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShipControllerDirect : MonoBehaviour
{
	private PlayerControls controls;
	public Camera mainCamera;
	public float RotationSpeed = 180;
	public float AccellerationSpeed = 28f;
	public float MaxVelocity = 60f;
	public Vector2 movementDirection, LookPosition;

	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		controls = new();

		controls.Player.Move.performed += OnMove;
		controls.Player.Move.canceled += OnMove;
	}

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

	private void OnEnable()
	{
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	private void Update()
	{


		rb.AddForce(AccellerationSpeed * rb.mass * movementDirection);

		if (rb.linearVelocity.magnitude > MaxVelocity)
		{
			rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
		}

	}
}
