using Assets.Scripts;
using System;
using UnityEngine;

public class ResetAll : MonoBehaviour
{
	[SerializeField] public HoldButton HoldButton;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		HoldButton.onClick.AddListener(ResetEverything);
	}

	private void ResetEverything()
	{
		Global.Reset();
		if (PersistentPlayer.Instance != null)
			Destroy(PersistentPlayer.Instance.gameObject);
		if(GlobalEvents.Instance != null)
			Destroy(GlobalEvents.Instance.gameObject);
	}
}
