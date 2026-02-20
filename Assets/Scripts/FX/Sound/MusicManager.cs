
using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
	[SerializeField] private Song[] Songs;
	private static MusicManager Instance;
	private AudioSource audioSource;
	public AudioMixer AudioMixer;
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
		audioSource = GetComponent<AudioSource>();
	
	}

	private void Start()
	{
		if (PlayerPrefs.HasKey("MusicVolume"))
			SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
		else
			Debug.Log("MusicVolume key not found in PlayerPrefs. Using default volume.");
		PlayMusic(0);
	}

	public static void PlayMusic(int index, float volume = 1f)
	{
		if(Instance.Songs == null || Instance.Songs.Length <= index)
		{
			Debug.LogWarning($"Invalid song index: {index}. Cannot play music.");
			return;
		}

		var song = Instance.Songs[index];
		Instance.audioSource.outputAudioMixerGroup = Instance.AudioMixer.FindMatchingGroups("Music")[0];
		Instance.audioSource.PlayOneShot(song.MusicFile, volume);
	}
	public static void SetMusicVolume(float volume)
	{
		if (Instance == null || Instance.AudioMixer == null)
		{
			Debug.LogWarning("MusicManager instance or audio source is null. Cannot set music volume.");
			return;
		}
		Debug.Log($"Setting music volume to {volume}");
		Instance.AudioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
	}

}
[Serializable]
public struct Song
{
	[HideInInspector] public string name;
	public AudioClip MusicFile;
}