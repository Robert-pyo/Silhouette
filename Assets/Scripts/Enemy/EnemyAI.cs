using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public enum EEnemyState
{
    Idle,
    Move,
    Attack,
}

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    private StateMachine<EEnemyState> m_enemyState;

    public EEnemyState currentState;

    private Enemy m_owner;

    private void Awake()
    {
        // 소유자 지정
        m_owner = GetComponent<Enemy>();

        m_enemyState.ChangeState(EEnemyState.Idle);
    }

    private void Idle_Update()
    {

    }
}
