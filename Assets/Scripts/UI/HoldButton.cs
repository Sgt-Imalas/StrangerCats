using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class HoldButton : CatPawButton
{
	bool Selected;
	public float holdTime = 1f;
	public UnityEvent onHoldComplete;

	private bool isHolding;
	private Coroutine holdRoutine;
	private PlayerControls controls;
	bool MouseMode = true;
	public Image FillImage;
	bool HeldLongEnough;

	public bool ReleaseToTrigger = true;
	Image bgImage;
	bool _buyingAllowed = true;
	public bool BuyingAllowed
	{
		get => _buyingAllowed && interactable;
		set
		{
			_buyingAllowed = value;
			image.color = value ? RegularButtonColor : LockedButtonColor;
		}
	}
	Color RegularButtonColor = new Color32(200, 200, 200, byte.MaxValue);
	Color LockedButtonColor = new Color32(235, 157, 152, byte.MaxValue);

	

	protected override void OnEnable()
	{
		base.OnEnable();
		if (controls != null)
			controls.UI.Enable();
		if (controls != null)
			controls.UI.Submit.performed += ctx => StartHoldIfSelected();

		if (FillImage != null)
			FillImage.fillAmount = 0;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		if (controls != null)
			controls.UI.Disable();
	}


	public override void OnSelect(BaseEventData eventData)
	{
		if (!interactable)
			return;
		Selected = true;
		base.OnSelect(eventData);
	}
	public override void OnDeselect(BaseEventData eventData)
	{
		Selected = false;
		CancelHold();
		if (!interactable)
			return;
		base.OnDeselect(eventData);
	}


	void StartHoldIfSelected()
	{
		if (Selected && interactable && BuyingAllowed)
		{
			MouseMode = false;
			StartHold();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		controls = new();
		if (FillImage == null)
			FillImage = transform.Find("Fill").GetComponent<Image>();
	}
	public override void OnPointerDown(PointerEventData eventData)
	{
		if (!interactable || !BuyingAllowed)
			return;
		MouseMode = true;
		StartHold();
		base.OnPointerDown(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (!interactable || !BuyingAllowed)
			return;

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

		if (HeldLongEnough)
			OnHeldLongEnough();
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
		CompleteAction();
	}

	IEnumerator WaitForRelease()
	{
		while (true)
		{
			if (!IsSubmitStillPressed())
			{
				OnHeldLongEnough();
				yield break;
			}
			yield return null;
		}

	}
	void OnHeldLongEnough()
	{
		HeldLongEnough = false;
		isHolding = false;
		onHoldComplete?.Invoke();
		FillImage.fillAmount = 0;
	}

	void CompleteAction()
	{
		if (ReleaseToTrigger)
		{
			HeldLongEnough = true;
			if (!MouseMode)
				StartCoroutine(WaitForRelease());
		}
		else
		{
			OnHeldLongEnough();
			if (this.IsInteractable() && BuyingAllowed)
				StartHold();
		}
	}
	private bool IsSubmitStillPressed()
	{
		return MouseMode || controls.UI.Submit.IsPressed();
	}

}
