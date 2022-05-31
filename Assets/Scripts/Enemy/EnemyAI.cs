using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.Events;

public enum EEnemyState
{
    Idle,
    Patrol,
    Trace,
    Sneak,
    Attack,
}

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    private StateMachine<EEnemyState> m_enemyState;
    public EEnemyState currentState;

    private Enemy m_owner;

    [SerializeField] private float m_delayTimeIdleToPatrol;
    private float m_idleTimeTaken;
    private float m_attackTimeTaken;
    
    private static readonly int OnAttack = Animator.StringToHash("OnAttack");
    
    //public UnityAction stateChangeEvent;

    private void Awake()
    {
        m_enemyState = StateMachine<EEnemyState>.Initialize(this);
        // ������ ����
        m_owner = GetComponent<Enemy>();

        m_enemyState.ChangeState(EEnemyState.Idle);
    }

    private void Idle_Enter()
    {
        currentState = m_enemyState.State;
        m_idleTimeTaken = 0;
    }
    private void Idle_Update()
    {
        m_idleTimeTaken += Time.deltaTime;

        if (m_delayTimeIdleToPatrol <= m_idleTimeTaken && m_owner.waypointSelector)
        {
            m_enemyState.ChangeState(EEnemyState.Patrol);
            return;
        }
        
        m_owner.FindTarget();

        // TODO : �Ҹ��� �����Ǿ��ٸ� �������� Trace
        if (m_owner.target)
        {
            m_enemyState.ChangeState(EEnemyState.Trace);
        }
    }

    private void Patrol_Enter()
    {
        currentState = m_enemyState.State;
    }
    private void Patrol_Update()
    {
        // TODO : �Ҹ��� �����Ǿ��ٸ� �������� Trace
        m_owner.FindTarget();
        if (m_owner.target)
        {
            m_enemyState.ChangeState(EEnemyState.Trace);
        }

        if (m_owner.Agent.hasPath && m_owner.Agent.remainingDistance < 0.1f)
        {
            m_enemyState.ChangeState(EEnemyState.Idle);
            return;
        }
        
        if (m_owner.Agent.remainingDistance < 0.1f)
        {
            m_owner.Move(m_owner.waypointSelector.MoveNext(0).position);
        }
    }
    private void Patrol_Exit()
    {
        m_owner.Agent.ResetPath();
    }

    private void Trace_Enter()
    {
        currentState = m_enemyState.State;
    }
    private void Trace_Update()
    {
        m_owner.FindTarget();
        if (!m_owner.target)
        {
            m_enemyState.ChangeState(EEnemyState.Idle);
            return;
        }

        Vector3 _targetPosition = m_owner.target.position;
        m_owner.Move(_targetPosition);

        if (!m_owner.target.CompareTag("Player") && !m_owner.target.CompareTag("VisionWard")) return;
        
        m_owner.targetDistance = (_targetPosition - m_owner.transform.position).sqrMagnitude;
        float _attackRange = m_owner.Data.attackRange * m_owner.Data.attackRange;
        if (_attackRange > m_owner.targetDistance)
        {
            m_enemyState.ChangeState(EEnemyState.Attack);
        }
    }

    private void Attack_Enter()
    {
        currentState = m_enemyState.State;

        m_owner.Agent.ResetPath();
        
        m_owner.transform.LookAt(m_owner.target);
        
        m_owner.StartCoroutine(nameof(m_owner.Attack));
    }
    private void Attack_Update()
    {
        m_attackTimeTaken += Time.deltaTime;
        if (m_attackTimeTaken > 1f)
        {
            m_enemyState.ChangeState(EEnemyState.Idle);
        }
    }
    private void Attack_Exit()
    {
        m_attackTimeTaken = 0f;
        m_owner.StopCoroutine(nameof(m_owner.Attack));
    }
}
