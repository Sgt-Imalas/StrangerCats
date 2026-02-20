using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
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
		ManeuveringThrusters, //movementspeed
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
		LaserDamage = GO_LaserDamage.GetComponent<UpgradeItem>();
		LaserSpeed = GO_LaserSpeed.GetComponent<UpgradeItem>();
		LaserRange = GO_LaserRange.GetComponent<UpgradeItem>();
		ManeuveringThrusters = GO_ManeuveringThrusters.GetComponent<UpgradeItem>();
		ResourceYield = GO_ResourceYield.GetComponent<UpgradeItem>();

		Items.Add(Radar);
		Items.Add(RotationSpeed);
		Items.Add(SuperCruise);
		Items.Add(LifeSupport);
		Items.Add(LaserDamage);
		Items.Add(LaserSpeed);
		Items.Add(LaserRange);
		Items.Add(ManeuveringThrusters);
		Items.Add(ResourceYield);

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
	}
	private void OnDisable()
	{
	}
}
