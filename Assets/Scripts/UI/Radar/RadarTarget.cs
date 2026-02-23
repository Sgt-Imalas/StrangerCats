using Unity.VisualScripting;
using UnityEngine;

public class RadarTarget : MonoBehaviour
{
	public Color Tint = Color.red;
	public bool IgnoreDistanceLimit = false;
	public float CutoffDistanceThreshold = 10;

	public void SetIgnoreDistanceLimit(bool value)
	{
		IgnoreDistanceLimit = value;
		RadarController.RemovePointer(this);
		RadarController.AddPointer(this, Tint, IgnoreDistanceLimit);

	}
	private void Start()
	{
		RadarController.AddPointer(this, Tint, IgnoreDistanceLimit);
	}
	private void OnDisable()
	{
		RadarController.RemovePointer(this);
	}
}
