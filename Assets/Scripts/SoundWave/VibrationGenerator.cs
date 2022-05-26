using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationGenerator : MonoBehaviour
{
    [SerializeField] private Transform m_genPoint;
    [SerializeField] private float m_power;
    [SerializeField] private float m_delayTime;

    private Outline m_outline;

    private void Awake()
    {
        m_outline = gameObject.AddComponent<Outline>();

        m_outline.enabled = true;
        m_outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        m_outline.OutlineColor = Color.cyan;
    }

    private void OnEnable()
    {
        StartCoroutine(GenerateVibration());
    }

    private IEnumerator GenerateVibration()
    {
        while (true)
        {
            SoundWaveManager.Instance.GenerateSoundWave(
            transform, m_genPoint.position, m_genPoint.position, m_power);
            yield return new WaitForSeconds(m_delayTime);
        }
    }
}
