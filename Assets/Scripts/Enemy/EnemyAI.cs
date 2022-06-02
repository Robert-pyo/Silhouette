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
    Hit,
    Dead,
}

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    private StateMachine<EEnemyState> m_enemyState;
    public EEnemyState currentState;

    private Enemy m_owner;

    [Header("Patrol Info")]
    [SerializeField] private float m_delayTimeIdleToPatrol;
    [SerializeField] private int m_waySelectNumber;
    private float m_idleTimeTaken;

    private float m_attackTimeTaken;
    
    private static readonly int OnAttack = Animator.StringToHash("OnAttack");
    
    //public UnityAction stateChangeEvent;

    private void Awake()
    {
        m_enemyState = StateMachine<EEnemyState>.Initialize(this);
        // 소유자 지정
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

        // TODO : 소리가 감지되었다면 진원지로 Trace
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
        // TODO : 소리가 감지되었다면 진원지로 Trace
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
            m_owner.Move(m_owner.waypointSelector.MoveNext(m_waySelectNumber).position);
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

        if (m_owner.target.GetComponent<IDamageable>().IsDead)
        {
            m_enemyState.ChangeState(EEnemyState.Idle);
            return;
        }

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

    private void Hit_Enter()
    {
        currentState = m_enemyState.State;
    }
    private void Hit_Exit()
    {

    }

    private void Dead_Enter()
    {
        currentState = m_enemyState.State;

        m_owner.Agent.isStopped = true;
        m_owner.Agent.ResetPath();
    }
    private void Dead_Update()
    {
        if (m_owner.EnemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            print($"{name} is Dead");
        }
    }
}
