using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Soldier : Enemy
{
    private EnemyAI m_ai;

    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int OnIdle = Animator.StringToHash("OnIdle");

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        
        // 애니메이터
        m_enemyAnim = GetComponentInChildren<Animator>();

        // 적 정보
        m_curHp = m_data.maxHp;
        
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
    }

    private void Update()
    {
        UpdateAnimation();
    }

    public override void Move(Vector3 dest)
    {
        m_movementCommand.Execute(dest);
    }
    
    public override void Attack()
    {
        
    }

    public override void UpdateAnimation()
    {
        switch (m_ai.currentState)
        {
            case EEnemyState.Idle:
            case EEnemyState.Patrol:
                m_enemyAnim.SetFloat(Velocity, Agent.velocity.sqrMagnitude);
                break;
            case EEnemyState.Trace:
                m_enemyAnim.SetFloat(Velocity, Agent.velocity.sqrMagnitude);
                break;
            case EEnemyState.Sneak:
                m_enemyAnim.SetFloat(Velocity, Agent.velocity.sqrMagnitude);
                break;
            case EEnemyState.Attack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
