using UnityEngine;

public class PlanetRandomPosition : MonoBehaviour
{
	public int MedDistance = 10, Variance = 5;

	public int MinAngle = 0, MaxAngle = 359;

	void Start()
	{
		transform.position = Global.Instance.GetPlanetPosition(gameObject.name, MedDistance, Variance, MinAngle, MaxAngle);
	}
}
