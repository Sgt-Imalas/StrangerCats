using Assets.Scripts;
using UnityEngine;

public class HurtablePlayer : MonoBehaviour
{

	void Start()
	{

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collision.gameObject.CompareTag("Enemy"))
			return;

		if(collision.gameObject.TryGetComponent<ContactDamageInformation>(out var information))
		{
			PersistentPlayer.Instance.DamageEnergyPercentage(information.ContactDamagePercentage);
			PersistentPlayer.Instance.DamageEnergy(information.ContactDamageFlat);
		}
		else
			PersistentPlayer.Instance.DamageEnergyPercentage(0.1f);
		// ????????????????

		/*		var idx = Random.Range(0, sounds.Length - 1);
				audioSource.clip = sounds[idx];
				audioSource.pitch = Random.Range(0.9f, 1.1f);
				audioSource.Play();*/
	}

	void Update()
	{

	}
}
