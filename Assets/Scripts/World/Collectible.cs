using UnityEngine;

public class Collectible : MonoBehaviour
{
	private bool turnedOff = false;

	public void TurnOff() => turnedOff = true;

	public void TurnOn() => turnedOff = false;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.gameObject.CompareTag("Player") || !isActiveAndEnabled || turnedOff)
			return;

		DoCollect();
	}

	protected virtual void DoCollect()
	{
	}

}
