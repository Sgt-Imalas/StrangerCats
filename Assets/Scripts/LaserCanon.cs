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

	public LayerMask layerMask;

	private Vector3 lastAimDirection;

	public Transform selectionMarker;
	public LineRenderer laserRenderer;
	public Transform sparkLight;
	public AudioSource impactSparklerSound;

	// TODO: pool
	public Rigidbody2D projectilePrefab;

	public float projectileCooldown = 0.1f;
	private float _timeSinceLastProjectile;

	bool ControllerAim;


	private float cachedLaserRange = 10.0f;

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
		if (!context.canceled || value.magnitude >= Global.StickDeadzone)
		{
			LookPosition = value;
		}
	}

	void Start()
	{
		PersistentPlayer.Instance.attributes.OnAttributeChanged += OnPlayerAttributeChanged;
		projectileCooldown = PersistentPlayer.GetAttribute(AttributeType.FireRate);
		cachedLaserRange = PersistentPlayer.GetAttribute(AttributeType.LaserRange);
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
			case AttributeType.LaserRange:
				cachedLaserRange = value;
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
		/*		var bullet = Object.Instantiate(projectilePrefab);
				bullet.transform.position = tipMarker.position;

				bullet.gameObject.SetActive(true);
				var rotatedDirection = Quaternion.Euler(0, 0, 0) * lastAimDirection;
				//bullet.AddForce(projectileSpeed * rotatedDirection);
				bullet.GetComponent<Attributes>().SetBaseValue(AttributeType.ExplosionRadius, explosionRadius, 0, 999);
				bullet.linearVelocity = projectileSpeed * rotatedDirection;
		*/


		//var dir = (Vector2)(mousePosition - tipMarker.position);

		var dir = lastAimDirection;

		var hit = Physics2D.BoxCast(
					transform.position,
					new Vector2(0.1f, 0.2f),
					0f,
					dir,
					500.0f,
					layerMask
				);

		if (hit.collider != null)
		{
			var damage = PersistentPlayer.GetAttribute(AttributeType.DigDamage);
			if (hit.collider.TryGetComponent(out Health health))
			{
				health.Damage(damage);
			}
			else
			{
				var insidePoint = hit.point - hit.normal;
				DestructibleTerrain.Instance.DamageTileAt(insidePoint, damage);
			}
		}

		_timeSinceLastProjectile = 0.0f;
	}


	void Update()
	{
		aimingLine.SetPosition(0, tipMarker.position);
		aimingLine.SetPosition(1, mousePosition);

		var dir = (Vector2)(mousePosition - tipMarker.position);

		var hit = Physics2D.BoxCast(
					transform.position,
					new Vector2(0.1f, 0.2f),
					0f,
					dir,
					cachedLaserRange,
					layerMask
				);

		var isHittingTile = hit.collider != null;

		if (isHittingTile)
		{
			var insidePoint = hit.point - hit.normal;
			selectionMarker.gameObject.SetActive(true);
			selectionMarker.position = DestructibleTerrain.Instance.GetTileCenter(insidePoint);
		}
		else
		{
			selectionMarker.gameObject.SetActive(false);
		}

		var isMouseDown = controls.Player.Attack.IsPressed() && Time.timeScale > 0;

		if (isMouseDown)
		{
			PersistentPlayer.Instance.LaserFiring = true;
			if (isHittingTile)
			{
				laserRenderer.SetPosition(0, tipMarker.position);
				laserRenderer.SetPosition(1, hit.point);

				var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

				sparkLight.transform.SetPositionAndRotation(hit.point, Quaternion.Euler(0f, 0f, angle + 90f));

				if (!impactSparklerSound.isPlaying)
					impactSparklerSound.Play();
			}
			else
			{
				var endPoint = transform.position + (Vector3)dir.normalized * cachedLaserRange;

				laserRenderer.SetPosition(0, tipMarker.position);
				laserRenderer.SetPosition(1, endPoint);

				var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

				sparkLight.transform.SetPositionAndRotation(endPoint, Quaternion.Euler(0f, 0f, angle + 90f));

				impactSparklerSound.Stop();
			}

			if (!laserRenderer.gameObject.activeSelf)
			{
				laserRenderer.gameObject.SetActive(true);
				sparkLight.gameObject.SetActive(true);
			}
		}
		else
		{

			laserRenderer.gameObject.SetActive(false);
			sparkLight.gameObject.SetActive(false);
			impactSparklerSound.Stop();
			PersistentPlayer.Instance.LaserFiring = false;
		}

		if (isMouseDown && _timeSinceLastProjectile > projectileCooldown && isMouseDown)
		{
			OnAttack(default);
		}

		_timeSinceLastProjectile += Time.deltaTime;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		var direction = LookPosition;
		if (!ControllerAim)
		{
			var mouseWorld = mainCamera.ScreenToWorldPoint(new(LookPosition.x, LookPosition.y, -mainCamera.transform.position.z));
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
			var rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			//float diff = Mathf.Abs(Mathf.DeltaAngle(currentAngle, rot_z));

			lastAimDirection = direction.normalized;
			transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90.0f);

		}
	}

}
