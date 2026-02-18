using Unity.VisualScripting;
using UnityEngine;

public class RadarTarget : MonoBehaviour
{
	private void Start()
	{
		RadarController.AddPointer(gameObject);
	}
	private void OnDisable()
	{
		RadarController.RemovePointer(gameObject);
	}
}
