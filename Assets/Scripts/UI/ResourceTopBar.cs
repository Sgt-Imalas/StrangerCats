using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResourceTopBar : MonoBehaviour
{
	TextMeshProUGUI ResourceA_Amount, ResourceB_Amount, ResourceC_Amount, ResourceD_Amount;
	public bool UpgradesInteractable = true;
	public GameObject UpgradeButton, ResourceA, ResourceB, ResourceC, ResourceD;
	public GameObject FlyingItemPrefab;
	Button UpgradeBtn;
	Transform Canvas;
	private PlayerControls controls;
	public GameObject UpgradeScreen;

	private void Awake()
	{
		controls = new();
		controls.Player.ToggleUpgradeScreen.performed += OnToggleUpgradeScreen;
		Canvas = GetComponentInParent<Canvas>().transform;
		UpgradeBtn = UpgradeButton.GetComponent<Button>();
		UpgradeBtn.onClick.AddListener(() => OnToggleUpgradeScreen(default));

		ResourceA_Amount = ResourceA.GetComponentInChildren<TextMeshProUGUI>();
		ResourceB_Amount = ResourceB.GetComponentInChildren<TextMeshProUGUI>();
		ResourceC_Amount = ResourceC.GetComponentInChildren<TextMeshProUGUI>();
		ResourceD_Amount = ResourceD.GetComponentInChildren<TextMeshProUGUI>();

		Global.Instance.SpaceshipResources.OnResourceDiscovered += RefreshVisibility;
		Global.Instance.SpaceshipResources.OnResourceCollected += OnResourceCollected;
		Global.Instance.SpaceshipResources.OnResourceSpent += OnResourceSpent;
	}

	private void OnToggleUpgradeScreen(InputAction.CallbackContext _)
	{
		if (!UpgradesInteractable || !UpgradeButton.activeSelf)
			return;

		if(UpgradeScreen == null)
			return;

		UpgradeScreen.SetActive(!UpgradeScreen.activeSelf);
	}

	private void OnDestroy()
	{
		Global.Instance.SpaceshipResources.OnResourceDiscovered -= RefreshVisibility;
		Global.Instance.SpaceshipResources.OnResourceCollected -= OnResourceCollected;
		Global.Instance.SpaceshipResources.OnResourceSpent -= OnResourceSpent;
	}
	private void OnEnable()
	{
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	void OnResourceCollected(ResourceType type, uint amount)
	{
		StartCoroutine(AnimateCollection(type, amount));
	}
	void OnResourceSpent(ResourceType type, uint amount)
	{
		RefreshDisplayAmount(type);
	}
	void RefreshUpgradeButton()
	{
		UpgradeBtn.interactable = Global.Instance.SpaceshipResources.AnyResourceDiscovered();
	}
	private void RefreshVisibility(ResourceType type)
	{
		switch (type)
		{
			case ResourceType.A:
				ResourceA.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.A));
				break;
			case ResourceType.B:
				ResourceB.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.B));
				break;
			case ResourceType.C:
				ResourceC.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.C));
				break;
			case ResourceType.D:
				ResourceD.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.D));
				break;
		}
		RefreshDisplayAmount(type);
		RefreshUpgradeButton();
	}

	private void RefreshDisplayAmount(ResourceType type)
	{
		switch (type)
		{
			case ResourceType.A:
				ResourceA_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.A).ToString();
				break;
			case ResourceType.B:
				ResourceB_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.B).ToString();
				break;
			case ResourceType.C:
				ResourceC_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.C).ToString();
				break;
			case ResourceType.D:
				ResourceD_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.D).ToString();
				break;
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		UpgradeButton.SetActive(UpgradesInteractable);
		RefreshVisibility(ResourceType.A);
		RefreshVisibility(ResourceType.B);
		RefreshVisibility(ResourceType.C);
		RefreshVisibility(ResourceType.D);
	}

	IEnumerator AnimateCollection(ResourceType type, uint amount, float duration = 0.5f)
	{
		yield return new WaitForSeconds(0.1f); // Wait to ensure the resource counter has repositioned if it was just discovered
		var startPos = Canvas.position; 
		startPos.z = 0;
		Vector3 endPos = transform.position;
		switch (type)
		{
			case ResourceType.A:
				endPos = ResourceA.transform.position;
				break;
			case ResourceType.B:
				endPos = ResourceB.transform.position;
				break;
			case ResourceType.C:
				endPos = ResourceC.transform.position;
				break;
			case ResourceType.D:
				endPos = ResourceD.transform.position;
				break;
		}
		var resourceGO = Instantiate(FlyingItemPrefab, transform.parent);
		resourceGO.SetActive(true);
		if (resourceGO.TryGetComponent<Image>(out var image))
			image.color = MiningResourceStorage.ResourceInfos[type];
		RectTransform rect = resourceGO.GetComponent<RectTransform>();
		var size = Mathf.Clamp(10 + amount * 5, 20, 50);
		rect.sizeDelta = new Vector2(size, size);

		float time = 0f;

		while (time < duration)
		{
			time += Time.deltaTime;
			float t = time / duration;

			// Optional easing (remove this line for perfectly linear motion)
			t = t * t;

			rect.position = Vector2.Lerp(startPos, endPos, t);
			yield return null;
		}
		Destroy(resourceGO);
		RefreshDisplayAmount(type);
	}

	public void CollectRandomResource()
	{
		var randomType = (ResourceType)UnityEngine.Random.Range(0, 4);
		Global.Instance.SpaceshipResources.CollectResource(randomType, 999);
	}
}
