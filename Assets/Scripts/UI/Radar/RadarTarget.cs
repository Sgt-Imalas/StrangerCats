using Unity.VisualScripting;
using UnityEngine;

public class RadarTarget : MonoBehaviour
{
	public Color Tint = Color.red;
	private void Start()
	{
		RadarController.AddPointer(gameObject, Tint);
	}
	private void OnDisable()
	{
		RadarController.RemovePointer(gameObject);
	}
}
