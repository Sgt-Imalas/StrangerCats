using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class ImpactShakeable : MonoBehaviour
	{
		public float Power = 1.0f;
		public float Dureation = 1.0f;
		public float Frequency = 4.0f;

		private Vector3 currentOffset = Vector3.zero;
		private Coroutine coroutine;

		private void Start()
		{
			GlobalEvents.Instance.OnPlayerHurt += OnPlayerHurt;
		}

		private void OnPlayerHurt(float damage, bool lethal, Vector3 from)
		{
			Shake(from, Power, Dureation);
		}

		private void OnDestroy()
		{
			GlobalEvents.Instance.OnPlayerHurt -= OnPlayerHurt;
		}

		public void Shake(Vector3 hitDirection, float strength = 0.4f, float duration = 0.2f)
		{

			currentOffset = Vector3.zero;

			if (coroutine != null)
				StopCoroutine(coroutine);

			coroutine = StartCoroutine(ShakeCoroutine(hitDirection.normalized, strength, duration));
		}

		private IEnumerator ShakeCoroutine(Vector3 direction, float power, float duration)
		{
			var elapsed = 0f;

			while (elapsed < duration)
			{
				var t = elapsed / duration;

				var damper = 1f - t;
				damper *= damper;

				var wobble = Mathf.Sin(t * Mathf.PI * Frequency);

				currentOffset = damper * wobble * power * direction;

				elapsed += Time.unscaledDeltaTime; // do not be affected by the time slowdown of the damage

				yield return null;
			}

			currentOffset = Vector3.zero;
		}

		private void LateUpdate()
		{
			transform.localPosition += currentOffset;
		}
	}

}
