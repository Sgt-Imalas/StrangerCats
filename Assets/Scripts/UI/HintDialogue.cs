using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintDialogue : MonoBehaviour
{
	[SerializeField] TalkingEffect TalkingOutput;
	[SerializeField] Button Confirm;

	const string SchwarzHint = "Tip:\r\n\r\nPour the liquid Schwartz into your fuel tank with Q to go into supercruise mode";

	private static HintDialogue instance;
	private void Awake()
	{
		instance = this;

		TalkingOutput.OnTypingCompleted += OnTalkingComplete;
		Confirm.onClick.AddListener(OnConfirm);
		this.gameObject.SetActive(false);
	}

	private void OnTalkingComplete()
	{
		Confirm.interactable = true;
		Confirm.Select();
	}
	public static void ShowHintDialogue(string text)
	{
		if(instance == null)
		{
			Debug.LogWarning("HintDialogue was null?!");
			return;
		}
		instance.gameObject.SetActive(true);
		instance.StartTalking(text);
	}

	void StartTalking(string text)
	{
		Confirm.interactable = false;
		TalkingOutput.SetTextAndStartTyping(text);
	}
	private void OnConfirm()
	{
		gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		Global.Instance.InDialogue = true;
		Time.timeScale = 0;
	}
	private void OnDisable()
	{
		Global.Instance.InDialogue = false;
		Time.timeScale = 1;
	}

	internal static void ShowSuperCruiseDialogue()
	{
		ShowHintDialogue(SchwarzHint);
	}
}