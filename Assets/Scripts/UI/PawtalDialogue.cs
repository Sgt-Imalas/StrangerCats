using Assets.Scripts;
using System;
using System.Collections;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class PawtalDialogue : MonoBehaviour
{
	[SerializeField] TalkingEffect TalkingOutput;
	[SerializeField] Button Confirm;
	TextMeshProUGUI ButtonText;

	const string dialogue_firsttime =
		"Far away from home you are, little one.\r\n\r\n" +
		"Master Meowgurt I am\r\n\r\n" +
		"getting you home this pawtal will.\r\n\r\n" +
		"powered down, the it is however. \r\n\r\n" +
		"rekindle it, only 3 legendary items can.\r\n\r\n" +
		"find them, you must!\r\n\r\n" +
		"helping you I will.\r\n\r\n" +
		"Aiding you in your travels, this liquid schwartz will";

	//todo a bit
	const string dialogue_questing =
		"Your quest incomplete still is.\r\n\r\n" +
		"All the legendary items, you have not.\r\n\r\n" +
		"Collecting them, you must!\r\n\r\n" +
		"Revealing themselves, they will in time.\r\n\r\n" +
		"Improving your spaceship, you should!\r\n\r\n\r\n" +
		"Better chances it will give you in finding them!";

	//todo a bit
	const string dialogue_win =
		"All the items you have found.\r\n\r\n" +
		"Your quest complete now is.\r\n\r\n" +
		"the pawtal rekindle I will for you\r\n\r\n" +
		"Had fun on this quest I hope?";

	private void Awake()
	{
		ButtonText = Confirm.GetComponentInChildren<TextMeshProUGUI>();
		TalkingOutput.OnTypingCompleted += OnTalkingComplete;
		Confirm.onClick.AddListener(OnConfirm);
	}

	public void ShowPawtalDialogue()
	{
		gameObject.SetActive(true);
	}

	private void OnConfirm()
	{
		AdvanceStory();
		gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		SetStoryDialogue();
		Global.Instance.InDialogue = true;
		Time.timeScale = 0;
	}
	private void OnDisable()
	{
		Global.Instance.InDialogue = false;
		Time.timeScale = 1;
	}

	private void SetStoryDialogue()
	{
		///first meeting, giving schwartz
		if (!Global.Instance.Upgrades.SuperCruiseUnlocked)
		{
			StartTalking(dialogue_firsttime);
			ButtonText.SetText("Thanks Master Meowgurt!");
		}
		///not all 3 artifacts found yet
		else if (
			!Global.Instance.Upgrades.MeatWorldItemFound ||
			!Global.Instance.Upgrades.TennisWorldItemFound ||
			!Global.Instance.Upgrades.DesertWorldItemFound
			)
		{
			StartTalking(dialogue_questing);
			ButtonText.SetText("I will continue searching!");
		}
		///Game win
		else
		{
			StartTalking(dialogue_win);
			ButtonText.SetText("<Enter the lit Pawtal>");
		}
	}

	IEnumerator DelayedSuperCruiseDialogue()
	{
		yield return new WaitForSecondsRealtime(1);
		HintDialogue.ShowSuperCruiseDialogue();
	}

	private void AdvanceStory()
	{
		if (!Global.Instance.Upgrades.SuperCruiseUnlocked)
		{
			Global.Instance.Upgrades.CollectFindableItem(FindableItem.SuperCruise);

			PersistentPlayer.Instance.StartCoroutine(DelayedSuperCruiseDialogue());
		}
		///not all 3 artifacts found yet
		else if (
			!Global.Instance.Upgrades.MeatWorldItemFound ||
			!Global.Instance.Upgrades.TennisWorldItemFound ||
			!Global.Instance.Upgrades.DesertWorldItemFound
			)
		{
			//do nothing
		}
		///Game win
		else
		{
			///Finish the game
		}
	}

	private void OnTalkingComplete()
	{
		Confirm.interactable = true;
		Confirm.Select();
	}
	void StartTalking(string text)
	{
		Confirm.interactable = false;
		TalkingOutput.SetTextAndStartTyping(text);
	}
}
