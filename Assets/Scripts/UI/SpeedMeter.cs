using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpeedMeter : MonoBehaviour
{
	[Header("Graphics")]
	public Image FillImage;
	public TextMeshProUGUI Text;

	[Header("Spring")]
	public float stiffness = 10f;
	public float damping = 1f;
	public float velocityInfluence = 1f;
	public float rotStiffness = 10f;
	public float rotDamping = 0.5f;

	private float _angle;
	private float _angularVelocity;

	void Start()
	{
		if (Text == null)
			Text = GetComponentInChildren<TextMeshProUGUI>();
	}

	void Update()
	{
		float velocity = Global.Instance.Spaceship.CurrentVelocity, maxVelocity = 0;

		UpdateSpring();

		if (SceneManager.GetActiveScene().name == "MineableTerrain")
		{
			maxVelocity = Global.Instance.Spaceship.PodMode.MaxVelocity + PersistentPlayer.GetAttribute(AttributeType.PodSpeed, 0f);
		}
		else
		{
			maxVelocity = Global.Instance.Spaceship.CurrentMode.MaxVelocity;

			if (!Global.Instance.Spaceship.InPrecisionFlightMode)
				maxVelocity *= PersistentPlayer.GetAttribute(AttributeType.SpaceShipSuperCruiseSpeed, 1f);
		}

		FillImage.fillAmount = velocity / maxVelocity;
		Text.SetText(Mathf.RoundToInt(velocity).ToString());
	}

	private Vector2 position;
	private Vector2 velocity;

	private void UpdateSpring()
	{
		var shipVelocity = Global.Instance.Spaceship.CurrentVelocityVectorClamped;

		var dt = Time.deltaTime;

		var force = -stiffness * position;
		force += -damping * velocity;
		force += -shipVelocity;

		var acceleration = force;

		velocity += acceleration * dt;
		position += velocity * dt;

		var angularAcceleration = -rotStiffness * _angle;
		angularAcceleration += -rotDamping * _angularVelocity;
		angularAcceleration += shipVelocity.x * 5f;

		_angularVelocity += angularAcceleration * dt;
		_angle += _angularVelocity * dt;

		transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0, 0, _angle));
	}
}
