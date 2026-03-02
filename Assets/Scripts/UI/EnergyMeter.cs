using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class EnergyMeter : MonoBehaviour
{
	[SerializeField] Image FillMeter;
	[SerializeField] TextMeshProUGUI Text;
	[SerializeField] RectTransform ActualFill;
	[SerializeField] RectTransform DepleteFill;
	[SerializeField] float catchUpSpeed;
	[SerializeField] float minWidth, maxWidth;
	[SerializeField]
	Color
		defaultColor = new Color32(0, 217, 214, 255),
		warningColor = new Color32(255, 65, 77, 255);

	public float TargetFillAmount = 1f, CurrentFillAmount = 1f;


	void Update()
	{
		if (Application.isPlaying)
		{
			TargetFillAmount = PersistentPlayer.Instance.EnergyPercentage;
		}

		if (FillMeter == null)
			return;

		if (TargetFillAmount > CurrentFillAmount)
			CurrentFillAmount = TargetFillAmount;
		else
			CurrentFillAmount = Mathf.MoveTowards(CurrentFillAmount, TargetFillAmount, Time.deltaTime * catchUpSpeed);

		var fillRange = maxWidth - minWidth;
		var totalFill = fillRange * TargetFillAmount;
		var tempFill = fillRange * CurrentFillAmount;

		FillMeter.color = TargetFillAmount < 0.20f ? warningColor : defaultColor;

		ActualFill.sizeDelta = new Vector2(Mathf.Round(totalFill + minWidth), 51);
		DepleteFill.sizeDelta = new Vector2(Mathf.Round(tempFill + minWidth), 51);

		Text.SetText(string.Format("{0:P0}", TargetFillAmount));
	}
}
