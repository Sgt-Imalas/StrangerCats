using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// simpler direct controls for the smaller ship
[DefaultExecutionOrder(10)]
public class ShipControllerDirect : MonoBehaviour
{
	private PlayerControls controls;

	[Header("Physics")]
	public float MaxTilt = 10.0f;
	public float TiltSpeed = 10.0f;
	public float AccellerationSpeed = 28f;
	public float MaxVelocity = 60f;
	public Vector2 movementDirection, LookPosition;
	public Transform shipBody;

	[Header("Graphics")]
	public GameObject jetFlame;

	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		controls = new();

		controls.Player.Move.performed += OnMove;
		controls.Player.Move.started += OnBeginMove;
		controls.Player.Move.canceled += OnMove;
		controls.Player.Move.canceled += OnStopMove;

		GlobalEvents.Instance.OnNewMapGenerated += OnNewMapGenerated;
		GlobalEvents.Instance.OnPlayerAttributesChanged += OnPlayerAttributesChanged;
	}

	private void Start()
	{
		StartCoroutine(GetAttributes());
	}
	IEnumerator GetAttributes()
	{
		yield return null;
		CachedSpeedIncreaseMultiplier = PersistentPlayer.GetAttribute(AttributeType.PodSpeed);
	}

	private void OnStopMove(InputAction.CallbackContext context)
	{
		if (Global.Instance.LockedInputs) return;
		jetFlame.SetActive(false);
	}

	private void OnBeginMove(InputAction.CallbackContext context)
	{
		if (Global.Instance.LockedInputs) return;
		jetFlame.SetActive(true);
	}

	private void OnDestroy()
	{
		GlobalEvents.Instance.OnNewMapGenerated -= OnNewMapGenerated;
		GlobalEvents.Instance.OnPlayerAttributesChanged -= OnPlayerAttributesChanged;
	}

	private void OnNewMapGenerated(Dictionary<Vector3Int, int> materials, PlanetDescriptor descriptor)
	{
		transform.position = descriptor.playerSpawnPoint;
	}

	private void OnPlayerAttributesChanged(AttributeType type, float finalValue)
	{
		switch (type)
		{
			case AttributeType.PodSpeed:
				CachedSpeedIncreaseMultiplier = finalValue;
				break;
		}
	}

	float CachedSpeedIncreaseMultiplier = 1f;

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

	private void FixedUpdate()
	{
		if (Global.Instance.LockedInputs) return;

		var force = AccellerationSpeed * CachedSpeedIncreaseMultiplier * rb.mass * movementDirection;
		force *= (60 * Time.fixedDeltaTime);
		rb.AddForce(force);

		if (rb.linearVelocity.magnitude > MaxVelocity)
		{
			rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxVelocity * CachedSpeedIncreaseMultiplier);
		}

		var targetRotation = -rb.linearVelocityX;
		shipBody.rotation = Quaternion.Euler(0f, 0f, targetRotation);

		Global.Instance.Spaceship.CurrentVelocity = rb.linearVelocity.magnitude;
		Global.Instance.Spaceship.CurrentVelocityVectorClamped = rb.linearVelocity;
	}
}
