using System.Collections;
using UnityEngine;

public class CardAnimation : MonoBehaviour
{
	public Transform ItemBox;
	public System.Action OnComplete;

	private void OnEnable()
	{
		StartCoroutine(Animation());
	}

	public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public AnimationCurve scaleCurve2 = AnimationCurve.EaseInOut(0, 0, 1, 1);
	IEnumerator Animation()
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.zero;
		//0.6s
		float timer = 0, duration = 0.6f;
		float totalRotations = 360 * 2;
		var targetRotation = Quaternion.Euler(0f, 0f, totalRotations);
		var startRotation = Quaternion.identity;
		var maxedScale = Vector3.one;

		///spin towards camera and become large
		while (timer <= duration)
		{
			float t = Mathf.Clamp01(timer / duration);
			float curvedT = scaleCurve.Evaluate(t);

			transform.localScale = Vector3.Lerp(Vector3.zero, maxedScale, curvedT);
			float rotationValue = totalRotations * curvedT;

			transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);

			timer += Time.unscaledDeltaTime;

			yield return null;
		}
		//3s

		///wait there for 3 seconds
		yield return new WaitForSecondsRealtime(3);

		var disappearScale = Vector3.one * 0.05f;
		var startLocalPos = transform.localPosition;


		timer = 0;
		//1.4s
		duration = 1.4f;
		totalRotations = 360 * 3;

		var localTargetPosition = transform.InverseTransformPoint(ItemBox.position);
		///become small again and fly to collectibles position		
		while (timer <= duration)
		{
			float t = Mathf.Clamp01(timer / duration);
			float curvedT = scaleCurve2.Evaluate(t);

			transform.localScale = Vector3.Lerp(maxedScale, disappearScale, curvedT);
			float rotationValue = totalRotations * curvedT;

			transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
			transform.localPosition = Vector3.Lerp(startLocalPos, localTargetPosition, curvedT);

			timer += Time.unscaledDeltaTime;

			yield return null;
		}
		OnComplete?.Invoke();
		gameObject.SetActive(false);
	}
}
