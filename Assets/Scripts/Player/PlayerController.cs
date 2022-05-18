using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EInteractionType
{
    Item,
    PushOrPull
}

namespace Player
{   
    public class PlayerController : MonoBehaviour, IWalkable
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
        [SerializeField] private Transform groundChecker;

        [HideInInspector] public bool isWalking;
        [HideInInspector] public bool isCrouching;
        [HideInInspector] public bool isThrowingReady;
        [HideInInspector] public bool isThrowingSomething;

        [HideInInspector] public bool isActing;

        [Header("Interaction Info")]
        public EInteractionType interactionType;
        public bool isInteractable;
        public GameObject targetObj;
        public Transform detectOrigin;
        public float detectDistance;
        private Obstacles m_interactionObstacle;
        private InteractDetectStrategy rayDetection;

        [Header("Projection")]
        public Transform mouseCursor;
        [SerializeField] private Projection _projection;
        [SerializeField] private Rock _rockPrefab;
        [SerializeField] private float _throwForce;
        [SerializeField] private Transform _startThrowPos;
        private Vector3 _projectileDir;

        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
        private static readonly int OnPushAction = Animator.StringToHash("OnPushAction");
        private static readonly int OnPush = Animator.StringToHash("OnPush");

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            m_playerAnim = GetComponentInChildren<Animator>();

            m_mousePointWalk = new RayPlayerWalk(this);
            rayDetection = new RayDetector(this);
        }

        private void Update()
        {
            // 기타 움직임 입력
            InputManager.Instance.GetPlayerInput();
            
            Move();
            
            ThrowSomething();
            
            //Interaction
            Climb();

            isInteractable = rayDetection.CanInteract();
            if (isInteractable && Input.GetKeyDown(KeyCode.E))
            {
                isActing = !isActing;
            }
            PushAndPull();
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

        public void GenerateWalkSoundWave()
        {
            // 걸을 때 음파 생성
            if (Physics.Raycast(groundChecker.position, Vector3.down, out var _hit, float.MaxValue, LayerMask.GetMask("Ground")))
            {
                SoundWaveManager.Instance.GenerateSoundWave(
                    _hit.transform, _hit.point, Vector3.zero, Agent.speed);
            }
        }

        private void PushAndPull()
        {
            if (!isInteractable)
            {
                ResetInteractionStatus();
                return;
            }
            if (interactionType != EInteractionType.PushOrPull) return;
            if (!isActing)
            {
                m_playerAnim.SetBool(OnPushAction, isActing);
                return;
            }

            if (!m_playerAnim.GetBool(OnPushAction))
            {
                m_playerAnim.SetTrigger(OnPush);
                m_playerAnim.SetBool(OnPushAction, isActing);
            }

            var _vInput = Input.GetAxis("Vertical");
            var _moveDir = transform.forward * _vInput;

            Agent.isStopped = true;
            Agent.ResetPath();
            Agent.isStopped = false;

            targetObj.transform.Translate(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            Agent.Move(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            m_playerAnim.SetFloat(Velocity, _vInput * moveSpeed);
        }

        private void Climb()
        {
            if (!isInteractable)
            {
                ResetInteractionStatus();
                return;
            }
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            m_interactionObstacle = targetObj.GetComponent<Obstacles>();
            if (m_interactionObstacle.obstacleType != EObstacleType.Climbable) return;
            
            // 목적지 초기화
            Agent.ResetPath();

            // Test
            Transform _targetWay = m_interactionObstacle.fourWayToClimb[0];
            float _closestDist = float.MaxValue;
            foreach (Transform _way in m_interactionObstacle.fourWayToClimb)
            {
                var _sqrDist = (_way.position - transform.position).sqrMagnitude;
                if (!(_closestDist > _sqrDist)) continue;
                _closestDist = _sqrDist;
                _targetWay = _way;
            }
            
            //Debug.Log(_targetWay.position - transform.position);
            Agent.Move(_targetWay.position - transform.position);
        }

        private void ResetInteractionStatus()
        {
            if (!targetObj && !m_interactionObstacle) return;
            
            targetObj = null;
            m_interactionObstacle = null;
        }

        // public void IndicateDestination(Vector3 target, Transform targetObject)
        // {
        //     GameObject _indicator = Instantiate(destinationFx, target, Quaternion.identity);
        //     _indicator.transform.parent = targetObject;
        //     Destroy(_indicator, 1f);
        // }
    }
}
