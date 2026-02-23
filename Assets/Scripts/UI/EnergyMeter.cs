using Assets.Scripts;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class EnergyMeter : MonoBehaviour
{
	[SerializeField] Image FillMeter;
	[SerializeField] TextMeshProUGUI Text;
	[SerializeField] RectTransform decal;
	[SerializeField] Vector2 decal0;
	[SerializeField] Vector2 decal1;

	Color defaultColor = new Color32(0, 217, 214, 255);
	Color warningColor = new Color32(255, 65, 77, 255);
	public float FillAmount = 1f;
	public float PixelOffset = 0f;
	// Update is called once per frame
	void Update()
	{

		if (Application.isPlaying)
			FillAmount = PersistentPlayer.Instance.EnergyPercentage;

		var fill = (double)FillMeter.mainTexture.width;
		var increment = 1.0d / fill;
		var snapFill = (float)(Math.Ceiling(FillAmount / increment) * increment);

		//decal.transform.position = 
		FillMeter.fillAmount = snapFill;
		FillMeter.color = FillAmount < 0.20f ? warningColor : defaultColor;

		decal.anchoredPosition = new Vector2(Mathf.RoundToInt(PixelOffset + Mathf.Lerp(0, decal1.x, snapFill)), decal0.y);

		Text.SetText(string.Format("{0:P0}", FillAmount));
	}
}
