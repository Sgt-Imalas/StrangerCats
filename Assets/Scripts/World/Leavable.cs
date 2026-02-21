using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Causes the pod to return to the starmap if the zone is left;
/// </summary>
[RequireComponent(typeof(Collider2D))]

public class Leavable : MonoBehaviour
{

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player")
			return;
		if (Global.Instance.LoadingScene)
			return;
		Global.Instance.StartLoadingStarmapScene();
	}
}
