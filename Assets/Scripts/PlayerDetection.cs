using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
	public Collider2D currentPlayer = null;

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player"))
			return;

		currentPlayer = null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player"))
			return;

		currentPlayer = collision;
	}
}
