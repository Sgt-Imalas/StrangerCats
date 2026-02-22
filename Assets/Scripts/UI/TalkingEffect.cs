using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class TalkingEffect : MonoBehaviour
{
	[TextArea]
	public string fullText = string.Empty;

	public float charactersPerSecond = 20f;

	[SerializeField]private TextMeshProUGUI textComponent;

	public event Action OnTypingCompleted;

	AudioSource audioSource;
	Coroutine current;

	void Awake()
	{
		Init();
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
		if(current != null)
			StopCoroutine(current);

		current = StartCoroutine(TypeText());
	}
	void Init()
	{
		if (textComponent == null)
			textComponent = GetComponent<TextMeshProUGUI>();
		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();
		if(fullText == null)
			fullText = string.Empty;
	}
	IEnumerator TypeText()
	{
		Init();
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