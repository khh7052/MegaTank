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

    void OffsetUpdate(float time)
    {
        offset = 1 / time;
    }

    public bool IsPlaying(GameObject ui)
    {
        return playingUI.Contains(ui);
    }

    public void FadeUI(Image ui, float start, float end, float time = 0)
    {
        if (time == 0) time = fadeTime;
        if (IsPlaying(ui.gameObject)) StopAllCoroutines();

        StartCoroutine(FadeCoroutine(ui, start, end, time));
    }

    public void FadeUI(TMP_Text ui, float start, float end, float time = 0)
    {
        if (time == 0) time = fadeTime;
        if (IsPlaying(ui.gameObject)) StopAllCoroutines();

        StartCoroutine(FadeCoroutine(ui, start, end, time));
    }

    IEnumerator FadeCoroutine(Image ui, float start, float end, float endTime)
    {
        playingUI.Add(ui.gameObject);

        OffsetUpdate(endTime);
        Color fadeColor = ui.color;
        fadeColor.a = start;
        ui.color = fadeColor;

        float time = 0;

        while (time < endTime)
        {
            time += Time.deltaTime;
            float a = Mathf.MoveTowards(fadeColor.a, end, offset * Time.deltaTime);
            fadeColor.a = a;
            ui.color = fadeColor;
            yield return null;
        }

        playingUI.Remove(ui.gameObject);
    }

    IEnumerator FadeCoroutine(TMP_Text ui, float start, float end, float endTime)
    {
        playingUI.Add(ui.gameObject);

        OffsetUpdate(endTime);
        Color fadeColor = ui.color;
        fadeColor.a = start;
        ui.color = fadeColor;

        float time = 0;

        while (time < endTime)
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
