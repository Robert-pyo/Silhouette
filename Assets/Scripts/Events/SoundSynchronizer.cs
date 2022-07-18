using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSynchronizer : MonoBehaviour
{
    public AudioSource source;
    public ESoundType type;

    private void Awake()
    {
        if (!source)
        {
            source = GetComponent<AudioSource>();
        }

        SyncData();

        SoundManager.Instance.onSoundValueChangeEvent += SyncData;
    }

    public void SyncData()
    {
        //print("SyncSoundData");
        if (type == ESoundType.Bgm)
        {
            source.volume = SoundManager.Instance.soundOption.volume_BGM / 100;
        }
        else
        {
            source.volume = SoundManager.Instance.soundOption.volume_Effect / 100;
        }
    }
}
