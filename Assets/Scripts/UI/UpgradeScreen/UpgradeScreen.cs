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

		RotationSpeed = GO_RotationSpeed.GetComponent<UpgradeItem>();
		RotationSpeed.SetUpgrade(Global.Instance.Upgrades.RotationSpeed);

		SuperCruise = GO_SuperCruise.GetComponent<UpgradeItem>();
		SuperCruise.SetUpgrade(Global.Instance.Upgrades.SuperCruise);

		LifeSupport = GO_LifeSupport.GetComponent<UpgradeItem>();
		LifeSupport.SetUpgrade(Global.Instance.Upgrades.LifeSupport);

		LaserDamage = GO_LaserDamage.GetComponent<UpgradeItem>();
		LaserDamage.SetUpgrade(Global.Instance.Upgrades.LaserDamage);

		LaserSpeed = GO_LaserSpeed.GetComponent<UpgradeItem>();
		LaserSpeed.SetUpgrade(Global.Instance.Upgrades.LaserSpeed);

		LaserRange = GO_LaserRange.GetComponent<UpgradeItem>();
		LaserRange.SetUpgrade(Global.Instance.Upgrades.LaserRange);

		PodSpeed = GO_ManeuveringThrusters.GetComponent<UpgradeItem>();
		PodSpeed.SetUpgrade(Global.Instance.Upgrades.PodSpeed);

		ResourceYield = GO_ResourceYield.GetComponent<UpgradeItem>();
		ResourceYield.SetUpgrade(Global.Instance.Upgrades.ResourceYield);

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

	private void OnEnable()
	{
		foreach(var item in Items)
		{
			item.Refresh();
		}
		foreach(var item in Items)
		{
			if (item.Interactable)
			{
				item.Select();
				break;
			}
		}
		Global.Instance.InUpgradeMenu = true;
		Time.timeScale = 0;
		Global.Instance.OnUpgradePurchased += OnItemBought;
	}
	void OnItemBought()
	{
		if (BuySound != null)
			MusicManager.PlayFx(BuySound);
	}

	private void OnDisable()
	{
		Global.Instance.InUpgradeMenu = false;
		Time.timeScale = 1;
		Global.Instance.OnUpgradePurchased -= OnItemBought;
	}
}
