using UnityEngine;

namespace Player
{
    public class RayPlayerWalk : MoveStrategy
    {
        private readonly PlayerController m_controller;

        private const float PLAYER_ROTATION_SPEED = 20f;

        public RayPlayerWalk(PlayerController controller)
        {
            m_controller = controller;
            moveSpeed = controller.MoveSpeed;
            m_controller.Agent.speed = moveSpeed;
        }

        public override void Move()
        {
            Walk();
        }

        private void Walk()
        {
            if (m_controller.isActing) return;

            var _ray = m_controller.playerCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, hitInfo: out var _hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                m_controller.Agent.velocity = Vector3.zero;
                m_controller.Agent.SetDestination(_hit.point);
                m_controller.Agent.velocity = m_controller.Agent.desiredVelocity;
            
                var _lookRotation = m_controller.Agent.steeringTarget - m_controller.transform.position;
                _lookRotation = new Vector3(_lookRotation.x, 0f, _lookRotation.z);

                if (_lookRotation == Vector3.zero) return;

                m_controller.transform.rotation = Quaternion.Slerp(
                    m_controller.transform.rotation,Quaternion.LookRotation(_lookRotation), PLAYER_ROTATION_SPEED * Time.deltaTime);
            
                // 클릭 지점에 목표 지점 가리키는 FX 생성
                //m_controller.IndicateDestination(_hit.point, _hit.transform);
            
                //pointer.transform.position = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
            }
        }
    }
}
