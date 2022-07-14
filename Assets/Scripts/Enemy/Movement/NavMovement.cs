using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMovement : EnemyMovement
{
    private NavMeshAgent m_agent;
    private bool m_isStopped = false;

    // Properties
    public bool IsStopped => m_isStopped;

    public NavMovement(Enemy enemy, NavMeshAgent agent, float moveSpeed, float rotateSpeed)
    {
        owner = enemy;
        this.moveSpeed = moveSpeed;
        this.rotateSpeed = rotateSpeed;
        m_agent = agent;

        owner.OnBeCarefulEvent += StopMovement;
    }

    public override void Execute(Vector3 dest)
    {
        if (m_isStopped) return;

        NavAgentMove(dest);
    }

    private void NavAgentMove(Vector3 dest)
    {
        m_agent.SetDestination(dest);
        m_agent.speed = moveSpeed;
    }
    
    public override void Rotate()
    {
        var _lookRotation = m_agent.steeringTarget - m_agent.transform.position;
        _lookRotation = new Vector3(_lookRotation.x, m_agent.transform.rotation.y, _lookRotation.z);

        m_agent.transform.rotation = Quaternion.Slerp(
            m_agent.transform.rotation, Quaternion.LookRotation(_lookRotation), rotateSpeed * Time.deltaTime);
    }

    private void StopMovement(bool condition)
    {
        m_isStopped = condition;
        m_agent.isStopped = condition;
    }
}
