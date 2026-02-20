using UnityEditor.Overlays;
using UnityEngine;

public class LoadOverlay : MonoBehaviour
{
    public GameObject overlay; 
	static LoadOverlay instance;
	private void Awake()
	{
		instance = this;
	}
	private void OnDestroy()
	{
		instance = null;
	}
	public static void ShowOverlay(bool show = true)
    {
        if(instance == null)
        {
            return;
		}
		instance.overlay.SetActive(show);
	}
}
