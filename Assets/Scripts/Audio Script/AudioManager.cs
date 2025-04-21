using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    [System.Serializable]
    public class AudioEntry
    {
        public string name;
        public AudioClip clip;
    }

    public List<AudioEntry> musicClips;
    public List<AudioEntry> sfxClips;

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void BuildDictionaries()
    {
        musicDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, AudioClip>();

        foreach (var entry in musicClips)
            musicDict[entry.name] = entry.clip;

        foreach (var entry in sfxClips)
            sfxDict[entry.name] = entry.clip;
    }

    public void PlayMusic(string name)
    {
        if (musicDict.TryGetValue(name, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
