using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpeedMeter : MonoBehaviour
{
	public Image FillImage;
	public TextMeshProUGUI Text;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (Text == null)
			Text = GetComponentInChildren<TextMeshProUGUI>();

	}

	// Update is called once per frame
	void Update()
	{
		float velocity = Global.Instance.Spaceship.CurrentVelocity, maxVelocity = 0;
		if (SceneManager.GetActiveScene().name == "MineableTerrain")
		{
			maxVelocity = Global.Instance.Spaceship.PodMode.MaxVelocity + PersistentPlayer.GetAttribute(AttributeType.PodSpeed,0f);
		}
		else
		{
			maxVelocity = Global.Instance.Spaceship.CurrentMode.MaxVelocity;
			
			if(!Global.Instance.Spaceship.InPrecisionFlightMode) 
				maxVelocity *= PersistentPlayer.GetAttribute(AttributeType.SpaceShipSuperCruiseSpeed, 1f);
		}

		FillImage.fillAmount = velocity / maxVelocity;
		Text.SetText(Mathf.RoundToInt(velocity).ToString());
	}
}
