using UnityEngine;
using UnityEngine.InputSystem;

public class Collectible : MonoBehaviour
{
	bool turnedOff = false;
	public void TurnOff() => turnedOff = true;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player") || !this.isActiveAndEnabled || turnedOff)
			return;

		DoCollect();
	}

	protected virtual void DoCollect()
	{
		Destroy(gameObject);
	}
}
