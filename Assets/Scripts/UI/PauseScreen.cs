using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScreen : MonoBehaviour
{
	private PlayerControls controls;
	public GameObject PauseMenuUi;
	public static bool IsCurrentlyPaused = false, CanCurrentlyPause = true;


	private void Awake()
	{
		controls = new();
		controls.Player.TogglePauseScreen.performed += OnPauseScreenToggled;
	}
	private void OnEnable()
	{
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	void OnPauseScreenToggled(InputAction.CallbackContext context) => PauseGame(!IsCurrentlyPaused);
	void PauseGame(bool setPaused)
	{
		if (!CanCurrentlyPause)
			return;

		if (setPaused)
		{
			Time.timeScale = 0;
			PauseMenuUi.SetActive(true);
			IsCurrentlyPaused = true;
		}
		else
		{
			Time.timeScale = 1;
			PauseMenuUi.SetActive(false);
			IsCurrentlyPaused = false;
		}
	}
}
