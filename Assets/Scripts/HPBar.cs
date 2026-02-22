using UnityEngine;

public class HPBar : MonoBehaviour
{
	public Transform fill;

	public void SetPercent(float percent)
	{
		if (fill == null)
			return;

		percent = Mathf.Clamp01(percent);

		fill.transform.localScale = new Vector3(percent, 1.0f, 1.0f);
	}
}
