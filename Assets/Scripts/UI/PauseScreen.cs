using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
	public GameObject PauseMenuUi;
	public static bool IsCurrentlyPaused = false, CanCurrentlyPause = true;
	public Slider MusicSlider, SfxSlider;
	public Button Resume, Exit;
	private PlayerControls controls;

	private void OnEnable()
	{
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	private void Awake()
	{
		controls = new();
		controls.Player.TogglePauseScreen.performed += OnPauseScreenToggled;

		if (PlayerPrefs.HasKey("MusicVolume"))
			MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
		if (PlayerPrefs.HasKey("SfxVolume"))
			SfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
		MusicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
		SfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
		Resume.onClick.AddListener( () => PauseGame(false));
		Exit.onClick.AddListener( () => SceneManager.LoadScene("MainMenu"));
		PauseGame(false);
	}
	void OnMusicVolumeChanged(float newValue)
	{
		MusicManager.SetMusicVolume(newValue);
		PlayerPrefs.SetFloat("MusicVolume", newValue);
	}
	void OnSfxVolumeChanged(float newValue)
	{
		MusicManager.SetSFXVolume(newValue);
		PlayerPrefs.SetFloat("SfxVolume", newValue);
	}



	private void OnDestroy()
	{
		PauseGame(false);
	}

	void OnPauseScreenToggled(InputAction.CallbackContext context) => PauseGame(!IsCurrentlyPaused);
	void PauseGame(bool setPaused)
	{
		if (!CanCurrentlyPause || Global.Instance.InUpgradeMenu)
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
