using Unity.VisualScripting;
using UnityEngine;

public class RadarTarget : MonoBehaviour
{
	public Color Tint = Color.red;
	public bool IgnoreDistanceLimit = false;
	private void Start()
	{
		RadarController.AddPointer(gameObject, Tint, IgnoreDistanceLimit);
	}
	private void OnDisable()
	{
		RadarController.RemovePointer(gameObject);
	}
}
