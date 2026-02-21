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
	public GameObject Radar, SuperCruise, MeatWorldItem, TennisWorldItem, DesertWorldItem;

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
		Global.Instance.Upgrades.OnItemCollected += RefreshVisibility;
	}

	private void OnToggleUpgradeScreen(InputAction.CallbackContext _)
	{
		if (!UpgradesInteractable || !UpgradeButton.activeSelf)
			return;

		if(UpgradeScreen == null)
			return;

		if (!Global.Instance.SpaceshipResources.AnyResourceDiscovered())
			return;

		UpgradeScreen.SetActive(!UpgradeScreen.activeSelf);
	}

	private void OnDestroy()
	{
		Global.Instance.SpaceshipResources.OnResourceDiscovered -= RefreshVisibility;
		Global.Instance.SpaceshipResources.OnResourceCollected -= OnResourceCollected;
		Global.Instance.SpaceshipResources.OnResourceSpent -= OnResourceSpent;
		Global.Instance.Upgrades.OnItemCollected -= RefreshVisibility;
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
			case ResourceType.Meat:
				ResourceA.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.Meat));
				break;
			case ResourceType.Rust:
				ResourceB.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.Rust));
				break;
			case ResourceType.Ball:
				ResourceC.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.Ball));
				break;
			case ResourceType.Dust:
				ResourceD.SetActive(Global.Instance.SpaceshipResources.ResourceDiscovered(ResourceType.Dust));
				break;
		}
		RefreshDisplayAmount(type);
		RefreshUpgradeButton();
	}
	private void RefreshVisibility(FindableItem type)
	{
		switch (type)
		{
			case FindableItem.Radar:
				Radar.SetActive(Global.Instance.Upgrades.RadarUnlocked);
				break;
			case FindableItem.SuperCruise:
				SuperCruise.SetActive(Global.Instance.Upgrades.SuperCruiseUnlocked);
				break;
			case FindableItem.Meat:
				MeatWorldItem.SetActive(Global.Instance.Upgrades.MeatWorldItemFound);
				break;
			case FindableItem.Tennis:
				TennisWorldItem.SetActive(Global.Instance.Upgrades.TennisWorldItemFound);
				break;
			case FindableItem.Desert:
				DesertWorldItem.SetActive(Global.Instance.Upgrades.DesertWorldItemFound);
				break;
		}
	}

	private void RefreshDisplayAmount(ResourceType type)
	{
		switch (type)
		{
			case ResourceType.Meat:
				ResourceA_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.Meat).ToString();
				break;
			case ResourceType.Rust:
				ResourceB_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.Rust).ToString();
				break;
			case ResourceType.Ball:
				ResourceC_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.Ball).ToString();
				break;
			case ResourceType.Dust:
				ResourceD_Amount.text = Global.Instance.SpaceshipResources.GetResourceAmount(ResourceType.Dust).ToString();
				break;
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		UpgradeButton.SetActive(UpgradesInteractable && UpgradeScreen != null);
		RefreshVisibility(ResourceType.Meat);
		RefreshVisibility(ResourceType.Rust);
		RefreshVisibility(ResourceType.Ball);
		RefreshVisibility(ResourceType.Dust);
		RefreshVisibility(FindableItem.Radar);
		RefreshVisibility(FindableItem.SuperCruise);
		RefreshVisibility(FindableItem.Meat);
		RefreshVisibility(FindableItem.Tennis);
		RefreshVisibility(FindableItem.Desert);

	}

	IEnumerator AnimateCollection(ResourceType type, uint amount, float duration = 0.5f)
	{
		yield return new WaitForSeconds(0.1f); // Wait to ensure the resource counter has repositioned if it was just discovered
		var startPos = Canvas.position; 
		startPos.z = 0;
		Vector3 endPos = transform.position;
		switch (type)
		{
			case ResourceType.Meat:
				endPos = ResourceA.transform.position;
				break;
			case ResourceType.Rust:
				endPos = ResourceB.transform.position;
				break;
			case ResourceType.Ball:
				endPos = ResourceC.transform.position;
				break;
			case ResourceType.Dust:
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
