using System;
using System.Collections.Generic;
using UnityEngine;
class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Serializable]
    public class Clip
    {
        public string name;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume;
        [Range(-3, 3)]
        public float pitch;
        public bool loop;
        [HideInInspector]
        public AudioSource source;
    }
    public List<Clip> musicClips;
    public List<Clip> sfxClips;
    List<AudioSource> musicSources = new List<AudioSource>();
    List<AudioSource> sfxSources = new List<AudioSource>();
    public bool isMusicOn;
    public bool isSfxOn;
    [Range(0, 1)]
    public float globalVolume;
    [Range(0, 1)]
    public float musicVolume;
    [Range(0, 1)]
    public float sfxVolume;
    void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject); }
    void Start()
    {
        GenerateSources(musicClips, musicSources);
        GenerateSources(sfxClips, sfxSources);
        AudioListener.volume = globalVolume;
        foreach (var i in musicSources) i.volume *= musicVolume;
        foreach (var i in sfxSources) i.volume *= sfxVolume;
    }
    void Update()
    {
        foreach (var i in musicSources) i.mute = !isMusicOn;
        foreach (var i in sfxSources) i.mute = !isSfxOn;
    }
    void GenerateSources(List<Clip> clips, List<AudioSource> sources)
    {
        foreach (var i in clips)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = i.clip;
            source.volume = i.volume;
            source.pitch = i.pitch;
            source.loop = i.loop;
            i.source = source;
            sources.Add(source);
        }
    }
    public void PlayMusic(string name) { musicClips.Find(i => i.name.Contains(name)).source.Play(); }
    public void StopMusic(string name) { musicClips.Find(i => i.name.Contains(name)).source.Stop(); }
    public void PlaySfx(string name) { sfxClips.Find(i => i.name.Contains(name)).source.Play(); }
    public void StopSfx(string name) { sfxClips.Find(i => i.name.Contains(name)).source.Stop(); }
}