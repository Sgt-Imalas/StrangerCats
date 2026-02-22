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

		PersistentPlayer.Instance.DamageEnergy(10.0f);
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
