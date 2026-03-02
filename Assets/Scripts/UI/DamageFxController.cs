using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class DamageFxController : MonoBehaviour
{
	public CanvasGroup RedOut;
	public Camera Camera;
	[Tooltip("This should start and end on 0.")]
	public AnimationCurve Curve;
	public float Duration = 0.8f;
	public float TimeScaleMultiplier = 0.3f;

	public AudioSource audioSource;
	public AudioClip[] hitSounds;

	private float elapsed;

	void Start()
	{
		GlobalEvents.Instance.OnPlayerHurt += OnPlayerHurt;
	}

	private void OnDestroy()
	{
		GlobalEvents.Instance.OnPlayerHurt -= OnPlayerHurt;
	}

	private void OnPlayerHurt(float damage, bool lethal, Vector3 _)
	{
		StartCoroutine(FadeEffects());
	}

	private IEnumerator FadeEffects()
	{
		var timeScale = Time.timeScale;

		Time.timeScale *= TimeScaleMultiplier;

		var durationScaled = Duration; // * (1.0f / TimeScaleMultiplier);
		elapsed = 0.0f;
		audioSource.PlayRandom(hitSounds);

		while (elapsed < durationScaled)
		{
			elapsed += Time.unscaledDeltaTime;

			var t = Curve.Evaluate(elapsed / durationScaled);
			t = Mathf.Clamp01(t);

			RedOut.alpha = t;

			Time.timeScale = 1.0f - (t * timeScale);

			yield return new WaitForEndOfFrame();
		}

		//Time.timeScale *= 1.0f / TimeScaleMultiplier;
		Time.timeScale = timeScale;

		Debug.Log(elapsed);

		yield return null;
	}
}
