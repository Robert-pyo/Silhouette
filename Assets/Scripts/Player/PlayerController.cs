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

        private CapsuleCollider m_collider;
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
        private Transform m_lastTargeted;
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
        [SerializeField] private float throwDelay;
        private float m_throwTimeTaken;
        private Vector3 m_projectileDir;

        [Space(10f)]
        public List<SoundGroup> soundGroupList;
        public SoundDistributor soundDistributor;

        public UnityAction interactionPopUpEvent;
        public UnityAction popUpReleaseEvent;
        public UnityAction onDeadEvent;
        
        public UnityAction<Wire> addWireToWardEvent;

        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
        private static readonly int OnPushAction = Animator.StringToHash("OnPushAction");
        private static readonly int OnPush = Animator.StringToHash("OnPush");
        private static readonly int OnJump = Animator.StringToHash("OnJump");
        private static readonly int OnDead = Animator.StringToHash("OnDead");
        private static readonly int OnActivateWard = Animator.StringToHash("OnActivateWard");

        private void Awake()
        {
            m_playerSM = StateMachine<EPlayerState>.Initialize(this);

            m_input = PlayerInput.Instance;

            m_curHp = (short)m_maxHp;
            
            //Agent = GetComponent<NavMeshAgent>();
            m_playerAnim = GetComponentInChildren<Animator>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_collider = GetComponent<CapsuleCollider>();

            //m_movement = new RayPlayerWalk(this);
            m_movement = new RigidbodyMovement(this);
            rayDetection = new RayDetector(this);

            // Command
            m_activateVisionWard = new VisionWardInteraction(this);
            
            // Sounds
            soundDistributor.soundGroupNames = new string[soundGroupList.Count];
            for (int i = 0; i < soundGroupList.Count; ++i)
            {
                soundDistributor.soundGroupNames[i] = soundGroupList[i].groupName;
            }
            
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
            m_collider.center = new Vector3(0f, 0.6f, 0f);
            m_collider.height = 1.2f;
        }

        private void Crouch_Update()
        {
            m_playerAnim.SetFloat(Velocity, m_rigidbody.velocity.sqrMagnitude);

            if (Physics.Raycast(transform.position, Vector3.up, 2f, 1 << LayerMask.NameToLayer("Wall")))
            {
                return;
            }
            
            // 숙이기 입력이 아직 true라면 계속 상태 유지
            if (m_input.CrouchInput) return;

            m_playerSM.ChangeState(m_rigidbody.velocity.sqrMagnitude > 0.01f ? EPlayerState.Run : EPlayerState.Idle);
        }

        private void Crouch_Exit()
        {
            //Agent.speed = moveSpeed;
            m_playerAnim.SetBool(IsCrouching, false);
            m_collider.center = new Vector3(0f, 0.95f, 0f);
            m_collider.height = 1.85f;
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
                return;
            }
            
            PushAndPull();
            m_lastTargeted = targetObj.transform;

            if (!m_input.InteractionInput) return;
            m_playerAnim.SetBool(OnPushAction, false);
            m_playerSM.ChangeState(EPlayerState.Idle);
        }

        private void PushAndPull_Exit()
        {
            isActing = false;
            print("Exit Push and Pull");
            m_lastTargeted.parent = null;
        }

        private void ThrowSomething_Update()
        {
            if (!isReadyToThrow) return;
            ThrowSomething();
        }

        private void OnActivateWard_Enter()
        {
            if (!targetObj.GetComponent<VibrationGenerator>().IsDead)
            {
                m_playerSM.ChangeState(EPlayerState.Idle);
                return;
            }

            isActing = true;
            
            m_playerAnim.SetTrigger(OnActivateWard);

            targetObj.GetComponent<VibrationGenerator>().StateToggle();
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

        private void Die_Enter()
        {
            // 죽을 때 처리
            m_input.playerControllerInputBlocked = true;
            soundDistributor.SoundPlayer(soundGroupList,"Dying", 0);
        }

        #endregion

        
        private void Move()
        {
            m_movement.Move();
        }

        private void ReadyToThrow()
        {
            if (m_throwTimeTaken < throwDelay)
            {
                m_throwTimeTaken += Time.deltaTime;

                isReadyToThrow = false;
                projection.lineRenderer.enabled = false;
                return;
            }

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
            
            m_playerAnim.SetFloat(Velocity, m_rigidbody.velocity.sqrMagnitude);
            
            if (!m_input.ThrowInput) return;
            isReadyToThrow = false;
            projection.lineRenderer.enabled = false;

            var _spawned = Instantiate(rockPrefab, startThrowPos.position, Quaternion.identity);
            _spawned.Init(m_projectileDir, false);

            m_playerSM.ChangeState(EPlayerState.Idle);
            m_throwTimeTaken = 0f;
        }
        
        public void BlockInputToggle()
        {
            isActing = !isActing;
            //Agent.isStopped = !Agent.isStopped;
        }

        private int m_stepCount = 0;
        public void GenerateWalkSoundWave()
        {
            // 걸을 때 음파 생성
            if (Physics.Raycast(groundChecker.position, Vector3.down, out var _hit, 2f, LayerMask.GetMask("Ground")))
            {
                GameObject _obj = SoundWaveManager.Instance.GenerateSoundWave(
                    _hit.transform, _hit.point, Vector3.zero, moveSpeed);

                if (!_obj) return;
                _obj.transform.GetChild(0).tag = "PlayerSound";
                
                float _volume = moveSpeed / SoundWaveManager.Instance.maxPower;

                SoundGroup _group = soundGroupList.Find(group => group.groupName == "Footstep");
                soundDistributor.SoundPlayer(soundGroupList, "Footstep", m_stepCount, _volume);
                m_stepCount = (m_stepCount + 1) % _group.audioClipList.Count;
            }
        }

        private void PushAndPull()
        {
            var _moveDir = Vector3.forward * m_input.VInput * (moveSpeed * walkSpeedReduction);

            //targetObj.transform.Translate(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));
            //Agent.Move(_moveDir * (moveSpeed * walkSpeedReduction * Time.deltaTime));

            transform.Translate(_moveDir * Time.deltaTime);
            targetObj.transform.parent = transform;

            m_playerAnim.SetFloat(Velocity, moveSpeed * m_input.VInput);
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

            if (m_curHp <= 0)
            {
                isDead = true;
                m_playerAnim.SetBool(OnDead, true);
                Die();
            }
        }

        public void Die()
        {
            m_playerSM.ChangeState(EPlayerState.Die);
            m_collider.isTrigger = true;
            m_rigidbody.isKinematic = true;
            enabled = false;
            onDeadEvent?.Invoke();
        }
    }
}
