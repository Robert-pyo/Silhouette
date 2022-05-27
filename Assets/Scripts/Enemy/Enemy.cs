using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class Enemy : MonoBehaviour, IDamageable, IWalkable
{
    private Animator m_enemyAnim;

    [Header("Enemy Info")]
    [SerializeField] private ushort m_maxHp;
    [SerializeField] private ushort m_curHp;
    public ushort MaxHp => m_maxHp;
    public ushort CurHp => m_curHp;

    [SerializeField] private float m_moveSpeed;
    public float MoveSpeed => m_moveSpeed;

    [Header("Sound Wave")]
    [SerializeField] private Transform m_groundCheckTransform;

    [Header("Enemy State")]
    public bool isDead;

    private void Awake()
    {
        // 애니메이터
        m_enemyAnim = GetComponentInChildren<Animator>();

        m_curHp = m_maxHp;
    }

    public void Hit(ushort damage)
    {
        m_curHp -= damage;

        if (m_curHp == 0)
        {
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        m_enemyAnim.SetBool("OnDead", true);
        // 현재 OnDead 애니메이션이 종료되었다면
        if (m_enemyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            // TODO : 게임 오버 처리
        }
    }

    public void GenerateWalkSoundWave()
    {
        // 걸을 때 음파 생성
        if (Physics.Raycast(m_groundCheckTransform.position, Vector3.down, out var _hit, float.MaxValue, LayerMask.GetMask("Ground")))
        {
            SoundWaveManager.Instance.GenerateSoundWave(
                _hit.transform, _hit.point, Vector3.zero, m_moveSpeed);
        }
    }
}
