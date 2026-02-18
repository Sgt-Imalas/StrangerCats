using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float lifeTime = 1.0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

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
