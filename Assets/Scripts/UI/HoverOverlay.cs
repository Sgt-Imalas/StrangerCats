using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HoverOverlay : MonoBehaviour
{
	public enum Allignment
	{
		BottomRight, BottomLeft, TopRight, TopLeft
	}

	static HoverOverlay Instance;
	HoverInfo ActiveTarget = null;
	[SerializeField] GameObject HoverDialogue;
	[SerializeField] TextMeshProUGUI TextField;


	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
		SceneManager.activeSceneChanged += OnSceneChanged;
	}
	private void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnSceneChanged;
		if (Instance == this)
			Instance = null;
	}

	private void OnSceneChanged(Scene arg0, Scene arg1)
	{
		HideActiveInfo();
	}

	void HideActiveInfo()
	{
		HoverDialogue.gameObject.SetActive(false);
	}
	void ShowActiveInfo()
	{
		if (ActiveTarget != null)
		{
			string text = ActiveTarget.Text;
			TextField.SetText(text);
			HoverDialogue.gameObject.SetActive(true);
			var align = ActiveTarget.Allignment;
			var targetPos = ActiveTarget.transform.position;

			var ownRect = GetComponent<RectTransform>();
			var targetRect = ActiveTarget.GetComponent<RectTransform>();
			ownRect.anchorMin = targetRect.anchorMin;
			ownRect.anchorMax = targetRect.anchorMax;
			ownRect.pivot = targetRect.pivot;
			ownRect.position = targetRect.position;
			ownRect.sizeDelta = targetRect.sizeDelta;

			var dialogueContainerRect = HoverDialogue.GetComponent<RectTransform>();


			var heightDiff = targetRect.sizeDelta.y + 10f;
			var widthDiff = targetRect.sizeDelta.x + 10f;

			switch (align)
			{
				case Allignment.BottomRight:
					targetPos.y -= heightDiff;
					targetPos.x += widthDiff;
					break;

				case Allignment.BottomLeft:
					targetPos.x -= widthDiff;
					targetPos.y -= heightDiff;
					break;
				case Allignment.TopRight:
					targetPos.x += widthDiff;
					targetPos.y += heightDiff;
					break;
				case Allignment.TopLeft:
					targetPos.x -= widthDiff;
					targetPos.y += heightDiff;
					break;
			}


			SetAnchorPreset(dialogueContainerRect, align);
			HoverDialogue.transform.position = targetPos;
		}
	}

	static void SetAnchorPreset(RectTransform rect, Allignment allignment)
	{
		var v01 = new Vector2(0, 1);
		var v10 = new Vector2(1, 0);
		switch (allignment)
		{
			case Allignment.TopLeft:
				rect.anchorMin = v01;
				rect.anchorMax = v01;
				rect.pivot = v10;
				break;

			case Allignment.BottomLeft:
				rect.anchorMin = Vector2.zero;
				rect.anchorMax = Vector2.zero;
				rect.pivot = Vector2.one;
				break;

			case Allignment.BottomRight:
				rect.anchorMin = v10;
				rect.anchorMax = v10;
				rect.pivot = v01;
				break;
			case Allignment.TopRight:
				rect.anchorMin = Vector2.one;
				rect.anchorMax = Vector2.one;
				rect.pivot = Vector2.zero;
				break;
		}
	}

	public static void OnStartDisplaying(HoverInfo target)
	{
		if (Instance == null)
		{
			return;
		}

		if (Instance.ActiveTarget != target)
		{
			Instance.HideActiveInfo();
			Instance.ActiveTarget = target;
			Instance.ShowActiveInfo();
		}
	}
	public static void OnStopDisplaying(HoverInfo target)
	{
		if (Instance == null)
		{
			return;
		}

		if (Instance.ActiveTarget == target)
		{
			Instance.HideActiveInfo();
			Instance.ActiveTarget = null;
		}
	}

}