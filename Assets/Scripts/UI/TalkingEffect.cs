using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class TalkingEffect : MonoBehaviour
{
	[TextArea]
	public string fullText;

	public float charactersPerSecond = 20f;

	private TextMeshProUGUI textComponent;

	public event Action OnTypingCompleted;

	AudioSource audioSource;

	void Awake()
	{
		textComponent = GetComponent<TextMeshProUGUI>();
		audioSource = GetComponent<AudioSource>();
	}

	public void SetTextAndStartTyping(string text)
	{
		fullText = text;
		StartTyping();
	}

	private void OnEnable()
	{
		StartTyping();
	}

	public void StartTyping()
	{
		StartCoroutine(TypeText());
	}

	IEnumerator TypeText()
	{
		textComponent.text = fullText;
		textComponent.maxVisibleCharacters = 0;
		if(audioSource != null) 
			audioSource.Play();

		int totalVisibleCharacters = fullText.Count();
		float delay = 1f / charactersPerSecond;

		while (textComponent.maxVisibleCharacters < totalVisibleCharacters)
		{
			textComponent.maxVisibleCharacters++;
			yield return new WaitForSecondsRealtime(delay);
		}
		OnTypingCompleted?.Invoke();
		if (audioSource != null)
			audioSource.Stop();
	}
}