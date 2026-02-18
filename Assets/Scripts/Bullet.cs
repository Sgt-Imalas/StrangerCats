using UnityEngine;

[RequireComponent(typeof(Attribute))]
public class Bullet : MonoBehaviour
{
	public Attributes attributes;
	public float lifeTime = 1.0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}


	void OnCollisionEnter2D(Collision2D collision)
	{
		Object.Destroy(gameObject);
	}

	// Update is called once per frame
	void Update()
	{
		lifeTime -= Time.deltaTime;

		if (lifeTime < 0)
		{
			// TODO: pool
			Object.Destroy(gameObject);
		}
	}
}
