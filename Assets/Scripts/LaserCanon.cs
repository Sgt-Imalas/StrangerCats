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
		controls = new();
		controls.Player.Look.performed += OnLook;
		controls.Player.Look.canceled += OnLook;
	}


	void Update()
	{
		aimingLine.SetPosition(0, tipMarker.position);
		aimingLine.SetPosition(1, mousePosition);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		mousePosition.z = transform.position.z;

		var diff = mousePosition - transform.position;
		diff.Normalize();
		var rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
	}

}
