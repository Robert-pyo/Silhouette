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
        
        // TODO : 소리가 감지되었다면 진원지로 Trace
    }

    private void Patrol_Enter()
    {
        currentState = m_enemyState.State;
    }
    private void Patrol_Update()
    {
        // TODO : 소리가 감지되었다면 진원지로 Trace
        
        if (m_owner.Agent.hasPath && m_owner.Agent.remainingDistance < 0.1f)
        {
            m_enemyState.ChangeState(EEnemyState.Idle);
            return;
        }
        
        if (m_owner.Agent.remainingDistance < 0.1f)
        {
            m_owner.Move(m_owner.waypointSelector.MoveNext().position);
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
        
    }
}
