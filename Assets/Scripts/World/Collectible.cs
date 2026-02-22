using UnityEngine;
using UnityEngine.InputSystem;

public class Collectible : MonoBehaviour
{
	[SerializeField]GameObject Target;
	bool turnedOff = false;
	private void Awake()
	{
		if(Target == null)
			Target = transform.parent.gameObject;
	}
	public void TurnOff() => turnedOff = true;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player" || !this.isActiveAndEnabled || turnedOff)
			return;

		DoCollect();
	}

	protected virtual void DoCollect()
	{
		Destroy(Target);
	}
}
