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
    
    [Header("Destroy Info")]
    [SerializeField] private ushort m_maxHp;
    [SerializeField] private short m_curHp;
    public ushort MaxHp => m_maxHp;
    public short CurHp => m_curHp;
    public bool IsDead => !m_isActivated;

    [Header("Sounds")]
    public AudioClip activateSound;
    public AudioClip deactivateSound;
    public AudioClip vibrationSound;
    private AudioSource m_audioSource;

    private Outline m_outline;

    private void Awake()
    {
        m_outline = gameObject.AddComponent<Outline>();

        m_outline.enabled = false;
        m_outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        m_outline.OutlineColor = Color.cyan;

        m_isActivated = false;

        m_audioSource = GetComponent<AudioSource>();
        
        GameManager.Instance.onVisionWardActivated += ActivateVisionWard;

        // 씬 전환 시 Awake 실행보다 GameManager의 플레이어 재할당이 느리므로 지연하여 실행
        Invoke(nameof(Init), 1f);
    }

    private void Init()
    {
        if (!GameManager.Instance.Player) return;
        print("Init");
        GameManager.Instance.Player.addWireToWardEvent += AddWireToWard;
    }

    private void ActivateVisionWard()
    {
        if (!m_isActivated) return;

        m_isActivated = true;
        StartCoroutine(nameof(GenerateVibration));
        m_outline.enabled = true;
        
        SoundManager.Instance.PlayAt(activateSound, m_audioSource);

        GameManager.Instance.onVisionWardActivated -= ActivateVisionWard;
        GameManager.Instance.onVisionWardDeactivated += DeactivateVisionWard;
    }

    private void DeactivateVisionWard()
    {
        print("deactivate");
        if (m_isActivated) return;

        m_isActivated = false;

        Destroy(m_linkedWire.gameObject);
        m_linkedWire = null;
        
        SoundManager.Instance.PlayAt(deactivateSound, m_audioSource);
        
        StopCoroutine(nameof(GenerateVibration));
        m_outline.enabled = false;
        GameManager.Instance.onVisionWardDeactivated -= DeactivateVisionWard;
        GameManager.Instance.onVisionWardActivated += ActivateVisionWard;
        GameManager.Instance.Player.addWireToWardEvent += AddWireToWard;
    }

    private IEnumerator GenerateVibration()
    {
        while (true)
        {
            GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
            transform, m_genPoint.position, m_genPoint.position, m_power);

            _obj.transform.GetChild(0).tag = "VisionSound";
            
            SoundManager.Instance.PlayAt(vibrationSound, m_audioSource);
            
            yield return new WaitForSeconds(m_delayTime);
        }
    }

    private void AddWireToWard(Wire linkedWire)
    {
        if (!m_isActivated) return;
        m_linkedWire = linkedWire;
        GameManager.Instance.Player.addWireToWardEvent -= AddWireToWard;
    }

    public void StateToggle()
    {
        m_isActivated = !m_isActivated;
    }

    public void Hit(ushort damage)
    {
        m_curHp -= (short) damage;

        if (m_curHp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        StateToggle();
        GameManager.Instance.OnWardDisabled();
    }
}
