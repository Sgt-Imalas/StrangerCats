using UnityEngine;

public class UpgradeScreen : MonoBehaviour
{

	private void OnEnable()
	{
		Time.timeScale = 0;
		Global.Instance.InMenu = true;
		PauseScreen.CanCurrentlyPause = false;
	}

	private void OnDisable()
	{
		Time.timeScale = 1;
		Global.Instance.InMenu = false;
		PauseScreen.CanCurrentlyPause = true;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
