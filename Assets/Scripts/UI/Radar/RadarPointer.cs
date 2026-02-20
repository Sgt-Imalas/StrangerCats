using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadarPointer : MonoBehaviour
{
	public GameObject Player;
	public GameObject Target;
	public Color? Tint;
	/// <summary>
	/// this can be used to point to a specific position in space instead of a GameObject
	/// </summary>
	public Vector3 TargetPosition;
	[SerializeField]
	float CurrentDistance = float.MaxValue;

	[SerializeField]
	GameObject ImageGO, DistanceTextGO;
	Image Image;
	TextMeshProUGUI DistanceText;
	//hide pointer below
	public float CutofffDistanceThreshold = 50;
	public bool IgnoreDistanceLimit = false;

	private void Start()
	{
		Image = ImageGO.GetComponent<Image>();
		DistanceText = DistanceTextGO.GetComponent<TextMeshProUGUI>();
		if (Player == null)
			this.gameObject.SetActive(false);
		if (Tint.HasValue && Image)
			Image.color = Tint.Value;
		if(Tint.HasValue && DistanceText)
			DistanceText.color = Tint.Value;
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		CurrentDistance = UpdatePointerPosAndRotation();
		ToggleVisBelowThreshold();
	}

	protected virtual void ToggleVisBelowThreshold()
	{
		bool distanceBigEnough = CurrentDistance >= CutofffDistanceThreshold;
		ImageGO.SetActive(distanceBigEnough);
		DistanceTextGO.SetActive(distanceBigEnough);
	}

	float UpdatePointerPosAndRotation()
	{
		var startPos = Player.transform.position;
		var targetPos = Target != null ? Target.transform.position : TargetPosition;
		var distanceVector = targetPos - startPos;
		float targetAngle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
		//farther away from the ship position to visualize distance;
		float distance = distanceVector.magnitude;
		float visualOffset = 300 + Mathf.Clamp(distance / 10f, 0, 150);
		var pos = Quaternion.Euler(0, 0, targetAngle) * (Vector3.right * visualOffset);

		transform.localPosition = pos;
		transform.rotation = Quaternion.Euler(0f, 0f, targetAngle - 90);

		DistanceText.SetText(distance.ToString("0"));
		return distance;
	}
}
