using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class DelayedSceneLoadTrigger : MonoBehaviour
{
	System.Action OnSceneLoaded = null, OnSceneLoadedAfterDelay = null;
	float Delay = 0.1f;

	private static DelayedSceneLoadTrigger Instance;
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			Debug.Log("DelayedSceneLoadTrigger instance created");
		}
		else
		{
			Debug.Log("DelayedSceneLoadTrigger instance already exists, destroying duplicate");
			Destroy(gameObject);
			return;
		}
		Init();
	}

	void Init()
	{
		SceneManager.sceneLoaded += OnSceneHasLoaded;
	}

	void OnSceneHasLoaded(Scene s, LoadSceneMode mode)
	{
		StartCoroutine(DoCachedActionsWithDelay());
	}
	IEnumerator DoCachedActionsWithDelay()
	{
		if (OnSceneLoaded != null)
		{
			OnSceneLoaded.Invoke();
			OnSceneLoaded = null;
		}
		if (OnSceneLoadedAfterDelay != null)
		{
			yield return new WaitForSeconds(Delay);
			OnSceneLoadedAfterDelay.Invoke();
			OnSceneLoadedAfterDelay = null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="OnSceneLoaded">executes immediatly after the scene is loaded</param>
	/// <param name="OnSceneLoadedAfterDelay">executes after a delay</param>
	/// <param name="delay">delay for delay action in s</param>
	public static void SetNextSceneLoadActions(System.Action onSceneLoaded, System.Action onSceneLoadedAfterDelay = null, float delay = 0.1f)
	{
		if(Instance == null)
		{
			Debug.LogWarning("No instance of DelayedSceneLoadTrigger found. Cannot set next scene load actions.");
			return;
		}

		Instance.Delay = delay;
		Instance.OnSceneLoaded = onSceneLoaded;
		Instance.OnSceneLoadedAfterDelay = onSceneLoadedAfterDelay;
	}
}
