using System;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EInteractionType
{
    Item,
    PushOrPull,
    VisionWard,
}

public enum EPlayerState
{
    Idle,
    Run,
    Crouch,
    PushAndPull,
    ThrowSomething,
    OnActivateWard,
    Hit,
    Die,
}

namespace Player
{
    public class PlayerController : MonoBehaviour, IWalkable, IDamageable
    {
        private StateMachine<EPlayerState> m_playerSM;

        private PlayerInput m_input;
        public NavMeshAgent Agent { get; private set; }

        private MoveStrategy m_movement;

        private Animator m_playerAnim;

        private Rigidbody m_rigidbody;
        public Rigidbody PlayerRigidbody => m_rigidbody;

        [Header("Player Info"), SerializeField]
        private float moveSpeed;
        [SerializeField] private float maxMoveSpeed = 10f;
        public float MoveSpeed => moveSpeed;

        [SerializeField] private ushort m_maxHp;
        [SerializeField] private short m_curHp;
        public ushort MaxHp => m_maxHp;
        public short CurHp => m_curHp;
        public bool IsDead => isDead;

        [Range(0, 1)] public float walkSpeedReduction;
        [Range(0, 1)] public float crouchSpeedReduction;

        [Header("Walk Info")]
        [SerializeField] private Transform groundChecker;
        
        [HideInInspector] public bool isActing;
        [HideInInspector] public bool isReadyToThrow;
        [HideInInspector] public bool isDead;

        [Header("Interaction Info")]
        public EInteractionType interactionType;
        public bool isInteractable;
        public GameObject targetObj;
        public Transform detectOrigin;
        public float detectDistance;
        private Obstacles m_interactionObstacle;
        private InteractDetectStrategy rayDetection;

        [Header("For Link To Vision Ward")]
        public Wire wire;
        public List<Wire> wireInstances = new List<Wire>();
        public Transform wireTiedPosition;
        private InteractionCommand m_activateVisionWard;

        [Header("Projection")]
        public Transform mouseCursor;
        [SerializeField] private Projection projection;
        [SerializeField] private Rock rockPrefab;
        [SerializeField] private float throwForce;
        [SerializeField] private Transform startThrowPos;
        private Vector3 m_projectileDir;

        public UnityAction interactionPopUpEvent;
        public UnityAction popUpReleaseEvent;
        
        public UnityAction<Wire> addWireToWardEvent;

        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
        private static readonly int OnPushAction = Animator.StringToHash("OnPushAction");
        private static readonly int OnPush = Animator.StringToHash("OnPush");
        private static readonly int OnJump = Animator.StringToHash("OnJump");
        private static readonly int OnDead = Animator.StringToHash("OnDead");

        private void Awake()
        {
            m_playerSM = StateMachine<EPlayerState>.Initialize(this);

            m_input = PlayerInput.Instance;

            m_curHp = (short)m_maxHp;
            
            //Agent = GetComponent<NavMeshAgent>();
            m_playerAnim = GetComponentInChildren<Animator>();
            m_rigidbody = GetComponent<Rigidbody>();

            //m_movement = new RayPlayerWalk(this);
            m_movement = new RigidbodyMovement(this);
            rayDetection = new RayDetector(this);

            // Command
            m_activateVisionWard = new VisionWardInteraction(this);
            
            m_playerSM.ChangeState(EPlayerState.Idle);
        }

        private void Update()
        {
            if (isDead) return;
            //Interaction
            isInteractable = rayDetection.CanInteract();
            
            if (isActing) return;
            Move();

            ReadyToThrow();
        }

        private void LateUpdate()
        {
            if (isInteractable)
            {
                interactionPopUpEvent?.Invoke();
            }
            else
            {
                popUpReleaseEvent?.Invoke();
            }
        }

        #region States
        private void Idle_Update()
        {
            if (isInteractable && m_input.InteractionInput)
            {
                switch (interactionType)
                {
                    case EInteractionType.PushOrPull:
                        {
                            m_playerAnim.SetTrigger(OnPush);
                            m_playerAnim.SetBool(OnPushAction, true);
                            m_playerSM.ChangeState(EPlayerState.PushAndPull);
                        }
                        break;

                    case EInteractionType.VisionWard:
                        {
                            // TODO : 애니메이션 추가 필요
                            m_playerSM.ChangeState(EPlayerState.OnActivateWard);
                            print("Vision");
                        }
                        break;

                    default:
                        Debug.LogError("현재 상호작용에 맞는 행동이 존재하지 않습니다.");
                        break;
                }
            }

            if (m_input.CrouchInput)
            {
                m_playerAnim.SetBool(IsCrouching, true);
                m_playerSM.ChangeState(EPlayerState.Crouch);
            }

            //if (Agent.velocity.sqrMagnitude < 0.01f) return;
            if (m_rigidbody.velocity.sqrMagnitude < 0.01f) return;
            
            m_playerSM.ChangeState(EPlayerState.Run);
        }

