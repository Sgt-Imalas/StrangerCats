using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{

	public HoldButton UnlockButton;
	public TextMeshProUGUI Title, UpgradeDesc;
	public GameObject A, B, C, D;
	BuyableUpgrade CurrentUpgrade = null;
	TextMeshProUGUI CostA, CostB, CostC, CostD;
	bool woke = false;
	private void Awake()
	{
		if (A != null)
			CostA = A.transform.GetComponentInChildren<TextMeshProUGUI>();
		else
			Debug.LogWarning("Upgrade item " + name + " is missing cost display for resource A");
		if (B != null)
			CostB = B.transform.GetComponentInChildren<TextMeshProUGUI>();
		else
			Debug.LogWarning("Upgrade item " + name + " is missing cost display for resource B");
		if (C != null)
			CostC = C.transform.GetComponentInChildren<TextMeshProUGUI>();
		else
			Debug.LogWarning("Upgrade item " + name + " is missing cost display for resource C");
		if (D != null)
			CostD = D.transform.GetComponentInChildren<TextMeshProUGUI>();
		else
			Debug.LogWarning("Upgrade item " + name + " is missing cost display for resource D");
		UnlockButton.onHoldComplete.AddListener(BuyingUpgrade);
		woke = true;
	}

	public System.Action<Selectable> OnSelect;
	public bool Interactable
	{
		get => UnlockButton.interactable;
		set => UnlockButton.interactable = value;
	}
	public void Select() => UnlockButton.Select();
	void BuyingUpgrade()
	{

	}
	private void OnEnable()
	{
		RefreshCanBuy(default, 0);
		Global.Instance.SpaceshipResources.OnResourceCollected += RefreshCanBuy;
	}
	private void OnDisable()
	{
		Global.Instance.SpaceshipResources.OnResourceCollected -= RefreshCanBuy;
	}


	void RefreshCanBuy(ResourceType _, int i)
	{
		if (CurrentUpgrade == null)
		{
			UnlockButton.interactable = false;
			return;
		}

		foreach (var cost in CurrentUpgrade.GetCurrentScaledCosts())
		{
			if (!Global.Instance.SpaceshipResources.CanAfford(cost.Key, cost.Value))
			{
				UnlockButton.interactable = false;
				return;
			}
		}
		UnlockButton.interactable = true;
	}

	void SetUpgradeCosts()
	{
		if (!woke)
			return;

		if(CurrentUpgrade == null)
		{
			A.SetActive(false);
			B.SetActive(false);
			C.SetActive(false);
			D.SetActive(false);
			return;
		}
		Dictionary<ResourceType, int> dict = new();
		foreach (var costEntry in CurrentUpgrade.GetCurrentScaledCosts())
			dict[costEntry.Key] = costEntry.Value;

		if (dict.TryGetValue(ResourceType.A, out var cost) && cost > 0)
		{
			A.SetActive(true);
			CostA.text = cost.ToString();
		}
		else
			A.SetActive(false);

		if (dict.TryGetValue(ResourceType.B, out cost) && cost > 0)
		{
			B.SetActive(true);
			CostB.text = cost.ToString();
		}
		else
			B.SetActive(false);

		if (dict.TryGetValue(ResourceType.C, out cost) && cost > 0)
		{
			C.SetActive(true);
			CostC.text = cost.ToString();
		}
		else
			C.SetActive(false);

		if (dict.TryGetValue(ResourceType.D, out cost) && cost > 0)
		{
			D.SetActive(true);
			CostD.text = cost.ToString();
		}
		else
			D.SetActive(false);
	}
	public void SetUpgrade(BuyableUpgrade upgrade)
	{
		CurrentUpgrade = upgrade;
		Title.text = upgrade?.Name;

		UpgradeDesc.SetText(upgrade?.GetUpgradeText());
		SetUpgradeCosts();
		Debug.Log("Set upgrade: " + upgrade?.Name);
	}
	public void Refresh() => SetUpgrade(CurrentUpgrade);
}
