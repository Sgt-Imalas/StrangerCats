using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyMeter : MonoBehaviour
{
    [SerializeField]Image FillMeter;
    [SerializeField]TextMeshProUGUI Text;

    Color defaultColor = new Color32(0, 217, 214, 255);
	// Update is called once per frame
	void Update()
    {
        var fillAmount = PersistentPlayer.Instance.EnergyPercentage;
		FillMeter.fillAmount = fillAmount;
        FillMeter.color = fillAmount < 0.20f ? Color.red : defaultColor;
        Text.SetText(string.Format("{0:P0}", fillAmount));
	}
}
