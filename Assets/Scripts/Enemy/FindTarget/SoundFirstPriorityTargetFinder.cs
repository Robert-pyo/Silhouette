using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFirstPriorityTargetFinder : TargetFinder
{
    private Collider[] m_colliders;
    private int m_collideCount;

    private Stack<Transform> m_targetedObjStack;
    private const byte MAX_STACK_COUNT = 20;
    private SoundWaveFx m_targetSoundFx;

    private Transform m_lastDetectedTarget;

    private float m_playerDetectedTime;

    public SoundFirstPriorityTargetFinder(Enemy enemy)
    {
        this.owner = enemy;
        m_colliders = new Collider[20];

        m_targetedObjStack = new Stack<Transform>();
    }
    
    public override Transform FindTarget()
    {
        if (m_lastDetectedTarget && m_lastDetectedTarget.CompareTag("Player"))
        {
            m_playerDetectedTime += Time.deltaTime;

            if (m_playerDetectedTime < 0.3f)
            {
                return m_lastDetectedTarget;
            }

            m_lastDetectedTarget = null;
            m_playerDetectedTime = 0;
        }

        // OverlapSphere 사용하여 구형의 감지 범위 생성
        m_collideCount = Physics.OverlapSphereNonAlloc(owner.transform.position, owner.Data.recognizeRange,
            m_colliders, LayerMask.GetMask("SoundVisualizer"));

        Transform _target = null;
        
        //Debug.Log(m_collideCount);
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
                _target = m_colliders[i].transform.root;
                break;
            }
            
            // 위에서 걸러지지 않았다면
            _target = m_colliders[i].transform;
            break;
        }

        if (!_target || _target.CompareTag("VisionWard")) return _target;
        m_targetSoundFx = _target.parent.GetComponent<SoundWaveFx>();

        // 감지된 소리가 너무 작다면
        if (m_targetSoundFx.soundVelocity < 5f)
        {
            return null;
        }

        if (_target.CompareTag("PlayerSound"))
        {
            Collider[] _results = new Collider[1];
            int _count = Physics.OverlapSphereNonAlloc(owner.transform.position, owner.Data.recognizeRange, _results, LayerMask.GetMask("Player"));

            if (_count > 0)
            {
                _target = _results[0].transform;
            }
        }

        // 중복 허용 안함
        if (_target != m_lastDetectedTarget)
        {
            if (m_targetedObjStack.Count <= MAX_STACK_COUNT)
            {
                m_targetedObjStack.Push(_target != null ? _target : m_lastDetectedTarget);
            }
        }

        // 목적지에 도착 했다면 스택에서 제거하기
        if (!owner.Agent.hasPath || owner.Agent.velocity.sqrMagnitude < 0.1f)
        {
            if (m_targetedObjStack.Count > 0)
            {
                m_targetedObjStack.Pop();
            }
        }

        m_lastDetectedTarget = _target;

        return m_targetedObjStack.Count > 0 ? m_targetedObjStack.Peek() : null;
    }
}
