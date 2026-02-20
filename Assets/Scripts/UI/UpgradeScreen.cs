using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeScreen : MonoBehaviour
{
	private PlayerControls controls;
	public float TimePressed=0;
	bool pressed;
	private void Awake()
	{
		controls = new();
		controls.UI.Click.started += OnConfirmButtonChanged;
		controls.UI.Click.canceled += OnConfirmButtonChanged;
		controls.UI.Click.performed += OnConfirmButtonChanged;

	}
	private void OnEnable()
	{
		Time.timeScale = 0;
		Global.Instance.InMenu = true;
		PauseScreen.CanCurrentlyPause = false;
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		Time.timeScale = 1;
		Global.Instance.InMenu = false;
		PauseScreen.CanCurrentlyPause = true;
		controls.Player.Disable();
	}
	void OnConfirmButtonChanged(InputAction.CallbackContext context)
	{
		if (context.started)
			pressed = true;
		else
		if (context.canceled)
		{
			pressed = false;
			TimePressed = 0;
		}
		else if (context.performed)
		{
			TimePressed = 0;
		}

	}
	private void Update()
	{
		if(pressed)
			TimePressed += 1;
	}
}
