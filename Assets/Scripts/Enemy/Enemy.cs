using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EEnemyType
{
    Static,
    Dynamic
}

public abstract class Enemy : MonoBehaviour, IDamageable, IWalkable
{
    protected Animator m_enemyAnim;
    [SerializeField] protected EnemyData m_data;
    public EnemyData Data => m_data;

    [Header("Enemy Info")]
    [SerializeField] protected ushort m_curHp;
    public ushort MaxHp => m_data.maxHp;
    public ushort CurHp => m_curHp;

    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_rotateSpeed;
    public float MoveSpeed => m_moveSpeed;
    public float RotateSpeed => m_rotateSpeed;
    
    protected NavMeshAgent m_agent;
    public NavMeshAgent Agent => m_agent;
    protected EnemyMovement m_movementCommand;
    public WaypointSelector waypointSelector;

    [Header("TargetInfo")]
    public Transform target;
    protected TargetFinder m_targetFinder;

    [Header("Sound Wave")]
    [SerializeField] protected Transform m_groundCheckTransform;

    [Header("Enemy State")]
    public bool isDead;

    protected Outline m_outline;

    private static readonly int IsDead = Animator.StringToHash("IsDead");

    public void Hit(ushort damage)
    {
        m_curHp -= damage;

        if (m_curHp != 0) return;
        
        isDead = true;
        Die();
    }

    public abstract void Move(Vector3 dest);

    public abstract void Attack();

    public abstract void UpdateAnimation();

    public virtual void FindTarget()
    {
        target = m_targetFinder.FindTarget();
        print(target);
    }

    public void Die()
    {
        m_enemyAnim.SetBool(IsDead, true);
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
            GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
                _hit.transform, _hit.point, Vector3.zero, m_moveSpeed);

            _obj.transform.GetChild(0).tag = "EnemySound";
        }
    }
}
