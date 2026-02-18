using UnityEngine;

public class POI_Pointer : MonoBehaviour
{
	public GameObject Player;
	public GameObject Target;
	public Vector3 TargetPosition;

    // Update is called once per frame
    void Update()
    {
        var startPos = Player.transform.position;
        var targetPos = Target?.transform.position ?? TargetPosition; 
        var direction = targetPos - startPos;
		float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float distanceFromShip = 300 + Mathf.Clamp(direction.magnitude / 10f, 0, 150);
        var pos = Quaternion.Euler(0, 0, targetAngle) * (Vector3.right * distanceFromShip);

		transform.localPosition = pos;
		transform.rotation = Quaternion.Euler(0f, 0f, targetAngle-90);
	}
}
