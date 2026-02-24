using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAnimator : MonoBehaviour
{
	public float buttonSpawnTime = 0.33f;
	public float buttonSpawnDelay = 0.2f;
	public GameObject Initializer;
	[SerializeField] public List<CatPawButton> buttons;
	public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (buttons == null)
			buttons = GetComponentsInChildren<CatPawButton>().ToList();
		foreach (var item in buttons)
		{
			item.gameObject.SetActive(false);
		}
	}

	public void AnimateSpawnButtons() => StartCoroutine(ButtonSpawnAnimation());

	IEnumerator ButtonSpawnAnimation()
	{
		if(Initializer != null)
		yield return UnRevealButtonAnim(Initializer);
		yield return null;
		foreach (var item in buttons)
		{
			yield return new WaitForSecondsRealtime(buttonSpawnDelay);
			yield return RevealButtonAnim(item.gameObject);
		}
		buttons.First()?.Select();
	}

	IEnumerator UnRevealButtonAnim(GameObject toUnReveal)
	{
		var startState = toUnReveal.transform.localScale;
		toUnReveal.transform.localScale = startState;
		float timer = 0, duration = 0.15f;
		var endState = new Vector2(0, 1);
		while (timer <= duration)
		{
			float t = Mathf.Clamp01(timer / duration);
			float curvedT = scaleCurve.Evaluate(t);

			toUnReveal.transform.localScale = Vector3.Lerp(startState, endState, curvedT);
			timer += Time.unscaledDeltaTime;
			yield return null;
		}
		toUnReveal.transform.localScale = endState;
		toUnReveal.SetActive(false);
	}

	IEnumerator RevealButtonAnim(GameObject button)
	{
		button.SetActive(true);
		var startState = new Vector2(0, 1);
		button.transform.localScale = startState;
		float timer = 0, duration = buttonSpawnTime;
		var endState = Vector3.one;
		while (timer <= duration)
		{
			float t = Mathf.Clamp01(timer / duration);
			float curvedT = scaleCurve.Evaluate(t);

			button.transform.localScale = Vector3.Lerp(startState, endState, curvedT);
			timer += Time.unscaledDeltaTime;
			yield return null;
		}
		button.transform.localScale = endState;
	}

}
