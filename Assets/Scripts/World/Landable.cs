using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Landable : MonoBehaviour
{
	public SceneAsset SceneToLoad;
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (Global.Instance.LoadingScene)
			return;
		Debug.Log("Leaving landable "+gameObject.name);
		Global.Instance.Spaceship.CanLand = true;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Global.Instance.LoadingScene || Global.Instance.Spaceship.BlockedFromLanding)
			return;

		Debug.Log("Landing on landable " + gameObject.name);
		Global.Instance.Spaceship.CanLand = false;
		Global.Instance.LoadingScene = true;
		SceneManager.sceneLoaded += OnSceneLoadFinished;
		SceneManager.LoadScene(SceneToLoad.name);
	}
	void OnSceneLoadFinished(Scene s, LoadSceneMode mode)
	{
		Global.Instance.LoadingScene = false;
		SceneManager.sceneLoaded -= OnSceneLoadFinished;
	}
}
