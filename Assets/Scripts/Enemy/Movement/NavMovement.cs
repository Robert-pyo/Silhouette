using UnityEngine;
using UnityEngine.AI;

public class NavMovement : EnemyMovement
{
    private NavMeshAgent m_agent;

    public NavMovement(NavMeshAgent agent, float moveSpeed, float rotateSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rotateSpeed = rotateSpeed;
        m_agent = agent;
    }

    public override void Execute(Vector3 dest)
    {
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
}
