using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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

        [Header("Walk Info")]
        [SerializeField] private Transform rightFoot;
        [SerializeField] private Transform leftFoot;

        [HideInInspector] public bool isWalking;
        [HideInInspector] public bool isCrouching;
        [HideInInspector] public bool isThrowingReady;
        [HideInInspector] public bool isThrowingSomething;

        [HideInInspector] public bool isActing;

        public Transform mouseCursor;
        [SerializeField] private Projection _projection;
        [SerializeField] private Rock _rockPrefab;
        [SerializeField] private float _throwForce;
        [SerializeField] private Transform _startThrowPos;
        private Vector3 _projectileDir;

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
            
            ThrowSomething();
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

        private void ThrowSomething()
        {
            if (isThrowingReady)
            {
                if (!_projection.lineRenderer.enabled)
                    _projection.lineRenderer.enabled = true;

                var _mouseDir = mouseCursor.position - transform.position;
                _projectileDir = new Vector3(_mouseDir.x, 0f, _mouseDir.z) * _throwForce + transform.up * _throwForce;
                _projection.SimulateTrajectory(_rockPrefab, _startThrowPos.position, _projectileDir);
                return;
            }
            
            _projection.lineRenderer.enabled = false;

            if (!isThrowingSomething) return;
            _projection.lineRenderer.enabled = false;

            var _spawned = Instantiate(_rockPrefab, _startThrowPos.position, Quaternion.identity);
            _spawned.Init(_projectileDir, false);
            isThrowingSomething = false;
        }
        
        public void BlockInputToggle()
        {
            isActing = !isActing;
            Agent.isStopped = !Agent.isStopped;
        }

        public void GenerateWalkFx()
        {
            // 걸을 때 음파 생성
            print(Agent.navMeshOwner);
            //SoundWaveManager.Instance.GenerateSoundWave();
        }

        // public void IndicateDestination(Vector3 target, Transform targetObject)
        // {
        //     GameObject _indicator = Instantiate(destinationFx, target, Quaternion.identity);
        //     _indicator.transform.parent = targetObject;
        //     Destroy(_indicator, 1f);
        // }
    }
}
