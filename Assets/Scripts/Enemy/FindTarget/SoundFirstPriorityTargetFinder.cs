using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFirstPriorityTargetFinder : TargetFinder
{
    private Enemy m_enemy;

    private Collider[] m_colliders;
    private int m_collideCount;

    public SoundFirstPriorityTargetFinder(Enemy enemy)
    {
        m_enemy = enemy;
        m_colliders = new Collider[10];
    }
    
    public override Transform FindTarget()
    {
        // OverlapSphere 사용하여 구형의 감지 범위 생성
        m_collideCount = Physics.OverlapSphereNonAlloc(m_enemy.transform.position, m_enemy.Data.recognizeRange,
            m_colliders, LayerMask.GetMask("SoundVisualizer"));

        Transform _target = null;

        for (int i = 0; i < m_collideCount; ++i)
        {
            // OverlapSphere에 걸린 Collider 중에 다른 Enemy가 발생시킨 sound가 있다면 제외
            if (m_colliders[i].CompareTag("EnemySound")) continue;

            // 우선적으로 PlayerSound 검출
            if (m_colliders[i].CompareTag("PlayerSound"))
            {
                _target = m_colliders[i].transform;
                break;
            }

            // 두번째로 VisionSound 검출
            if (m_colliders[i].CompareTag("VisionSound"))
            {
                _target = m_colliders[i].transform;
                break;
            }
            
            // 위에서 걸러지지 않았다면
            _target = m_colliders[i].transform;
            break;
        }

        if (_target && _target.CompareTag("PlayerSound"))
        {
            Collider[] _results = new Collider[1];
            int _count = Physics.OverlapSphereNonAlloc(m_enemy.transform.position, m_enemy.Data.recognizeRange, _results, LayerMask.GetMask("Player"));

            if (_count > 0)
            {
                return _results[0].transform;
            }
        }

        return _target;
    }
}
