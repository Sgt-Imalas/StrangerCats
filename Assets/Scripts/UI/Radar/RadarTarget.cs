using Unity.VisualScripting;
using UnityEngine;

public class RadarTarget : MonoBehaviour
{
	public Color Tint = Color.red;
	public bool IgnoreDistanceLimit = false;
	public float CutoffDistanceThreshold = 50;

	public void SetIgnoreDistanceLimit(bool value)
	{
		IgnoreDistanceLimit = value;
		RadarController.RemovePointer(gameObject);
		RadarController.AddPointer(gameObject, Tint, IgnoreDistanceLimit);

	}
	private void Start()
	{
		RadarController.AddPointer(gameObject, Tint, IgnoreDistanceLimit);
	}
	private void OnDisable()
	{
		RadarController.RemovePointer(gameObject);
	}
}
