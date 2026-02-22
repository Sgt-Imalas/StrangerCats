
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;

public class MusicManager : MonoBehaviour
{
	[SerializeField] private AudioCollection[] Songs;
	[SerializeField] private AudioCollection[] SFX;
	private static MusicManager Instance;
	//crossfading
	[SerializeField] 
	AudioSource MusicSourceA, MusicSourceB, 
		//sfx
		SfxSource;
	public AudioMixer AudioMixer;

	private AudioSource activeSource;
	private AudioSource inactiveSource;


	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			Debug.Log("MusicManager instance created");
		}
		else
		{
			Debug.Log("MusicManager instance already exists, destroying duplicate");
			Destroy(gameObject);
			return;
		}

		activeSource = MusicSourceA;
		inactiveSource = MusicSourceB;

		
	}

	private void Start()
	{
		if (PlayerPrefs.HasKey("MusicVolume"))
			SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
		else
			Debug.Log("MusicVolume key not found in PlayerPrefs. Using default volume.");

		if (PlayerPrefs.HasKey("sfxVolume"))
			SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
		//PlayNewSong(0);
	}

	public static void PlayNewSong(int index, float fadeDuration = 3f)
	{
		if (Instance == null)
			return;
		if(Instance.Songs == null || Instance.Songs.Length <= index)
		{
			Debug.LogWarning($"Invalid song index: {index}. Cannot play music.");
			return;
		}

		var song = Instance.Songs[index];

		//Instance.audioSource.outputAudioMixerGroup = Instance.AudioMixer.FindMatchingGroups("Music")[0];
		Instance.StartCoroutine(Instance.Crossfade(song.GetSound(), fadeDuration));
	}

	private IEnumerator Crossfade(AudioClip newClip, float duration)
	{
		inactiveSource.clip = newClip;
		inactiveSource.volume = 0f;
		inactiveSource.Play();

		float time = 0f;
		float startVolume = activeSource.volume;

		while (time < duration)
		{
			time += Time.deltaTime;
			float t = time / duration;

			activeSource.volume = Mathf.Lerp(startVolume, 0f, t);
			inactiveSource.volume = Mathf.Lerp(0f, startVolume, t);

			yield return null;
		}

		activeSource.Stop();
		activeSource.volume = 1f;

		AudioSource temp = activeSource;
		activeSource = inactiveSource;
		inactiveSource = temp;
	}


	public static void SetMusicVolume(float volume)
	{
		if (Instance == null || Instance.AudioMixer == null)
		{
			Debug.LogWarning("MusicManager instance or audio source is null. Cannot set music volume.");
			return;
		}
		Instance.AudioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
	}
	public static void SetSFXVolume(float volume)
	{
		if (Instance == null || Instance.AudioMixer == null)
		{
			Debug.LogWarning("MusicManager instance or audio source is null. Cannot set music volume.");
			return;
		}
		Instance.AudioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
	}

}
[Serializable]
public struct AudioCollection
{
	[HideInInspector] public string name;
	public List<AudioClip> MusicFiles;
	public AudioClip GetSound() => MusicFiles[UnityEngine.Random.Range(0, MusicFiles.Count)];
}