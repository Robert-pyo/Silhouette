using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointSelector : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPatrolWay
    {
        public List<Transform> waypoints;

        public int currentIndex = -1;
    }

    public List<EnemyPatrolWay> ways = new List<EnemyPatrolWay>();

    public Transform MoveNext(int selectWayNumber)
    {
        if (!PathVerifier()) return null;

        ways[selectWayNumber].currentIndex = (ways[selectWayNumber].currentIndex + 1) % ways[selectWayNumber].waypoints.Count;

        return ways[selectWayNumber].waypoints[ways[selectWayNumber].currentIndex];
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

        ways[selectWayNumber].currentIndex = index;
        
        return ways[selectWayNumber].waypoints[ways[selectWayNumber].currentIndex];
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
