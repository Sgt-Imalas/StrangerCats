using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class HoldButton : CatPawButton
{
	bool pressed,Selected;
	public float holdTime = 2f;
	public UnityEvent onHoldComplete;

	//public InputActionReference submitAction;

	private bool isHolding;
	private Coroutine holdRoutine;
	private PlayerControls controls;
	bool MouseMode = true;
	public Image FillImage;

	protected override void OnEnable()
	{
		base.OnEnable();
		controls.UI.Enable();
		controls.UI.Submit.performed += ctx => StartHoldIfSelected();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		Selected = true;
		base.OnSelect(eventData);
	}
	public override void OnDeselect(BaseEventData eventData)
	{
		Selected = false;
		base.OnDeselect(eventData);
	}

	void StartHoldIfSelected()
	{
		if (Selected)
		{
			MouseMode = false;
			StartHold();
		}
	}

	protected override void OnDisable()
	{ 
		base.OnDisable();
		controls.UI.Disable();
	}

	protected override void Awake()
	{ 
		base.Awake();
		controls = new();
		FillImage = transform.Find("Fill").GetComponent<Image>();
	}
	public override void OnPointerDown(PointerEventData eventData)
	{
		MouseMode = true;
		StartHold();
		base.OnPointerDown(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		CancelHold();
		base.OnPointerUp(eventData);
	}

	private void StartHold()
	{
		if (isHolding)
			return;

		isHolding = true;
		holdRoutine = StartCoroutine(HoldCoroutine());
	}

	private void CancelHold()
	{
		if (!isHolding)
			return;

		isHolding = false;

		if (holdRoutine != null)
			StopCoroutine(holdRoutine);
		FillImage.fillAmount = 0;
	}

	private IEnumerator HoldCoroutine()
	{
		float timer = 0f;

		while (timer < holdTime)
		{
			if (!IsSubmitStillPressed())
			{
				CancelHold();
				yield break;
			}

			timer += Time.unscaledDeltaTime;
			FillImage.fillAmount = timer / holdTime;
			yield return null;
		}

		isHolding = false;
		onHoldComplete?.Invoke();
	}

	private bool IsSubmitStillPressed()
	{
		return MouseMode || controls.UI.Submit.IsPressed();
	}
}
