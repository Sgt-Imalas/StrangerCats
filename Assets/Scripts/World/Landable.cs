using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Landable : MonoBehaviour
{
	public SceneAsset SceneToLoad;
	//move this to global
	bool isLanded = false;
	private void OnTriggerExit2D(Collider2D collision)
	{
		isLanded = false;
		Debug.Log("Exit");
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isLanded)
			return;
		isLanded = true;

		SceneManager.LoadScene(SceneToLoad.name);
	}
}
