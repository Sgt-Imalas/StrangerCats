using Assets.Scripts;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Landable : MonoBehaviour
{
	public string SceneToLoad;
	public PlanetDescriptor planetDescriptor;

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player")
			return;
		if (Global.Instance.LoadingScene)
			return;
		Debug.Log("Leaving landable " + gameObject.name);
		Global.Instance.Spaceship.CanLand = true;
	}

	void Start()
	{
		if (PlanetToGenerate == null)
			return;

		var renderer = GetComponent<SpriteRenderer>();
		var icon = PlanetToGenerate.icon;

		if (icon != null)
		{
			var sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector3.zero);
			renderer.sprite = sprite;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player")
			return;

		if (Global.Instance.LoadingScene || Global.Instance.Spaceship.BlockedFromLanding || SceneToLoad == null || !SceneToLoad.Any())
			return;

		Debug.Log("Landing on landable " + gameObject.name);
		Global.Instance.Spaceship.CanLand = false;
		Global.Instance.LoadingScene = true;
		if (collision.TryGetComponent<Rigidbody2D>(out var rb))
		{
			rb.linearVelocity = Vector3.zero;
		}
		;

		if (Camera.main.TryGetComponent<CameraAnimator>(out var animator))
		{
			animator.AnimateOffsetChange(-0.3f, 0.5f, StartLoadingScene, true);
		}
		else
		{
			StartLoadingScene();
		}
	}
	void StartLoadingScene()
	{
		SceneManager.sceneLoaded += OnSceneLoadFinished;
		SceneManager.LoadScene(SceneToLoad);
	}

	void OnSceneLoadFinished(Scene s, LoadSceneMode mode)
	{
		Global.Instance.LoadingScene = false;
		SceneManager.sceneLoaded -= OnSceneLoadFinished;

		Global.Instance.loadPlanet = planetDescriptor;
	}
}
