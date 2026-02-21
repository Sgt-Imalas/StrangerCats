using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

// simpler direct controls for the smaller ship
[DefaultExecutionOrder(10)]
public class ShipControllerDirect : MonoBehaviour
{
	private PlayerControls controls;
	public Camera mainCamera;
	public float RotationSpeed = 180;
	public float MaxTilt = 10.0f;
	public float TiltSpeed = 10.0f;
	public float AccellerationSpeed = 28f;
	public float MaxVelocity = 60f;
	public Vector2 movementDirection, LookPosition;
	public Transform shipBody;

	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		controls = new();

		controls.Player.Move.performed += OnMove;
		controls.Player.Move.canceled += OnMove;
		
		GlobalEvents.Instance.OnPlayerAttributesChanged += OnPlayerAttributesChanged;
	}
	void OnExit(InputAction.CallbackContext context)
	{
		SceneManager.LoadScene("Starmap");

	}

	private void OnPlayerAttributesChanged(AttributeType type, float finalValue)
	{
		switch (type)
		{
			case AttributeType.PodSpeed:
				AccellerationSpeed = finalValue;
				break;
		}
	}

	void OnMove(InputAction.CallbackContext context)
	{
		var value = context.ReadValue<Vector2>();
		if (context.canceled || value.magnitude < Global.StickDeadzone)
		{
			movementDirection = Vector2.zero;
		}
		else
		{
			movementDirection = value;
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
		if (Global.Instance.LockedInputs) return;

		rb.AddForce(AccellerationSpeed * rb.mass * movementDirection);

		if (rb.linearVelocity.magnitude > MaxVelocity)
		{
			rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity);
		}

		var targetRotation = -rb.linearVelocityX;
		shipBody.rotation = Quaternion.Euler(0f, 0f, targetRotation);

		Global.Instance.Spaceship.CurrentVelocity = rb.linearVelocity.magnitude;
	}
}
