using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Soldier : Enemy
{
    private EnemyAI m_ai;

    [Header("Attack Info")]
    [SerializeField] private Transform m_attackPos;

    [SerializeField] private GameObject m_attackFx;

    private WaitForSeconds m_attackWaitTime;

    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int OnIdle = Animator.StringToHash("OnIdle");
    private static readonly int OnAttack = Animator.StringToHash("OnAttack");

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        
        // 애니메이터
        m_enemyAnim = GetComponentInChildren<Animator>();

        // 적 정보
        m_curHp = (short)m_data.maxHp;
        
        waypointSelector = FindObjectOfType<WaypointSelector>();
        if (!waypointSelector)
        {
            Debug.LogError("wayPoint가 지정되지 않았습니다.");
        }
        
        m_ai = GetComponent<EnemyAI>();

        // 커맨드
        m_movementCommand = new NavMovement(m_agent, MoveSpeed, RotateSpeed);
        m_targetFinder = new SoundFirstPriorityTargetFinder(this);

        // 아웃라인
        m_outline = gameObject.AddComponent<Outline>();
        m_outline.enabled = true;
        m_outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        
        // 공격
        m_attackWaitTime = new WaitForSeconds(0.2f);
    }

    private void Update()
    {
        if (isDead) return;
        UpdateAnimation();
        m_enemyAnim.SetFloat(Velocity, Agent.velocity.sqrMagnitude);
    }

    public override void Move(Vector3 dest)
    {
        m_movementCommand.Execute(dest);
    }
    
    public override IEnumerator Attack()
    {
        while (true)
        {
            GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
                transform, m_attackPos.position, Vector3.zero, 10f);
            _obj.transform.GetChild(0).tag = "EnemySound";

            GameObject _muzzleFx = Instantiate(m_attackFx, m_attackPos.position, Quaternion.Euler(m_attackPos.eulerAngles));
            Destroy(_muzzleFx, 1f);
            
            soundDistributor.SoundPlayer(soundGroups, "RiffleShot", 0);
            
            if (Physics.Raycast(transform.position, transform.forward, out var _hit, float.MaxValue, LayerMask.GetMask("Player", "Interactable")))
            {
                IDamageable _damageable = _hit.transform.GetComponent<IDamageable>();
                
                if (_damageable is {IsDead: false})
                {
                    _damageable.Hit(Data.attackDamage);
                }
            }

            yield return m_attackWaitTime;
        }
    }

    public override void UpdateAnimation()
    {
        switch (m_ai.currentState)
        {
            case EEnemyState.Idle:
                m_enemyAnim.SetBool(OnAttack, false);
                break;
            case EEnemyState.Patrol:
                break;
            case EEnemyState.Trace:
                break;
            case EEnemyState.Sneak:
                break;
            case EEnemyState.Attack:
                m_enemyAnim.SetBool(OnAttack, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
