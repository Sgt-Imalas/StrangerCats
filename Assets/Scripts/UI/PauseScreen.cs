using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
	private PlayerControls controls;
	public GameObject PauseMenuUi;
	public static bool IsCurrentlyPaused = false, CanCurrentlyPause = true;
	public Slider MusicSlider, SfxSlider;
	public Button Resume, Exit;


	private void Awake()
	{
		controls = new();
		controls.Player.TogglePauseScreen.performed += OnPauseScreenToggled;
		MusicSlider.value = Global.Instance.Settings.MusicVolume;
		SfxSlider.value = Global.Instance.Settings.SfxVolume;
		MusicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
		SfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
		Resume.onClick.AddListener( () => PauseGame(false));
		Exit.onClick.AddListener( () => SceneManager.LoadScene("MainMenu"));
		PauseGame(false);
	}

	void OnMusicVolumeChanged(float newValue)
	{
		Global.Instance.Settings.MusicVolume = newValue;
	}
	void OnSfxVolumeChanged(float newValue)
	{
		Global.Instance.Settings.SfxVolume = newValue;
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

		Global.Instance.InPauseMenu = setPaused;
		Resume.Select();
	}
}
