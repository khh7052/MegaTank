using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXButton : MonoBehaviour
{
    private Button button;
    public AudioClip clickSound;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null) return;
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        SoundManager.Instance.PlaySFX(clickSound);
    }
}
