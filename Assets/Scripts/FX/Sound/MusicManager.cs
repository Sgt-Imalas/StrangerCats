using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

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

	float Volume = 1.0f;
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
		Volume = activeSource.volume;


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
		if (Instance == null || Instance.Songs == null || Instance.Songs.Length <= index)
		{
			Debug.LogWarning($"Invalid song index: {index}. Cannot play music.");
			return;
		}


		var song = Instance.Songs[index];

		//Instance.audioSource.outputAudioMixerGroup = Instance.AudioMixer.FindMatchingGroups("Music")[0];

		//Instance.CancelActiveTransition();

		Instance.PlayOrQueue(song.GetSound(), fadeDuration);
	}
	public static void PlayFx(AudioClip sound, float volume = 1.0f)
	{
		Instance.SfxSource.PlayOneShot(sound, volume);
	}

	Queue<Tuple<AudioClip, float>> Queued = new();
	void PlayOrQueue(AudioClip clip, float fadeDuration)
	{

		if(CurrentCrossfade != null)
			Queued.Enqueue(new(clip, fadeDuration));
		
		else
			CurrentCrossfade = StartCoroutine(Instance.Crossfade(clip, fadeDuration));
	}

	//void CancelActiveTransition()
	//{
	//	if (Instance.CurrentCrossfade != null)
	//	{
	//		Debug.Log("Canceling current transition");
	//		Instance.StopCoroutine(Instance.CurrentCrossfade);
	//		OnCrossfadeComplete();
	//	}
	//}
	void OnCrossfadeComplete()
	{
		activeSource.Stop();
		activeSource.volume = Volume;
		inactiveSource.volume = Volume;
		AudioSource temp = activeSource;
		activeSource = inactiveSource;
		inactiveSource = temp;
		CurrentCrossfade = null;

		var next = Queued.Dequeue();
		if (next != null)
			PlayOrQueue(next.Item1,next.Item2);
	}

	Coroutine CurrentCrossfade;

	private IEnumerator Crossfade(AudioClip newClip, float duration)
	{
		Debug.Log("now playing song: " + newClip.name);
		inactiveSource.Stop();
		inactiveSource.clip = newClip;
		inactiveSource.volume = 0.001f;
		inactiveSource.Play();

		var time = 0f;
		var startVolume = activeSource.volume;

		while (time < duration)
		{
			time += Time.deltaTime;
			var t = time / duration;

			activeSource.volume = Mathf.Lerp(startVolume, 0f, t);
			inactiveSource.volume = Mathf.Lerp(0f, Volume, t);

			yield return null;
		}
		OnCrossfadeComplete();
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