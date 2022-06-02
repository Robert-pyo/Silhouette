using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VibrationGenerator : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform m_genPoint;
    [SerializeField] private float m_power;
    [SerializeField] private float m_delayTime;
    
    [SerializeField] private bool m_isActivated;

    [SerializeField] private Wire m_linkedWire;

    private Outline m_outline;

    private void Awake()
    {
        m_outline = gameObject.AddComponent<Outline>();

        m_outline.enabled = false;
        m_outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        m_outline.OutlineColor = Color.cyan;

        m_isActivated = false;

        GameManager.Instance.onVisionWardActivated += ActivateVisionWard;
        GameManager.Instance.player.addWireToWardEvent += AddWireToWard;
    }

    private void ActivateVisionWard()
    {
        if (m_isActivated) return;

        m_isActivated = true;
        StartCoroutine(nameof(GenerateVibration));
        m_outline.enabled = true;
        GameManager.Instance.onVisionWardActivated -= ActivateVisionWard;
        GameManager.Instance.onVisionWardDeactivated += DeactivateVisionWard;
    }

    private void DeactivateVisionWard()
    {
        print("deactivate");
        if (!m_isActivated) return;

        m_isActivated = false;

        Destroy(m_linkedWire.gameObject);
        m_linkedWire = null;
        
        StopCoroutine(nameof(GenerateVibration));
        m_outline.enabled = false;
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

    private void AddWireToWard(Wire linkedWire)
    {
        m_linkedWire = linkedWire;
    }

    [SerializeField] private ushort m_maxHp;
    [SerializeField] private short m_curHp;
    public ushort MaxHp => m_maxHp;
    public short CurHp => m_curHp;
    public bool IsDead => !m_isActivated;

    public void Hit(ushort damage)
    {
        m_curHp -= (short) damage;

        if (m_curHp <= 0)
        {
            GameManager.Instance.OnWardDisabled();
        }
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }
}
