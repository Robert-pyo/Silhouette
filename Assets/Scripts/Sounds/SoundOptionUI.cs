using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SoundOptionUI : MonoBehaviour
{
    public SoundOptionData optionData;

    public Slider bgmSlider;
    public Slider effectSlider;

    public UnityAction onSoundValueChanged;

    private void Awake()
    {
        optionData = SoundManager.Instance.soundOption;

        onSoundValueChanged += SoundManager.Instance.OnSoundValueChange;
    }

    public void BGMVolumeChange()
    {
        optionData.volume_BGM = Mathf.Round(bgmSlider.value);
        onSoundValueChanged?.Invoke();
    }

    public void EffectVolumeChange()
    {
        optionData.volume_Effect = Mathf.Round(effectSlider.value);
        onSoundValueChanged?.Invoke();
    }

    public void SyncVolumeOption()
    {
        bgmSlider.value = optionData.volume_BGM;
        effectSlider.value = optionData.volume_Effect;
    }
}
