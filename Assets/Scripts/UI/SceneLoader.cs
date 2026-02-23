using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public void LoadMainMenu()
	{
		Global.Instance.StartLoadingMainMenu();
	}
	public void StartGame()
	{
		SceneManager.LoadScene("Starmap");
	}
	public void LoadGame()
	{
		Global.LoadPersistantInstance();
		SceneManager.LoadScene("Starmap");
	}
	public void ExitGame()
	{
		Application.Quit();
	}
}
