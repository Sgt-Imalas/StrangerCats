using Assets.Scripts;
using System;
using UnityEngine;

public class ResetAll : MonoBehaviour
{
	[SerializeField] public HoldButton HoldButton;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		HoldButton.onHoldComplete.AddListener(ResetEverything);
	}

	private void ResetEverything()
	{
		Debug.Log("Resetting all Progress");
		Global.DeleteSaveFile();
		if (PersistentPlayer.Instance != null)
			Destroy(PersistentPlayer.Instance.gameObject);
		if(GlobalEvents.Instance != null)
			Destroy(GlobalEvents.Instance.gameObject);
	}
}
