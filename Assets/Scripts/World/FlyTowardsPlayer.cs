using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class FlyTowardsPlayer : MonoBehaviour
{
	public Collider2D currentPlayer = null;

	[SerializeField]GameObject Target;
	[SerializeField]Rigidbody2D rb;

	public float CollectionSpeed = 5f;
	public bool LosesTargeting = false;
	public bool Accellerates = true;


	private void Awake()
	{
		if (Target == null)
			Target = transform.parent.gameObject;
		if(rb == null)
			rb = transform.parent.GetComponent<Rigidbody2D>();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player" || !LosesTargeting)
			return;
		currentPlayer = null;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player")
			return;
		currentPlayer = collision;
	}

	float incrementalMultiplier = 1f;
	private void FixedUpdate()
	{
		if(currentPlayer != null)
		{
			rb.AddForce((currentPlayer.transform.position - rb.transform.position) * Time.fixedDeltaTime * CollectionSpeed * incrementalMultiplier);
			if (Accellerates)
				incrementalMultiplier += Time.fixedDeltaTime;
		}
	}
}
