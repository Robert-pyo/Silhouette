using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    public NavMeshData navMeshData;
    private NavMeshDataInstance _navMeshDataInstance = new NavMeshDataInstance();

    private void OnEnable()
    {
        _navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
        _navMeshDataInstance.owner = GameObject.FindGameObjectWithTag("Ground");
    }

    private void OnDisable()
    {
        _navMeshDataInstance.Remove();
    }
}
