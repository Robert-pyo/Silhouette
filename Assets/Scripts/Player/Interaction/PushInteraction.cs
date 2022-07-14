using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Player;

public class PushInteraction : InteractionCommand
{
    private Transform m_targetTransform;
    private Vector3 m_lookDir;

    public PushInteraction(PlayerController player, Transform target) : base(player)
    {
        m_targetTransform = target;
    }

    public override void Execute()
    {
        controller.transform.localRotation = Quaternion.LookRotation(m_targetTransform.forward);
    }

    //private Vector3 FindClosestDirVector()
    //{
    //    float[] _angles = new float[4];

    //    Vector3 _targetDir = Vector3.forward - controller.transform.forward;
    //    _angles[0] = Mathf.Atan2(_targetDir.z, _targetDir.x) * Mathf.Rad2Deg;

    //    _targetDir = Vector3.back - controller.transform.forward;
    //    _angles[1] = Mathf.Atan2(_targetDir.z, _targetDir.x) * Mathf.Rad2Deg;

    //    _targetDir = Vector3.left - controller.transform.forward;
    //    _angles[2] = Mathf.Atan2(_targetDir.z, _targetDir.x) * Mathf.Rad2Deg;

    //    _targetDir = Vector3.right - controller.transform.forward;
    //    _angles[3] = Mathf.Atan2(_targetDir.z, _targetDir.x) * Mathf.Rad2Deg;

    //    float _minAngle = _angles.Min();

    //    Vector3 _result = Vector3.zero;
    //    for (int i = 0; i < 4; ++i)
    //    {
    //        if (_minAngle != _angles[i]) continue;

    //        switch (i)
    //        {
    //            case 0:
    //                _result = Vector3.forward;
    //                break;
    //            case 1:
    //                _result = Vector3.back;
    //                break;
    //            case 2:
    //                _result = Vector3.left;
    //                break;
    //            case 3:
    //                _result = Vector3.right;
    //                break;
    //            default:
    //                break;
    //        }
    //        break;
    //    }

    //    return _result;
    //}
}
