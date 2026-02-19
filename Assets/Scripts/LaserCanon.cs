using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaserCanon : MonoBehaviour
{
	private PlayerControls controls;
	public float RotationSpeed = 180;
	public Vector2 LookPosition;
	public Camera mainCamera;
	public LineRenderer aimingLine;
	private Vector3 mousePosition;
	public Transform tipMarker;
	public float projectileSpeed = 600;

	private Vector3 lastAimDirection;

	// TODO: pool
	public Rigidbody2D projectilePrefab;

	public float projectileCooldown = 0.1f;
	private float _timeSinceLastProjectile;

	bool ControllerAim;
	float stickDeadzone = 0.1f;

	private void Awake()
	{
		controls = new();

		controls.Player.Look.performed += OnLook;
		controls.Player.LookController.performed += OnLook;
	}
	void OnLook(InputAction.CallbackContext context)
	{
		ControllerAim = context.control.device is Gamepad;
		//Debug.Log("Look triggered: " + context.ReadValue<Vector2>()+", device: "+ context.control.device+" is controller: "+ ControllerAim);
		var value = context.ReadValue<Vector2>();
		if (!context.canceled || value.magnitude >= stickDeadzone)
		{
			LookPosition = value;
		}
	}

	void Start()
	{
		PersistentPlayer.Instance.attributes.OnAttributeChanged += OnPlayerAttributeChanged;
		projectileCooldown = PersistentPlayer.Instance.attributes.Get(AttributeType.FireRate);
	}

	private void OnPlayerAttributeChanged(AttributeType attributeId, float value)
	{
		switch (attributeId)
		{
			case AttributeType.FireRate:
				projectileCooldown = PersistentPlayer.Instance.attributes.Get(AttributeType.FireRate);
				break;
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

	private void OnAttack(InputAction.CallbackContext context)
	{
		var bullet = Object.Instantiate(projectilePrefab);
		bullet.transform.position = tipMarker.position;

		bullet.gameObject.SetActive(true);
		var rotatedDirection = Quaternion.Euler(0, 0, 0) * lastAimDirection;
		//bullet.AddForce(projectileSpeed * rotatedDirection);

		bullet.linearVelocity = projectileSpeed * rotatedDirection;

		_timeSinceLastProjectile = 0.0f;
	}

	void Update()
	{
		aimingLine.SetPosition(0, tipMarker.position);
		aimingLine.SetPosition(1, mousePosition);

		if (_timeSinceLastProjectile > projectileCooldown && controls.Player.Attack.ReadValue<float>() > 0.0f)
		{
			OnAttack(default);
		}

		_timeSinceLastProjectile += Time.deltaTime;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Vector2 direction = LookPosition;
		if (!ControllerAim)
		{
			Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(new(LookPosition.x, LookPosition.y, -mainCamera.transform.position.z));
			direction = mouseWorld - transform.position;
			mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		}
		else
		{
			mousePosition = transform.position + (Vector3)(direction.normalized * 15);
		}
		mousePosition.z = transform.position.z;
		if (direction != Vector2.zero)
		{
			float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			//float diff = Mathf.Abs(Mathf.DeltaAngle(currentAngle, rot_z));

			lastAimDirection = direction.normalized;
			transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90.0f);

		}
	}

}
