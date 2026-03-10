using Assets.Scripts;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeaveEarly : MonoBehaviour
{
	public HoldButton HoldButton;
	private PlayerControls controls;
	public MothershipAnimator ShipAnim;
	TextMeshProUGUI label;
	bool canLeave;
	const float buttonHoldTime = 0.75f;


	private void Awake()
	{
		controls = new();
		HoldButton.holdTime = buttonHoldTime;
		HoldButton.onHoldComplete.AddListener(OnButtonTriggered);
		controls.Player.LeaveLevelEarly.started += ctx =>
		{
			StartHold();
		};
		controls.Player.LeaveLevelEarly.canceled += ctx =>
		{
			CancelHold();
		};
		label = HoldButton.GetComponentInChildren<TextMeshProUGUI>();
		HoldButton.gameObject.SetActive(true);
		Global.Instance.Upgrades.OnItemCollected += OnItemFound;
		Global.Instance.FlightStateChanged += OnFlightStateChanged;


	}
	private void Start()
	{
		GlobalEvents.Instance.OnPlayerEnergyDepleted += TeleportToShip;
		OnFlightStateChanged(Global.PlayerState.Landed);
	}

	void OnButtonTriggered()
	{
		if (canLeave)
			ShipAnim.AnimateLeaving();
		else
			TeleportToShip();
	}
	void TeleportToShip()
	{
		ShipAnim.TeleportPlayerToHangar(false);
	}


	private void OnFlightStateChanged(Global.PlayerState state)
	{
		if (state != Global.PlayerState.Landed)
			return;

		canLeave = PersistentPlayer.Instance.InHangar && Global.Instance.Upgrades.RadarUnlocked;
		label.text = canLeave ? "Leave Planet" : "Return to Ship";
	}

	private void OnDestroy()
	{
		Global.Instance.Upgrades.OnItemCollected -= OnItemFound;
		Global.Instance.FlightStateChanged -= OnFlightStateChanged;
		GlobalEvents.Instance.OnPlayerEnergyDepleted -= TeleportToShip;
	}

	private void OnItemFound(FindableItem item)
	{
		//HoldButton.gameObject.SetActive(Global.Instance.Upgrades.RadarUnlocked);
	}

	bool HeldEnough = false;
	Coroutine handle = null;
	private void StartHold()
	{
		if (!HoldButton.isActiveAndEnabled)
			return;

		handle = StartCoroutine(AnimateHolding());
	}
	void CancelHold()
	{
		if (!HoldButton.isActiveAndEnabled)
			return;
		StopCoroutine(handle);
		HoldButton.FillImage.fillAmount = 0f;

		if (HeldEnough)
		{
			OnButtonTriggered();
		}
		HeldEnough = false;
	}

	private IEnumerator AnimateHolding()
	{
		float timer = 0f;
		float holdTime = buttonHoldTime;
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
