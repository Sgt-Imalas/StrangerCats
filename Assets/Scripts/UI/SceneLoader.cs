using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public  void LoadMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
	public void StartGame()
	{
		SceneManager.LoadScene("Starmap");
	}
}
