using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
	public AudioClip BuySound;
	//starship
	UpgradeItem
		Radar, //unlockable, reveals pois
		RotationSpeed, //regular movespeed, rotationspeed
		SuperCruise //unlockable, upgrades increase speed
		;
	//lander
	UpgradeItem		
		LifeSupport, //energy, duration of landing
		LaserDamage, //mob damage, tile damage
		LaserSpeed, //laser fire rate
		LaserRange, //range and dig radius
		PodSpeed, //movementspeed
		ResourceYield //tile and enemy yield
		;
	//starship
	public GameObject
		GO_Radar, //unlockable, reveals pois
		GO_RotationSpeed, //regular movespeed, rotationspeed
		GO_SuperCruise //unlockable, upgrades increase speed
		;
	//lander
	public GameObject
		GO_LifeSupport, //energy, duration of landing
		GO_LaserDamage, //mob damage, tile damage
		GO_LaserSpeed, //laser fire rate
		GO_LaserRange, //range and dig radius
		GO_ManeuveringThrusters, //movementspeed
		GO_ResourceYield //tile and enemy yield
		;

	List<UpgradeItem> Items;

	private void Awake()
	{
		Radar = GO_Radar.GetComponent<UpgradeItem>();
		Radar.SetUpgrade(Global.Instance.Upgrades.RadarRange);
		//Radar.OnInteractableChanged += OnInteractableChanged;

		RotationSpeed = GO_RotationSpeed.GetComponent<UpgradeItem>();
		RotationSpeed.SetUpgrade(Global.Instance.Upgrades.RotationSpeed);
		//RotationSpeed.OnInteractableChanged += OnInteractableChanged;

		SuperCruise = GO_SuperCruise.GetComponent<UpgradeItem>();
		SuperCruise.SetUpgrade(Global.Instance.Upgrades.SuperCruise);
		//SuperCruise.OnInteractableChanged += OnInteractableChanged;

		LifeSupport = GO_LifeSupport.GetComponent<UpgradeItem>();
		LifeSupport.SetUpgrade(Global.Instance.Upgrades.LifeSupport);
		//LifeSupport.OnInteractableChanged += OnInteractableChanged;

		LaserDamage = GO_LaserDamage.GetComponent<UpgradeItem>();
		LaserDamage.SetUpgrade(Global.Instance.Upgrades.LaserDamage);
		//LaserDamage.OnInteractableChanged += OnInteractableChanged;

		LaserSpeed = GO_LaserSpeed.GetComponent<UpgradeItem>();
		LaserSpeed.SetUpgrade(Global.Instance.Upgrades.LaserSpeed);
		//LaserSpeed.OnInteractableChanged += OnInteractableChanged;

		LaserRange = GO_LaserRange.GetComponent<UpgradeItem>();
		LaserRange.SetUpgrade(Global.Instance.Upgrades.LaserRange);
		//LaserRange.OnInteractableChanged += OnInteractableChanged;

		PodSpeed = GO_ManeuveringThrusters.GetComponent<UpgradeItem>();
		PodSpeed.SetUpgrade(Global.Instance.Upgrades.PodSpeed);
		//PodSpeed.OnInteractableChanged += OnInteractableChanged;

		ResourceYield = GO_ResourceYield.GetComponent<UpgradeItem>();
		ResourceYield.SetUpgrade(Global.Instance.Upgrades.ResourceYield);
		//ResourceYield.OnInteractableChanged += OnInteractableChanged;

		Items = new List<UpgradeItem>
		{
			Radar,
			RotationSpeed,
			SuperCruise,
			LifeSupport,
			LaserDamage,
			LaserSpeed,
			LaserRange,
			PodSpeed,
			ResourceYield
		};

	}
	void OnInteractableChanged(bool cardIsInteractable)
	{
		if (cardIsInteractable)
			return;

		SelectFirstInteractable();
	}
	void SelectFirstInteractable()
	{
		foreach (var item in Items)
		{
			if (item.BuyingAllowed)
			{
				item.Select();
				break;
			}
		}
	}


	private void OnEnable()
	{
		foreach(var item in Items)
		{
			item.Refresh();
		}
		SelectFirstInteractable();
		Global.Instance.InUpgradeMenu = true;
		Time.timeScale = 0;
		Global.Instance.OnUpgradePurchased += OnItemBought;
	}
	void OnItemBought()
	{
		if (BuySound != null)
			MusicManager.PlayFx(BuySound);

		foreach (var item in Items)
			item.Refresh();
	}

	private void OnDisable()
	{
		Global.Instance.InUpgradeMenu = false;
		Time.timeScale = 1;
		Global.Instance.OnUpgradePurchased -= OnItemBought;
	}
}
