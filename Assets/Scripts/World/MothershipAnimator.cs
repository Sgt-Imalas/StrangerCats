using Assets.Scripts;
using Assets.Scripts.Rendering;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MothershipAnimator : MonoBehaviour
{
	public List<GameObject> EngineAnimations = new();
	public List<GameObject> ToDisableDuringAnim = new();
	public GameObject Player;
	public GameObject Colliders;
	public SpriteRenderer HangarOverlay;
	public PixelScaleCamera PixelCam;
	public AnimationCurve scaleCurve2 = AnimationCurve.EaseInOut(0, 0, 1, 1);
	float animatedTravelDistance = 120;
	float flyAnimDuration = 3f;

	Collider2D[] colliders;
	private void Awake()
	{
		colliders = Colliders.GetComponentsInChildren<Collider2D>();
	}
	private void Start()
	{
		GlobalEvents.Instance.OnNewMapGenerated += OnNewMapGenerated;
		AnimateArrival();
	}
	private void OnDestroy()
	{
		GlobalEvents.Instance.OnNewMapGenerated -= OnNewMapGenerated;

		Global.Instance.InShipAnimation = false;
	}

	private void OnNewMapGenerated(Dictionary<Vector3Int, int> materials, PlanetDescriptor descriptor)
	{
		transform.position = descriptor.playerSpawnPoint;
		AnimateArrival();
	}

	public void AnimateArrival()
	{
		StartCoroutine(LandAnim());

	}
	void ToggleAllDisableables(bool on)
	{
		foreach (var go in ToDisableDuringAnim)
			go.SetActive(on);
	}

	public void AnimateLeaving(bool forcePlayerNow = false)
	{
		StartCoroutine(LeaveAnim(forcePlayerNow));
	}

	IEnumerator LeaveAnim(bool forcePlayerPositionNow)
	{
		if (forcePlayerPositionNow)
			Player.transform.position = HangarOverlay.transform.position;
		Global.Instance.InShipAnimation = true;


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
		ToggleAllDisableables(false);
		yield return new WaitForSecondsRealtime(0.3f);

		Player.transform.position = HangarOverlay.transform.position;
		Player?.gameObject.SetActive(false);
		foreach (var anim in EngineAnimations)
		{
			anim.SetActive(true);
			yield return new WaitForSecondsRealtime(0.05f);
		}


		foreach (var collider in colliders)
			collider.gameObject.SetActive(false);

		time = 0f; fadeDuration = flyAnimDuration;
		var pos = transform.position;
		var targetPos = transform.position;
		targetPos.x += animatedTravelDistance;
		if (PixelCam != null)
			PixelCam._targetZoom = 60;
		while (time < fadeDuration)
		{
			time += Time.unscaledDeltaTime;
			var t = time / fadeDuration;
			var scaled = scaleCurve2.Evaluate(t);
			transform.position = Vector3.Lerp(pos, targetPos, scaled);
			yield return null;
		}
		Global.Instance.InShipAnimation = false;
		Player?.gameObject.SetActive(true);

		ToggleAllDisableables(true);
		Global.StartLoadingStarmapScene();

	}


	IEnumerator LandAnim()
	{
		ToggleAllDisableables(false);
		Global.Instance.InShipAnimation = true;
		Player?.gameObject.SetActive(false);
		Player.transform.position = HangarOverlay.transform.position;
		var hangarColor = HangarOverlay.color;
		hangarColor.a = 1;
		HangarOverlay.color = hangarColor;


		foreach (var anim in EngineAnimations)
			anim.SetActive(true);

		foreach (var collider in colliders)
			collider.gameObject.SetActive(false);

		float time = 0f, fadeDuration = 0.75f;

		time = fadeDuration = flyAnimDuration;
		var pos = transform.position;
		var targetPos = transform.position;
		targetPos.x -= animatedTravelDistance;

		while (time > 0)
		{
			time -= Time.unscaledDeltaTime;
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

		time = 0f; fadeDuration = 0.75f;
		while (time < fadeDuration)
		{
			time += Time.unscaledDeltaTime;
			var t = time / fadeDuration;
			HangarOverlay.color = Color.Lerp(startColor, endColor, t);
			yield return null;
		}

		foreach (var collider in colliders)
			collider.gameObject.SetActive(true);

		Global.Instance.InShipAnimation = false;
		ToggleAllDisableables(true);
	}
}