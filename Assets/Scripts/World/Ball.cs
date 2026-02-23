using UnityEngine;

public class Ball : MonoBehaviour
{
	public float lifetime = 20.0f;
	private float elapsed = 0;

	public AudioSource audioSource;

	public AudioClip[] sounds;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();

	}
	void Update()
	{
		elapsed += Time.deltaTime;
		if (elapsed > lifetime)
		{
			Object.Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var idx = Random.Range(0, sounds.Length - 1);
		audioSource.clip = sounds[idx];
		audioSource.pitch = Random.Range(0.9f, 1.1f);
		audioSource.Play();
	}
}
