using UnityEngine;
using UnityEngine.Pool;

public class ArtifactTennis : MonoBehaviour
{
	public float ballInterval = 0.5f;
	public float rotationSpeed = 2.0f;
	public float downTime = 1.0f;

	public BallLauncher[] launchers;

	public int _currentLauncher = 0;
	public float _elapsed;
	public bool _isRotating;
	public float targetRotation = 0;
	public float _currentAngle;

	public bool doSequence = true;

	public ObjectPool<Ball> balls;
	[SerializeField] private Ball _ballPrefab;

	void Start()
	{
		balls = new ObjectPool<Ball>(
			CreateItem,
			OnGet,
			OnRelease,
			OnDestroyItem,
			maxSize: 512
			);

		foreach (var launcher in launchers)
			launcher.SetPool(balls);
	}

	private Ball CreateItem()
	{
		var item = Instantiate(_ballPrefab);
		item.gameObject.SetActive(true);

		return item;
	}

	private void OnGet(Ball item) => item.gameObject.SetActive(true);

	private void OnRelease(Ball item) => item.gameObject.SetActive(false);

	private void OnDestroyItem(Ball item) => Destroy(item.gameObject);

	void Update()
	{
		_elapsed += Time.deltaTime;

		if (!_isRotating)
		{
			if (_elapsed > ballInterval)
			{
				if (_currentLauncher == launchers.Length)
				{
					if (doSequence)
					{
						_isRotating = true;
						targetRotation += 45.0f;
					}

					_currentLauncher = 0;

				}
				else
				{
					launchers[_currentLauncher++].TriggerManually();
				}


				_elapsed = 0;
			}
		}
		else
		{
			if (_elapsed > ballInterval)
			{
				_currentAngle = Mathf.MoveTowards(_currentAngle, targetRotation, rotationSpeed);
				transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);

				if (_elapsed > downTime)
				{
					_isRotating = false;
					_elapsed = 0.0f;
				}
			}
		}
	}
}
