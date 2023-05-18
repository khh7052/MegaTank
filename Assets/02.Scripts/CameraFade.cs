using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class CameraFade : MonoBehaviour
{
    public Image fadeImage;
    public TMP_Text dayText;
    public float fadeTime = 3f;

    private bool isPlaying = false;

    public float Alpha
    {
        get { return fadeImage.color.a; }
        set
        {
            Color c = fadeImage.color;
            c.a = value;
            fadeImage.color = c;

            c = dayText.color;
            c.a = value;
            dayText.color = c;
        }
    }

    public void FadeIn()
    {
        if (isPlaying) return;
        StartCoroutine(FadeCoroutine(0));
    }

    public void FadeOut()
    {
        if (isPlaying) return;
        StartCoroutine(FadeCoroutine(1));
    }

    IEnumerator FadeCoroutine(float end)
    {
        isPlaying = true;
        Color fadeColor = fadeImage.color;
        Color dayColor = dayText.color;
        float offset = 1 / fadeTime;
        float time = 0;


        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float a = Mathf.MoveTowards(Alpha, end, offset * Time.deltaTime);
            fadeColor.a = a;
            dayColor.a = a;
            fadeImage.color = fadeColor;
            dayText.color = dayColor;
            yield return null;
        }

        isPlaying = false;
    }
}
