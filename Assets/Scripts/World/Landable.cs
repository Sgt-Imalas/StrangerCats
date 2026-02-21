using Assets.Scripts;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Landable : Interactable
{
	public string SceneToLoad;
	public PlanetDescriptor planetDescriptor;

	public override void OnRadiusEnter(Collider2D collision)
	{
		base.OnRadiusEnter(collision);

		InteractHint.SetText("Land here");
	}
	public override void OnInteractPressed()
	{
		base.OnInteractPressed();
		ToggleInteract(false);
		LandWithAnimation();
	}


	void LandWithAnimation()
	{
		if (Global.Instance.LoadingScene || Global.Instance.Spaceship.BlockedFromLanding || SceneToLoad == null || !SceneToLoad.Any())
			return;
		Debug.Log("Landing on landable " + gameObject.name);
		Global.Instance.Spaceship.CanLand = false;
		Global.Instance.LoadingScene = true;
		if (currentPlayer != null && currentPlayer.TryGetComponent<Rigidbody2D>(out var rb))
		{
			rb.linearVelocity = Vector3.zero;
		};

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
		Global.Instance.Spaceship.CanLand = true;
		LoadOverlay.ShowOverlay();
		SceneManager.sceneLoaded += OnSceneLoadFinished;
		SceneManager.LoadScene(SceneToLoad);
	}

	void OnSceneLoadFinished(Scene s, LoadSceneMode mode)
	{
		LoadOverlay.ShowOverlay(false);
		Global.Instance.LoadingScene = false;
		SceneManager.sceneLoaded -= OnSceneLoadFinished;
		Global.Instance.loadPlanet = planetDescriptor;
	}
}
