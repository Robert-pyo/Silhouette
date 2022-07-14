using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightTargetFinder : TargetFinder
{
    private Collider[] hitTargets;
    private RaycastHit m_hit;

    public SightTargetFinder(Enemy enemy)
    {
        owner = enemy;

        hitTargets = new Collider[1];
    }

    public override Transform FindTarget()
    {
        int _countTargetsInViewRadius = Physics.OverlapSphereNonAlloc(owner.transform.position, owner.viewRadius, hitTargets, owner.targetMask);

        if (_countTargetsInViewRadius > 0)
        {
            Vector3 _dirToTarget = (hitTargets[0].transform.position - owner.transform.position).normalized;

            float _targetAngle = Mathf.Acos(Vector3.Dot(owner.transform.forward, _dirToTarget)) * Mathf.Rad2Deg;

            if (_targetAngle < owner.viewAngle * 0.5f)
            {
                float _distToTarget = Vector3.Distance(owner.transform.position, hitTargets[0].transform.position);

                if (!Physics.Raycast(owner.transform.position, _dirToTarget, _distToTarget, owner.obstacleMask))
                {
                    return hitTargets[0].transform;
                }
            }
        }

        return null;
    }
}
