using Assets.Scripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PawtalDialogue : MonoBehaviour
{
	[SerializeField] TalkingEffect TalkingOutput;
	[SerializeField] Button Confirm;
	TextMeshProUGUI ButtonText;
	[SerializeField] RadarTarget PawtalRadar;
	[SerializeField] Button LeaveLaterButton;

	const string dialogue_firsttime =
		"Far away from home you are, little one.\r\n" +
		"Master Meowgurt, am I.\r\n" +
		"Getting you home this pawtal will.\r\n" +
		"Powered down however, it is. \r\n" +
		"Rekindle it, only 3 legendary items can.\r\n" +
		"Find them, you must!\r\n" +
		"Helping you, will I.\r\n" +
		"Aiding you in your travels, will this Liquid Schwartz.";

	//todo a bit
	const string dialogue_questing =

		"Your quest incomplete, it still is.\r\n" +
		"All the legendary items, you have not.\r\n" +
		"Collect them, you must!\r\n" +
		"Reveal themselves, they will in time.\r\n" +
		"Improve your spaceship, you should!\r\n\r\n" +
		"Better chances it will give you in finding them!";

	//todo a bit
	const string dialogue_win =
		"Achieved your goal, you have!\r\n" +
		"All the items, you have found.\r\n" +
		"Been reignited, the Pawtal has.\r\n" +
		"Now complete, your quest is.\r\n" +
		"Had fun on this quest I hope!\r\n" +
		"Leave, you will now? Hmm?";

	private void Awake()
	{
		ButtonText = Confirm.GetComponentInChildren<TextMeshProUGUI>();
		TalkingOutput.OnTypingCompleted += OnTalkingComplete;
		Confirm.onClick.AddListener(OnConfirm);
		LeaveLaterButton.onClick.AddListener(OnCancel);
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
	void OnCancel()
	{
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
		LeaveLaterButton.gameObject.SetActive(false);
		///first meeting, giving schwartz
		if (!Global.Instance.Upgrades.SuperCruiseUnlocked)
		{
			PawtalRadar.SetIgnoreDistanceLimit(true);
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
			ButtonText.SetText("I will continue my search!");
		}
		///Game win
		else
		{
			StartTalking(dialogue_win);
			ButtonText.SetText("<Enter the lit Pawtal>");
			LeaveLaterButton.gameObject.SetActive(true);
		}
	}

	IEnumerator DelayedSuperCruiseDialogue()
	{
		yield return new WaitForSecondsRealtime(6);
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
			Global.Instance.StartLoadingMainMenu();
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
