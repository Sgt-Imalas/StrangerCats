using UnityEngine;

public class RadarPointer : MonoBehaviour
{
	public GameObject Player;
	public GameObject Target;
	/// <summary>
	/// this can be used to point to a specific position in space instead of a GameObject
	/// </summary>
	public Vector3 TargetPosition;

    // Update is called once per frame
    void Update()
    {
        var startPos = Player.transform.position;
        var targetPos = Target?.transform.position ?? TargetPosition; 
        var direction = targetPos - startPos;
		float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		//farther away from the ship position to visualize distance;
        float distanceFromShip = 300 + Mathf.Clamp(direction.magnitude / 10f, 0, 150);
        var pos = Quaternion.Euler(0, 0, targetAngle) * (Vector3.right * distanceFromShip);

		transform.localPosition = pos;
		transform.rotation = Quaternion.Euler(0f, 0f, targetAngle-90);
	}
}
