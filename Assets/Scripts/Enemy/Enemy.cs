using System;
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
    public Animator EnemyAnimator => m_enemyAnim;
    [SerializeField] protected EnemyData m_data;
    public EnemyData Data => m_data;

    [Header("Enemy Info")]
    [SerializeField] protected short m_curHp;
    public ushort MaxHp => m_data.maxHp;
    public short CurHp => m_curHp;
    public bool IsDead => isDead;

    public float viewRadius;
    [Range(0, 360)]public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    protected TargetFinder fovTargetFinder;

    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_rotateSpeed;
    public float MoveSpeed => m_moveSpeed;
    public float RotateSpeed => m_rotateSpeed;
    
    protected NavMeshAgent m_agent;
    public NavMeshAgent Agent => m_agent;
    protected EnemyMovement m_movement;
    public WaypointSelector waypointSelector;

    [Header("TargetInfo")]
    public Transform target;
    public float sqrTargetDistance;
    protected TargetFinder m_targetFinder;

    [Header("Sound Wave")]
    [SerializeField] protected Transform m_groundCheckTransform;

    [Header("Enemy State")]
    public bool isDead;
    
    [Header("Sounds")]
    public List<SoundGroup> soundGroups;
    public SoundDistributor soundDistributor;

    protected Outline m_outline;

    private static readonly int HashIsDead = Animator.StringToHash("IsDead");

    public abstract void Move(Vector3 dest);

    public abstract IEnumerator Attack();

    public abstract void UpdateAnimation();

    // Events
    public event Action<bool> OnBeCarefulEvent;

    public IEnumerator FindTarget()
    {
        while (true)
        {
            target = m_targetFinder.FindTarget();

            if (!target)
            {
                target = fovTargetFinder.FindTarget();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Hit(ushort damage)
    {
        m_curHp -= (short)damage;

        if (m_curHp != 0) return;

        isDead = true;
        Die();
    }

    public void Die()
    {
        m_enemyAnim.SetBool(HashIsDead, true);
        // 현재 OnDead 애니메이션이 종료되었다면
        if (m_enemyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            // 죽고 애니메이션 출력이 끝났다면 삭제
            Destroy(gameObject);
        }
    }

    private int m_stepCount = 0;
    public void GenerateWalkSoundWave()
    {
        // 걸을 때 음파 생성
        if (Physics.Raycast(m_groundCheckTransform.position, Vector3.down, out var _hit, float.MaxValue, LayerMask.GetMask("Ground")))
        {
            GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
                _hit.transform, _hit.point, Vector3.zero, m_moveSpeed);

            if (!_obj) return;
            _obj.transform.GetChild(0).tag = "EnemySound";

            float _volume = (m_moveSpeed * 2f) / SoundWaveManager.Instance.maxPower;
            
            soundDistributor.SoundPlayer(soundGroups, "Footstep", m_stepCount, _volume);
            SoundGroup _group = soundGroups.Find(group => group.groupName == "Footstep");
            m_stepCount = (m_stepCount + 1) % _group.audioClipList.Count;
        }
    }

    public void GenerateSoundWaveAt(Transform genTransform, float power)
    {
        GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
            transform, genTransform.position, Vector3.zero, power);

        _obj.transform.GetChild(0).tag = "EnemySound";
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        float _radian = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(_radian), 0, Mathf.Cos(_radian));
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Data.recognizeRange);

        Vector3 _rightDir = DirFromAngle(viewAngle * 0.5f, false);
        Vector3 _leftDir = DirFromAngle(-viewAngle * 0.5f, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + _leftDir * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + _rightDir * viewRadius);

        Gizmos.color = Color.green;
        if (target)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
            //float _targetDist = (target.transform.position - transform.position).magnitude;

            //if (_targetDist > viewRadius) return;
        }
    }
}
