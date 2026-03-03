using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
	public float lifetime = 20.0f;
	private float elapsed = 0;

	public AudioSource audioSource;
	public Rigidbody rb;

	public AudioClip[] sounds;

	public Action<Ball> onBreak;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		elapsed += Time.deltaTime;

		if (elapsed > lifetime)
		{
			if (onBreak != null)
				onBreak.Invoke(this);
			else
				Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		audioSource.PlayRandom(sounds);
	}
}
