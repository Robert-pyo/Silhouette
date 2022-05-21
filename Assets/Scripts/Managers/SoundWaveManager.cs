using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
    private static SoundWaveManager m_instance;
    public static SoundWaveManager Instance => m_instance;

    public GameObject soundWave;
    private ParticleSystem m_soundWaveFx;
    private GameObject m_soundVisualizer;

    private void Awake()
    {
        if (m_instance)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
        DontDestroyOnLoad(gameObject);

        if (soundWave)
        {
            m_soundWaveFx = soundWave.GetComponentInChildren<ParticleSystem>();
        }
    }

    public void GenerateSoundWave(Transform generator, Vector3 hitPos, Vector3 hitDir, float powerSize)
    {
        if (powerSize > 20)
        {
            powerSize = 20;
        }
        
        ParticleSystem.MainModule _particleMain = m_soundWaveFx.main;
        _particleMain.startSize = powerSize;

        var _obj = Instantiate(soundWave, hitPos, Quaternion.Euler(hitDir));
        _obj.transform.parent = generator;
        var _visualizer = _obj.transform.Find("Visualizer").gameObject;
        
        //Destroy(_obj, m_visualizerDuration);

        StartCoroutine(GenerateVisualizer(_obj, _visualizer, powerSize));
    }

    private IEnumerator GenerateVisualizer(GameObject obj, GameObject visualizer, float powerSize)
    {
        yield return StartCoroutine(StartVisualizer(visualizer, powerSize));
        yield return StartCoroutine(ReleaseVisible(visualizer));
        Destroy(obj);
    }

    private IEnumerator StartVisualizer(GameObject visualizer, float powerSize)
    {
        float _visualizerDuration = 1.5f * (powerSize * 0.1f);
        if (_visualizerDuration < 0.3f) _visualizerDuration = 0.3f;
        float _visualizerRetainedTime = 0f;

        while (visualizer.transform.localScale.x < powerSize)
        {
            visualizer.transform.localScale += Vector3.one * 0.2f;
            yield return new WaitForSeconds(0.01f);
        }

        while (_visualizerRetainedTime < _visualizerDuration)
        {
            _visualizerRetainedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ReleaseVisible(GameObject visualizer)
    {
        while (visualizer.transform.localScale.x > 0.1f)
        {
            visualizer.transform.localScale -= Vector3.one * 0.2f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
