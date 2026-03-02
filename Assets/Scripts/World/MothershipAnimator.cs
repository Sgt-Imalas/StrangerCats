using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MothershipAnimator : MonoBehaviour
{
	public List<GameObject> EngineAnimations = new();
	public GameObject Player;
	public GameObject Colliders;
	public SpriteRenderer HangarOverlay;
	public AnimationCurve scaleCurve2 = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float AnimatedTravelDistance = 50;

	Collider2D[] colliders;
	private void Awake()
	{
		colliders = Colliders.GetComponentsInChildren<Collider2D>();
	}
	public void AnimateArrival()
	{
		Global.Instance.InShipAnimation = true;



		Global.Instance.InShipAnimation = false;
	}

	public void AnimateLeaving()
	{
		StartCoroutine(LeaveAnim());
	}

	IEnumerator LeaveAnim()
	{
		Global.Instance.InShipAnimation = true;
		
		foreach(var collider in colliders) 
			collider.gameObject.SetActive(false);
		
		float time = 0f, fadeDuration = 0.75f;

		var startColor = HangarOverlay.color;
		var endColor = startColor;
		startColor.a = 0;
		endColor.a = 1;

		while (time < fadeDuration)
		{
			time += Time.unscaledDeltaTime;
			var t = time / fadeDuration;
			HangarOverlay.color = Color.Lerp(startColor, endColor, t);
			yield return null;
		}
		yield return new WaitForSecondsRealtime(0.3f);

		foreach (var anim in EngineAnimations)
		{
			anim.SetActive(true);
			yield return new WaitForSecondsRealtime(0.05f);
		}

		Player?.gameObject.SetActive(false);
		time = 0f; fadeDuration = 4f;
		var pos = transform.position;
		var targetPos = transform.position;
		targetPos.x += AnimatedTravelDistance;

		while (time < fadeDuration)
		{
			time += Time.unscaledDeltaTime;
			var t = time / fadeDuration;
			var scaled = scaleCurve2.Evaluate(t);
			transform.position = Vector3.Lerp(pos,targetPos, scaled);			
			yield return null;
		}
		Global.Instance.InShipAnimation = false;
		Player?.gameObject.SetActive(true);

		//Global.Instance.StartLoadingStarmapScene();
	}


	IEnumerator LandAnim()
	{
		Global.Instance.InShipAnimation = true;
		Player?.gameObject.SetActive(false);

		foreach (var collider in colliders)
			collider.gameObject.SetActive(false);

		float time = 0f, fadeDuration = 0.75f;

		


		time = 0f; fadeDuration = 4f;
		var pos = transform.position;
		var targetPos = transform.position;
		targetPos.x += AnimatedTravelDistance;

		while (time < fadeDuration)
		{
			time += Time.unscaledDeltaTime;
			var t = time / fadeDuration;
			var scaled = scaleCurve2.Evaluate(t);
			transform.position = Vector3.Lerp(pos, targetPos, scaled);
			yield return null;
		}

		foreach (var anim in EngineAnimations)
		{
			anim.SetActive(false);
			yield return new WaitForSecondsRealtime(0.05f);
		}

		Player?.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(0.3f);
		var startColor = HangarOverlay.color;
		var endColor = startColor;
		startColor.a = 1;
		endColor.a = 0;

		while (time < fadeDuration)
		{
			time += Time.unscaledDeltaTime;
			var t = time / fadeDuration;
			HangarOverlay.color = Color.Lerp(startColor, endColor, t);
			yield return null;
		}

		Global.Instance.InShipAnimation = false;
		foreach (var collider in colliders)
			collider.gameObject.SetActive(true);
	}

	private void OnDestroy()
	{
		Global.Instance.InShipAnimation = false;
	}
}