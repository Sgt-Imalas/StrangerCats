using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDetection : MonoBehaviour
{
	public Collider2D currentPlayer = null;

	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player"))
			return;

		currentPlayer = null;
	}

	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player"))
			return;

		currentPlayer = collision;
	}
}
