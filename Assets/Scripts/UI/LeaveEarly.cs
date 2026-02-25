using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeaveEarly : MonoBehaviour
{
	public HoldButton HoldButton;
	private PlayerControls controls;
	private void Awake()
	{
        controls = new();
		HoldButton.holdTime = 1.4f;
		HoldButton.onHoldComplete.AddListener(() =>
		{
			Global.StartLoadingStarmapScene();
		});
		controls.Player.LeaveLevelEarly.started += ctx =>
		{
			StartHold();
		};
		controls.Player.LeaveLevelEarly.canceled += ctx =>
		{
			CancelHold();
		};
	}

	bool HeldEnough = false;
	Coroutine handle = null;
	private void StartHold()
	{
		handle = StartCoroutine(AnimateHolding());
	}
	void CancelHold()
	{
		StopCoroutine(handle);
		HoldButton.FillImage.fillAmount = 0f;

		if(HeldEnough)
			Global.StartLoadingStarmapScene();
		HeldEnough = false;
	}

	private IEnumerator AnimateHolding()
	{
		float timer = 0f;
		float holdTime = 1.4f;
		while (timer < holdTime)
		{
			timer += Time.unscaledDeltaTime;
			HoldButton.FillImage.fillAmount = timer / holdTime;
			;
			yield return null;
		}
		HeldEnough = true;
	}

	private void OnEnable()
	{
		controls.Player.Enable();
	}
	private void OnDisable()
	{
		controls.Player.Disable();
	}
}
