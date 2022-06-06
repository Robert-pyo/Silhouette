using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSound : MonoBehaviour
{
    public AudioClip titleBgm;
    public AudioClip uiClickEffect;
    public AudioClip uiHoverEffect;

    private void Awake()
    {
        SoundManager.Instance.Play(titleBgm, SoundType.Bgm, 0.5f, 0.85f);
    }

    public void OnClickButton()
    {
        SoundManager.Instance.Play(uiClickEffect);
    }

    public void OnHoverButton()
    {
        SoundManager.Instance.Play(uiHoverEffect);
    }
}
