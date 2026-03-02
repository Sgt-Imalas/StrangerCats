using Assets.Scripts;
using UnityEngine;

public class HurtablePlayer : MonoBehaviour
{
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collision.gameObject.CompareTag("Enemy"))
			return;

		if (collision.gameObject.TryGetComponent<ContactDamageInformation>(out var information))
			PersistentPlayer.Instance.DamageEnergy(information, collision.contacts[0].normal);
		else
			Debug.LogWarning("Hurtable Player collided with something it shouldn't have: " + collision.collider.name);
	}
}
