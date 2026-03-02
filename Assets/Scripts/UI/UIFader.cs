using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class UIFader : MonoBehaviour
	{
		public AnimationCurve fadeCurve;
		public float fadeDuration;
		[SerializeField] private CanvasGroup _targetCanvasGroup;
		private float _elapsed;
		private float _fade, _targetFade;

		public TransitionFinishedEvent onFadeFinished;

		private void OnEnable()
		{
			_elapsed = 0.0f;
			FadeOut();
		}

		[Serializable]
		public class TransitionFinishedEvent : UnityEvent
		{
		}

		public void FadeIn()
		{
			Button ntm;
			_elapsed = 0.0f;
			_fade = 0.0f;
			_targetFade = 1.0f;
			StartCoroutine(FadeCoroutine());
		}

		public void FadeOut()
		{
			_elapsed = 0.0f;
			_fade = 1.0f;
			_targetFade = 0.0f;
			StartCoroutine(FadeCoroutine());
		}

		private IEnumerator FadeCoroutine()
		{
			while (!Mathf.Approximately(_targetFade, _fade))
			{
				_elapsed += Time.deltaTime;
				var t = _elapsed / fadeDuration;
				_fade = Mathf.Lerp(_fade, _targetFade, fadeCurve.Evaluate(t));

				_targetCanvasGroup.alpha = _fade;

				yield return null;
			}

			onFadeFinished?.Invoke();
		}
	}
}