        private void Run_Update()
        {
            m_playerAnim.SetFloat(Velocity, m_rigidbody.velocity.sqrMagnitude);

            // 걸음 속도 초기화
            if (!m_input.WalkInput)
            {
                moveSpeed = maxMoveSpeed;
            }
            
            if (m_rigidbody.velocity.sqrMagnitude < 0.01f)
                m_playerSM.ChangeState(EPlayerState.Idle);

            if (m_input.WalkInput)
            {
                //Agent.speed = moveSpeed * (1 - walkSpeedReduction);
                moveSpeed = maxMoveSpeed * (1 - walkSpeedReduction);
            }

            if (!m_input.CrouchInput) return;
            
            m_playerAnim.SetBool(IsCrouching, true);
            m_playerSM.ChangeState(EPlayerState.Crouch);
        }

        private void Crouch_Enter()
        {
            moveSpeed = maxMoveSpeed * (1 - crouchSpeedReduction);
        }

        private void Crouch_Update()
        {
            m_playerAnim.SetFloat(Velocity, m_rigidbody.velocity.sqrMagnitude);

            if (m_input.CrouchInput) return;

            m_playerSM.ChangeState(m_rigidbody.velocity.sqrMagnitude > 0.01f ? EPlayerState.Run : EPlayerState.Idle);
        }

        private void Crouch_Exit()
        {
            //Agent.speed = moveSpeed;
            m_playerAnim.SetBool(IsCrouching, false);
        }

        private void PushAndPull_Enter()
        {
            //Agent.isStopped = true;
            //Agent.ResetPath();
            isActing = true;
        }

        private void PushAndPull_Update()
        {
            if (!isInteractable)
            {
                m_playerAnim.SetBool(OnPushAction, false);
                m_playerSM.ChangeState(EPlayerState.Idle);
            }
            
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
            if (!isReadyToThrow) return;
            ThrowSomething();
        }

        private void OnActivateWard_Enter()
        {
            isActing = true;

            targetObj.GetComponent<VibrationGenerator>().generatorEnableEvent?.Invoke();
            m_activateVisionWard.Execute();
        }

        private void OnActivateWard_Update()
        {
            if (m_playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                m_playerSM.ChangeState(EPlayerState.Idle);
            }
        }

        private void OnActivateWard_Exit()
        {
            isActing = false;
        }

        private void Hit_Enter()
        {
            // 맞았을 때 처리
            m_input.playerControllerInputBlocked = true;
        }

        private void Hit_Exit()
        {
            m_input.playerControllerInputBlocked = false;
        }

        private void Die_Enter()
        {
            // 죽을 때 처리
            m_input.playerControllerInputBlocked = true;
        }

        #endregion

        
        private void Move()
        {
            m_movement.Move();
        }

        private void ReadyToThrow()
        {
            if (!m_input.ReadyToThrowInput) return;

            if (isReadyToThrow)
            {
                projection.lineRenderer.enabled = false;
                isReadyToThrow = false;
                
                m_playerSM.ChangeState(EPlayerState.Idle);
                return;
            }

            projection.lineRenderer.enabled = true;
            isReadyToThrow = true;

            m_playerSM.ChangeState(EPlayerState.ThrowSomething);
        }

        private void ThrowSomething()
        {
            var _mouseDir = mouseCursor.position - transform.position;
            m_projectileDir = new Vector3(_mouseDir.x, 0f, _mouseDir.z) * throwForce + transform.up * throwForce;
            projection.SimulateTrajectory(rockPrefab, startThrowPos.position, m_projectileDir);
            
            if (!m_input.ThrowInput) return;
            isReadyToThrow = false;
            projection.lineRenderer.enabled = false;

            var _spawned = Instantiate(rockPrefab, startThrowPos.position, Quaternion.identity);
            _spawned.Init(m_projectileDir, false);

            m_playerSM.ChangeState(EPlayerState.Idle);
        }
        
        public void BlockInputToggle()
        {
            isActing = !isActing;
            //Agent.isStopped = !Agent.isStopped;
        }

        public void GenerateWalkSoundWave()
        {
            // 걸을 때 음파 생성
            if (Physics.Raycast(groundChecker.position, Vector3.down, out var _hit, float.MaxValue, LayerMask.GetMask("Ground")))
            {
                GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
                    _hit.transform, _hit.point, Vector3.zero, moveSpeed);

                _obj.transform.GetChild(0).tag = "PlayerSound";
            }
        }

        private void PushAndPull()
        {
            var _moveDir = transform.forward * m_input.VInput;

            targetObj.transform.Translate(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            //Agent.Move(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            transform.Translate(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            m_playerAnim.SetFloat(Velocity, m_input.VInput * moveSpeed);
        }

        private void ResetInteractionStatus()
        {
            if (!targetObj && !m_interactionObstacle) return;
            
            targetObj = null;
            m_interactionObstacle = null;
        }

        public Wire EnableWire()
        {
            Wire _obj = Instantiate(wire, Vector3.zero, Quaternion.identity);
            wireInstances.Add(_obj);
            addWireToWardEvent?.Invoke(_obj);
            return _obj;
        }
        
        public void Hit(ushort damage)
        {
            m_curHp -= (short)damage;

            if (m_curHp == 0)
            {
                isDead = true;
                Die();
            }
        }

        public void Die()
        {
            m_playerAnim.SetBool(OnDead, true);
            // 현재 OnDead 애니메이션이 종료되었다면
            if (m_playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                // TODO : 게임 오버 처리
                print("게임 오버");
            }
        }
    }
}
