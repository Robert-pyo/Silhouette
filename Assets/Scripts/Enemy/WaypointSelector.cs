using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointSelector : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPatrolWay
    {
        public List<Transform> waypoints;
    }

    public List<EnemyPatrolWay> ways = new List<EnemyPatrolWay>();
    
    private int m_index = -1;
    public int Index => m_index;

    public Transform MoveNext(int selectWayNumber)
    {
        if (!PathVerifier()) return null;

        m_index = (m_index + 1) % ways[selectWayNumber].waypoints.Count;

        return ways[selectWayNumber].waypoints[m_index];
    }

    // public Transform MoveNext()
    // {
    //     if (!PathVerifier()) return null;
    //     
    //     if (m_index == waypoints.Count - 1)
    //     {
    //         // 이미 마지막 인덱스라면
    //         return null;
    //     }
    //
    //     m_index++;
    //     return waypoints[m_index];
    // }

    public Transform MoveTo(int selectWayNumber, int index)
    {
        if (!PathVerifier()) return null;

        if (index >= ways[selectWayNumber].waypoints.Count || index < 0)
        {
            //Debug.LogError("이동하려는 인덱스가 존재하지 않습니다.");
            return null;
        }

        m_index = index;
        
        return ways[selectWayNumber].waypoints[m_index];
    }

    private bool PathVerifier()
    {
        if (ways.Count == 0)
        {
            //Debug.LogError("등록된 Waypoint가 존재하지 않습니다.");
            return false;
        }

        return true;
    }
}
