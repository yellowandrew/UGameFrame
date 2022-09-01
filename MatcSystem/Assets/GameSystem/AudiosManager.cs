using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiosManager 
{
    private static readonly Lazy<AudiosManager> lazy =
    new Lazy<AudiosManager>(() => new AudiosManager());
    public static AudiosManager Instance { get { return lazy.Value; } }
    Dictionary<string, AudioClip> clips;

    AudioSource _musicSource, _soundSource;
    AudiosManager()
    {
        clips = new Dictionary<string, AudioClip>();
        var sounds = Resources.LoadAll<AudioClip>("Audioss");
        foreach (var sound in sounds)
        {
            clips.Add(sound.name, sound);
        }

        var audioObj = new GameObject("AudiosManager");
        _musicSource = audioObj.AddComponent<AudioSource>();
        _soundSource = audioObj.AddComponent<AudioSource>();

    }


    public void PlaySound(string clip, float vol = 1, bool loop = false)
    {
        _soundSource.volume = vol;
        _soundSource.loop = loop;
        _soundSource.PlayOneShot(clips[clip]);

    }
    public void PlayMusic(string clip, float vol = 1, bool loop = true)
    {
        _musicSource.volume = vol;
        _musicSource.loop = loop;
        _musicSource.clip = clips[clip];
        _musicSource.Play();
    }
}
