using Assets.Scripts;
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
	float explosionRadius = 0;

	private Vector3 lastAimDirection;

	// TODO: pool
	public Rigidbody2D projectilePrefab;

	public float projectileCooldown = 0.1f;
	private float _timeSinceLastProjectile;

	private void Awake()
	{
		controls = new();
	}

	void Start()
	{
		PersistentPlayer.Instance.attributes.OnAttributeChanged += OnPlayerAttributeChanged;
		projectileCooldown = PersistentPlayer.GetAttribute(AttributeType.FireRate);
	}

	private void OnPlayerAttributeChanged(AttributeType attributeId, float value)
	{
		switch (attributeId)
		{
			case AttributeType.FireRate:
				projectileCooldown = value;
				break;
			case AttributeType.ExplosionRadius:
				explosionRadius = value;
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
		bullet.GetComponent<Attributes>().SetBaseValue(AttributeType.ExplosionRadius, explosionRadius, 0, 999);
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
		mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		mousePosition.z = transform.position.z;

		var diff = mousePosition - transform.position;
		diff.Normalize();
		var rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

		lastAimDirection = diff;
		transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90.0f);
	}

}
