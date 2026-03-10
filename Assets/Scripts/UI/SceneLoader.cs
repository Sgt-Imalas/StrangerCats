using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static SceneLoader Instance { get; private set; }

	public UIFader fader;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Debug.LogWarning($"SceneLoader instance already exists, destroying duplicate ({Instance.name} stays, {name} deleted.) ");
			Destroy(gameObject);

			return;
		}
	}

	public void LoadMainMenu()
	{
		Global.Instance.StartLoadingMainMenu();
	}

	public void LoadGame()
	{
		Global.LoadPersistantInstance();
		LoadSceneByName("Starmap");
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	private UIFader GetFader()
	{
		if (fader == null)
			fader = Object.FindFirstObjectByType<UIFader>();

		return fader;
	}

	public void LoadSceneByName(string name, bool fade = true)
	{
		if (fade)
		{
			var fader = GetFader();
			if (fader != null)
			{
				fader.FadeIn();
				fader.onFadeFinished.AddListener(() =>
				{
					SceneManager.LoadScene(name);
					fader.onFadeFinished.RemoveAllListeners();
				});
			}
		}
		else
		{
			SceneManager.LoadScene(name);
		}
	}

	public void LoadCredits() => LoadSceneByName("Credits");
}
