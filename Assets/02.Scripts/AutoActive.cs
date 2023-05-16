using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoActive : MonoBehaviour
{
    public bool active = false;
    public float time = 5;

    void OnEnable()
    {
        CancelInvoke(nameof(SetActive));
        Invoke(nameof(SetActive), time);
    }

    void SetActive()
    {
        gameObject.SetActive(active);
    }
}
