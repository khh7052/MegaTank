using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer audioMixer;
    private AudioSource source;

    public float BGMVolume
    {
        set
        {
            audioMixer.SetFloat("BGM", value);
        }
    }

    public float SFXVolume
    {
        set
        {
            audioMixer.SetFloat("SFX", value);
        }
    }

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip[] clips)
    {
        int rand = Random.Range(0, clips.Length);

        source.PlayOneShot(clips[rand]);
    }

    public void VolumeUpdate()
    {
        switch (GameManager.Instance.State)
        {
            case GameState.LOBBY:
                break;
            case GameState.OPENING:
                SFXVolume = -80;
                break;
            case GameState.STARTBATTLE:
                SFXVolume = 0;
                break;
            case GameState.ENDBATTLE:
                break;
            case GameState.REST:
                break;
        }
    }
}
