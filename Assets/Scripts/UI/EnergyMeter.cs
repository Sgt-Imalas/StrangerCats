using Assets.Scripts;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyMeter : MonoBehaviour
{
	[SerializeField] Image FillMeter;
	[SerializeField] TextMeshProUGUI Text;
	[SerializeField] RectTransform decal;
	[SerializeField] Vector2 decal0;
	[SerializeField] Vector2 decal1;

	Color defaultColor = new Color32(0, 217, 214, 255);
	// Update is called once per frame
	void Update()
	{
		var fillAmount = PersistentPlayer.Instance.EnergyPercentage;

		var fill = (double)FillMeter.mainTexture.width;
		var increment = 1.0d / fill;
		var snapFill = (float)(Math.Ceiling(fillAmount / increment) * increment);

		//decal.transform.position = 
		FillMeter.fillAmount = snapFill;
		FillMeter.color = fillAmount < 0.20f ? Color.red : defaultColor;

		decal.anchoredPosition = new Vector2(Mathf.RoundToInt(Mathf.Lerp(0, decal1.x, fillAmount)), decal0.y);

		Text.SetText(string.Format("{0:P0}", fillAmount));
	}
}
