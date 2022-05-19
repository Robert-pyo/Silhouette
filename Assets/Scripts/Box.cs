using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Box : Obstacles
{
    [SerializeField] private NavMeshLink m_navMeshLink;
    private NavMeshSourceTag m_navMeshSourceTag;

    private void Awake()
    {
        obstacleType = EObstacleType.Climbable;
        m_navMeshSourceTag = GetComponent<NavMeshSourceTag>();
    }
}
