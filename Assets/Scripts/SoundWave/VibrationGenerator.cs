using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VibrationGenerator : MonoBehaviour
{
    [SerializeField] private Transform m_genPoint;
    [SerializeField] private float m_power;
    [SerializeField] private float m_delayTime;
    
    [SerializeField] private bool m_isActivated;

    public UnityAction generatorEnableEvent;

    private Outline m_outline;

    private void Awake()
    {
        m_outline = gameObject.AddComponent<Outline>();

        m_outline.enabled = false;
        m_outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        m_outline.OutlineColor = Color.cyan;

        m_isActivated = false;
        generatorEnableEvent += ActivateGenerator;

        GameManager.Instance.onVisionWardActivated += ActivateVisionWard;
    }

    private void ActivateVisionWard()
    {
        if (!m_isActivated) return;
        
        StartCoroutine(nameof(GenerateVibration));
        m_outline.enabled = true;
        GameManager.Instance.onVisionWardActivated -= ActivateVisionWard;
        GameManager.Instance.onVisionWardDeactivated += DeactivateVisionWard;
    }

    private void DeactivateVisionWard()
    {
        if (m_isActivated) return;
        
        StopCoroutine(nameof(GenerateVibration));
        GameManager.Instance.onVisionWardDeactivated -= DeactivateVisionWard;
        GameManager.Instance.onVisionWardActivated += ActivateVisionWard;
    }

    private IEnumerator GenerateVibration()
    {
        while (true)
        {
            GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
            transform, m_genPoint.position, m_genPoint.position, m_power);

            _obj.transform.GetChild(0).tag = "VisionSound";
            
            yield return new WaitForSeconds(m_delayTime);
        }
    }

    private void ActivateGenerator()
    {
        m_isActivated = true;
    }
}
