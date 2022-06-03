using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
    private static SoundWaveManager m_instance;
    public static SoundWaveManager Instance => m_instance;

    public SoundWaveFx soundWavePrefab;
    private GameObject m_soundVisualizer;

    private ObjectPool<SoundWaveFx> m_soundWavePool;

    private void Awake()
    {
        if (m_instance)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_soundWavePool = new ObjectPool<SoundWaveFx>(
            createFunc: () => Instantiate(soundWavePrefab, new Vector3(1000f, 1000f, 1000f), Quaternion.identity),
            actionOnGet: (soundFx) =>
            {
                if (!soundFx) return;
                soundFx.gameObject.SetActive(true);
            },
            actionOnRelease: (soundFx) =>
            {
                if (!soundFx) return;
                soundFx.gameObject.SetActive(false);
            },
            actionOnDestroy: (soundFx) =>
            {
                if (!soundFx) return;
                Destroy(soundFx.gameObject);
            }, maxSize: 50);

        SceneController.Instance.onSceneChangeEvent += Reset;
    }

    private void Reset()
    {
        m_soundWavePool.Clear();
        m_soundWavePool = new ObjectPool<SoundWaveFx>(
            createFunc: () => Instantiate(soundWavePrefab, new Vector3(1000f, 1000f, 1000f), Quaternion.identity),
            actionOnGet: (soundFx) =>
            {
                if (!soundFx) return;
                soundFx.gameObject.SetActive(true);
            },
            actionOnRelease: (soundFx) =>
            {
                if (!soundFx) return;
                soundFx.gameObject.SetActive(false);
            },
            actionOnDestroy: (soundFx) =>
            {
                if (!soundFx) return;
                Destroy(soundFx.gameObject);
            }, maxSize: 50);
    }

    public GameObject GenerateSoundWave(Transform generator, Vector3 hitPos, Vector3 hitDir, float powerSize)
    {
        if (powerSize > 20)
        {
            powerSize = 20;
        }

        var _soundFx = m_soundWavePool.Get();
        
        ParticleSystem.MainModule _particleMain = _soundFx.soundWaveFx.main;
        _particleMain.startSize = powerSize;
        
        _soundFx.soundVelocity = powerSize;
        _soundFx.transform.position = hitPos;
        _soundFx.transform.localRotation = Quaternion.Euler(hitDir);
        _soundFx.transform.parent = generator;
        
        GameObject _visualizer = _soundFx.transform.GetChild(0).gameObject;

        StartCoroutine(GenerateVisualizer(_soundFx, _visualizer, powerSize));

        return _soundFx.gameObject;
    }

    private IEnumerator GenerateVisualizer(SoundWaveFx soundFx, GameObject visualizer, float powerSize)
    {
        if (!visualizer) yield break;
        yield return StartCoroutine(StartVisualizer(visualizer, powerSize));
        if (!visualizer) yield break;
        yield return StartCoroutine(ReleaseVisible(visualizer));
        
        if (visualizer)
        {
            visualizer.tag = "Untagged";
        }

        m_soundWavePool.Release(soundFx);
    }

    private IEnumerator StartVisualizer(GameObject visualizer, float powerSize)
    {
        float _visualizerDuration = 1.5f * (powerSize * 0.1f);
        if (_visualizerDuration < 0.3f) _visualizerDuration = 0.3f;
        float _visualizerRetainedTime = 0f;

        while (visualizer && visualizer.transform.localScale.x < powerSize)
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
        while (visualizer && visualizer.transform.localScale.x > 0.1f)
        {
            visualizer.transform.localScale -= Vector3.one * 0.2f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
