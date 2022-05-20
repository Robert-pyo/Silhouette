using System;
using System.Collections;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EInteractionType
{
    Item,
    PushOrPull
}

public enum EPlayerState
{
    Idle,
    Run,
    Crouch,
    PushAndPull,
    ThrowSomething,
}

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour, IWalkable
    {
        private StateMachine<EPlayerState> m_playerSM;

        private PlayerInput m_input;
        public NavMeshAgent Agent { get; private set; }

        private MoveStrategy m_mousePointWalk;

        private Animator m_playerAnim;

        [Header("Player Info"), SerializeField]
        private float moveSpeed;
        public float MoveSpeed => moveSpeed;
        [Range(0, 1)] public float walkSpeedReduction;
        [Range(0, 1)] public float crouchSpeedReduction;

        [Header("Walk Info")]
        [SerializeField] private Transform groundChecker;
        
        [HideInInspector] public bool isActing;
        [HideInInspector] public bool isReadyToThrow;

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
        private static readonly int OnJump = Animator.StringToHash("OnJump");

        private void Awake()
        {
            m_playerSM = StateMachine<EPlayerState>.Initialize(this);

            m_input = PlayerInput.Instance;
            
            Agent = GetComponent<NavMeshAgent>();
            m_playerAnim = GetComponentInChildren<Animator>();

            m_mousePointWalk = new RayPlayerWalk(this);
            rayDetection = new RayDetector(this);
            
            m_playerSM.ChangeState(EPlayerState.Idle);
        }

        private void Update()
        {
            //Interaction
            isInteractable = rayDetection.CanInteract();
            
            if (isActing) return;
            Move();

            ReadyToThrow();
        }
        
        #region States
        private void Idle_Update()
        {
            if (isInteractable && m_input.InteractionInput && interactionType == EInteractionType.PushOrPull)
            {
                m_playerAnim.SetTrigger(OnPush);
                m_playerAnim.SetBool(OnPushAction, true);
                m_playerSM.ChangeState(EPlayerState.PushAndPull);
            }

            if (m_input.CrouchInput)
            {
                m_playerAnim.SetBool(IsCrouching, true);
                m_playerSM.ChangeState(EPlayerState.Crouch);
                Debug.Log("Change to crouch");
            }
            
            if (Agent.velocity.sqrMagnitude < 0.01f) return;
            
            m_playerSM.ChangeState(EPlayerState.Run);
        }

        private void Run_Update()
        {
            m_playerAnim.SetFloat(Velocity, Agent.velocity.sqrMagnitude);

            // 걸음 속도 초기화
            if (!m_input.WalkInput)
            {
                Agent.speed = moveSpeed;
            }
            
            if (Agent.velocity.sqrMagnitude < 0.01f)
                m_playerSM.ChangeState(EPlayerState.Idle);

            if (m_input.WalkInput)
            {
                Agent.speed = moveSpeed * (1 - walkSpeedReduction);
            }

            if (!m_input.CrouchInput) return;
            
            m_playerAnim.SetBool(IsCrouching, true);
            m_playerSM.ChangeState(EPlayerState.Crouch);
        }

        private void Crouch_Enter()
        {
            Agent.speed = moveSpeed * (1 - crouchSpeedReduction);
        }

        private void Crouch_Update()
        {
            m_playerAnim.SetFloat(Velocity, Agent.velocity.sqrMagnitude);

            if (m_input.CrouchInput) return;

            m_playerSM.ChangeState(Agent.velocity.sqrMagnitude > 0.01f ? EPlayerState.Run : EPlayerState.Idle);
        }

        private void Crouch_Exit()
        {
            Agent.speed = moveSpeed;
            m_playerAnim.SetBool(IsCrouching, false);
        }

        private void PushAndPull_Enter()
        {
            Agent.ResetPath();
            isActing = true;
        }

        private void PushAndPull_Update()
        {
            PushAndPull();

            if (!m_input.InteractionInput) return;
            m_playerAnim.SetBool(OnPushAction, false);
            m_playerSM.ChangeState(EPlayerState.Idle);
        }

        private void PushAndPull_Exit()
        {
            isActing = false;
        }

        private void ThrowSomething_Update()
        {
            print("throwSomething");
            if (!isReadyToThrow) return;
            ThrowSomething();
        }

        #endregion

        
        private void Move()
        {
            if (!m_input.MouseInput) return;
            m_mousePointWalk.Move();
        }

        private void ReadyToThrow()
        {
            if (!m_input.ReadyToThrowInput) return;

            if (isReadyToThrow)
            {
                _projection.lineRenderer.enabled = false;
                isReadyToThrow = false;
                
                m_playerSM.ChangeState(EPlayerState.Idle);
                return;
            }

            _projection.lineRenderer.enabled = true;
            isReadyToThrow = true;

            m_playerSM.ChangeState(EPlayerState.ThrowSomething);
        }

        private void ThrowSomething()
        {
            var _mouseDir = mouseCursor.position - transform.position;
            _projectileDir = new Vector3(_mouseDir.x, 0f, _mouseDir.z) * _throwForce + transform.up * _throwForce;
            _projection.SimulateTrajectory(_rockPrefab, _startThrowPos.position, _projectileDir);
            
            if (!m_input.ThrowInput) return;
            isReadyToThrow = false;
            _projection.lineRenderer.enabled = false;

            var _spawned = Instantiate(_rockPrefab, _startThrowPos.position, Quaternion.identity);
            _spawned.Init(_projectileDir, false);

            m_playerSM.ChangeState(EPlayerState.Idle);
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
            var _moveDir = transform.forward * m_input.VInput;

            Agent.isStopped = true;
            Agent.ResetPath();
            Agent.isStopped = false;

            targetObj.transform.Translate(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            Agent.Move(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            m_playerAnim.SetFloat(Velocity, m_input.VInput * moveSpeed);
        }

        private void ResetInteractionStatus()
        {
            if (!targetObj && !m_interactionObstacle) return;
            
            targetObj = null;
            m_interactionObstacle = null;
        }

        // private IEnumerator CullingObjectForCameraView()
        // {
        //     var _dir = m_input.PlayerCamera.gameObject.transform.position - transform.position;

        //     if (Physics.Raycast(transform.position, _dir, out var _hit, 100f, LayerMask.GetMask("Wall")))
        //     {
        //         Renderer _renderer = _hit.transform.GetComponent<Renderer>();
        //         _renderer.material
        //     }
        // }

        // public void IndicateDestination(Vector3 target, Transform targetObject)
        // {
        //     GameObject _indicator = Instantiate(destinationFx, target, Quaternion.identity);
        //     _indicator.transform.parent = targetObject;
        //     Destroy(_indicator, 1f);
        // }
    }
}
