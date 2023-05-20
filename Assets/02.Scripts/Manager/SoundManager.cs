using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer audioMixer;
    private AudioSource source;

    public AudioClip lobbyBGM;
    public AudioClip openingBGM;
    public AudioClip startBattleBGM;
    public AudioClip endBattleSFX;
    public AudioClip restBGM;
    public AudioClip winSFX;
    public AudioClip defeatSFX;

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

    public void PlayBGM(AudioClip clip = null)
    {
        if(clip == null) source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip[] clips)
    {
        int rand = Random.Range(0, clips.Length);

        source.PlayOneShot(clips[rand]);
    }

    public void SoundUpdate()
    {
        switch (GameManager.Instance.State)
        {
            case GameState.LOBBY:
                PlayBGM(lobbyBGM);
                SFXVolume = 0;
                break;
            case GameState.OPENING:
                PlayBGM(openingBGM);
                SFXVolume = -80;
                break;
            case GameState.STARTBATTLE:
                PlayBGM(startBattleBGM);
                SFXVolume = 0;
                break;
            case GameState.ENDBATTLE:
                PlayBGM();
                PlaySFX(endBattleSFX);
                break;
            case GameState.SETTING:
            case GameState.REST:
                PlayBGM(restBGM);
                break;
            case GameState.WIN:
                PlaySFX(winSFX);
                break;
            case GameState.DEFEAT:
                PlaySFX(defeatSFX);
                break;
        }
    }
}
