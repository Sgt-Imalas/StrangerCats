using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedMeter : MonoBehaviour
{
    public Image FillImage;
    public TextMeshProUGUI Text;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        if(Text == null)
			Text = GetComponentInChildren<TextMeshProUGUI>();

	}

    // Update is called once per frame
    void Update()
    {
        float velocity = Global.Instance.Spaceship.CurrentVelocity;
		FillImage.fillAmount = velocity / Global.Instance.Spaceship.CurrentMode.MaxVelocity;
        Text.SetText(Mathf.RoundToInt(velocity).ToString());
	}
}
