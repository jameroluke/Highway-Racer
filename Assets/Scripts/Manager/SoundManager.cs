using System.Collections.Generic;
using Common;
using UnityEngine;

[System.Serializable]
public class NamedClip {
    public string key;
    public AudioClip clip;
}

public class SoundManager : MonoSingleton<SoundManager> {
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Clips")]
    [SerializeField] private List<NamedClip> clips = new List<NamedClip>();

    private Dictionary<string, AudioClip> _map;

    public void Play(string key) {
        if (sfxSource == null) {
            return;
        }
        if (_map != null && _map.TryGetValue(key, out var clip)) {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayMusic(string key, bool loop = true) {
        if (musicSource == null) {
            return;
        }
        if (_map != null && _map.TryGetValue(key, out var clip)) {
            if (musicSource.clip != clip) {
                musicSource.clip = clip;
            }
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void StopMusic() {
        if (musicSource == null) {
            return;
        }
        musicSource.Stop();
    }

    public void PlayClick() {
        Play("click");
    }

    protected override void Init() {
        _map = new Dictionary<string, AudioClip>();
        foreach (var nc in clips) {
            if (nc != null && !string.IsNullOrEmpty(nc.key) && nc.clip != null) {
                _map[nc.key] = nc.clip;
            }
        }
    }
}
