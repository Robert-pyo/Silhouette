using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
    private static SoundWaveManager m_instance;
    public static SoundWaveManager Instance => m_instance;

    public GameObject soundWave;
    private ParticleSystem m_soundWaveFx;

    private void Awake()
    {
        if (m_instance)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_soundWaveFx = soundWave?.GetComponentInChildren<ParticleSystem>();
    }

    public void GenerateSoundWave(Transform generator, Vector3 hitPos, Vector3 hitDir, float powerSize)
    {
        if (powerSize > 30)
        {
            powerSize = 30;
        }
        
        ParticleSystem.MainModule _particleMain = m_soundWaveFx.main;
        _particleMain.startSize = powerSize;

        var _obj = Instantiate(soundWave, hitPos, Quaternion.Euler(hitDir));
        _obj.transform.parent = generator;

        Destroy(_obj, 3);
    }
}
