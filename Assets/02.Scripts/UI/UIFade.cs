using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
public class UIFade : Singleton<UIFade>
{
    public float fadeTime = 3f;
    private float offset;
    
    public HashSet<GameObject> playingUI = new();

    private void Awake()
    {
        offset = 1 / fadeTime;
    }

    public bool IsPlaying(GameObject ui)
    {
        return playingUI.Contains(ui);
    }

    public void FadeUI(Image ui, float start, float end)
    {
        if (IsPlaying(ui.gameObject)) StopCoroutine(FadeCoroutine(ui, start, end));

        StartCoroutine(FadeCoroutine(ui, start, end));
    }

    public void FadeUI(TMP_Text ui, float start, float end)
    {
        if (IsPlaying(ui.gameObject)) StopCoroutine(FadeCoroutine(ui, start, end));

        StartCoroutine(FadeCoroutine(ui, start, end));
    }

    IEnumerator FadeCoroutine(Image ui, float start, float end)
    {
        playingUI.Add(ui.gameObject);

        Color fadeColor = ui.color;
        fadeColor.a = start;
        ui.color = fadeColor;

        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float a = Mathf.MoveTowards(fadeColor.a, end, offset * Time.deltaTime);
            fadeColor.a = a;
            ui.color = fadeColor;
            yield return null;
        }

        playingUI.Remove(ui.gameObject);
    }

    IEnumerator FadeCoroutine(TMP_Text ui, float start, float end)
    {
        playingUI.Add(ui.gameObject);
        
        Color fadeColor = ui.color;
        fadeColor.a = start;
        ui.color = fadeColor;

        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float a = Mathf.MoveTowards(fadeColor.a, end, offset * Time.deltaTime);
            fadeColor.a = a;
            ui.color = fadeColor;
            yield return null;
        }

        playingUI.Remove(ui.gameObject);
    }
}
