using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class CameraFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeTime = 3f;

    private bool isPlaying = false;

    public float Alpha
    {
        get { return fadeImage.color.a; }
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
        Color color = fadeImage.color;
        float offset = 1 / fadeTime;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            color.a = Mathf.MoveTowards(Alpha, end, offset * Time.deltaTime);
            fadeImage.color = color;

            yield return null;
        }

        isPlaying = false;
    }
}
