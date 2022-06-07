using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class RigidbodyMovement : MoveStrategy
{
    private PlayerInput m_input;

    public RigidbodyMovement(PlayerController player)
    {
        controller = player;

        m_input = PlayerInput.Instance;
    }

    public override void Move()
    {
        var _moveVec = m_input.CamDirection.right * m_input.HInput + m_input.CamDirection.forward * m_input.VInput;

        if (_moveVec.sqrMagnitude > 1)
        {
            _moveVec.Normalize();
        }

        controller.PlayerRigidbody.velocity = new Vector3(
            _moveVec.x * controller.MoveSpeed, controller.PlayerRigidbody.velocity.y, _moveVec.z * controller.MoveSpeed);

        if (_moveVec == Vector3.zero) return;

        controller.transform.rotation = Quaternion.Slerp(
            controller.transform.rotation, Quaternion.LookRotation(_moveVec), 30 * Time.deltaTime);
    }

    private void RotateToMousePoint()
    {
        if (!m_input.MouseInput) return;

        RaycastHit _mousePoint = m_input.MouseHit;
        var _lookAtTarget = new Vector3(_mousePoint.point.x, controller.transform.position.y, _mousePoint.point.z);

        controller.transform.LookAt(_lookAtTarget);
    }
}
