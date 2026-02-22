using UnityEngine;

//[RequireComponent(typeof(Collider2D))]
public class FlyTowardsPlayer : MonoBehaviour
{
	//public Collider2D currentPlayer = null;
	public GameObject currentPlayer = null;

	[SerializeField] Rigidbody2D rb;

	public float CollectionSpeed = 5f;
	public bool Accellerates = true;


	private void Awake()
	{
		if (rb == null)
			rb = transform.GetComponent<Rigidbody2D>();
	}


	float incrementalMultiplier = 1f;
	private void FixedUpdate()
	{
		if (currentPlayer != null)
		{
			rb.AddForce(CollectionSpeed * incrementalMultiplier * Time.fixedDeltaTime * (currentPlayer.transform.position - rb.transform.position));
			if (Accellerates)
				incrementalMultiplier += Time.fixedDeltaTime;
		}
	}
}
