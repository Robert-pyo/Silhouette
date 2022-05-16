using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public NavMeshAgent Agent { get; private set; }
        [Space(10f)]
        public Camera playerCam;
        //public GameObject destinationFx;

        private MoveStrategy m_mousePointWalk;

        private Animator m_playerAnim;

        [Header("Player Info"), SerializeField]
        private float moveSpeed;
        public float MoveSpeed => moveSpeed;
        [Range(0, 1)] public float walkSpeedReduction;
        [Range(0, 1)] public float crouchSpeedReduction;

        public bool isWalking;
        public bool isCrouching;

        [HideInInspector] public bool isActing;

        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            m_playerAnim = GetComponentInChildren<Animator>();

            m_mousePointWalk = new RayPlayerWalk(this);
        }

        private void Update()
        {
            // 기타 움직임 입력
            InputManager.Instance.GetPlayerInput();
            
            Move();
        }

        private void Move()
        {
            if (!isWalking && !isCrouching)
            {
                Agent.speed = moveSpeed;
            }
            
            Walk();
            Crouch();
            
            m_playerAnim.SetFloat(Velocity, Agent.velocity.sqrMagnitude);
            m_playerAnim.SetBool(IsCrouching, isCrouching);

            if (!Input.GetButtonDown("Fire2")) return;
            m_mousePointWalk.Move();
        }

        private void Walk()
        {
            if (!isWalking) return;

            Agent.speed = moveSpeed * (1 - walkSpeedReduction);
        }

        private void Crouch()
        {
            if (!isCrouching) return;

            Agent.speed = moveSpeed * (1 - crouchSpeedReduction);
        }
        
        public void BlockInputToggle()
        {
            isActing = !isActing;
            Agent.isStopped = !Agent.isStopped;
        }

        // public void IndicateDestination(Vector3 target, Transform targetObject)
        // {
        //     GameObject _indicator = Instantiate(destinationFx, target, Quaternion.identity);
        //     _indicator.transform.parent = targetObject;
        //     Destroy(_indicator, 1f);
        // }
    }
}
