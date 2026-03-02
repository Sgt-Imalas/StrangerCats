using UnityEngine;

namespace Assets.Scripts.UI
{
	public class MainMenu : MonoBehaviour
	{
		public void LoadStarmap() => SceneLoader.Instance.LoadGame();

		public void LoadCredits()
		{
			if (SceneLoader.Instance == null)
				Debug.Log("wtf");
			SceneLoader.Instance.LoadCredits();
		}

		public void ExitGame() => SceneLoader.Instance.ExitGame();
	}
}
