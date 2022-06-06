using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct tPlayerCommand
{
    public KeyCode playerWalk;
    public KeyCode playerCrouch;
    public KeyCode playerInteraction;
    public KeyCode playerThrowReady;
    public KeyCode playerThrowSomething;
    public KeyCode pause;
}

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return s_instance; }
    }
    private static PlayerInput s_instance;

    [HideInInspector] public bool playerControllerInputBlocked;

    [SerializeField]
    // 마우스 입력
    private Camera m_camera;
    private Transform m_camDirection;
    private RaycastHit m_mouseHit;
    private bool m_isMouseClickedGround;

    public tPlayerCommand m_command;
    
    // 플레이어 움직임 관련 입력
    private bool m_walk;
    private bool m_crouch;
    private bool m_throwReady;
    private bool m_throwSomething;

    // 상호작용 입력
    private bool m_bInteraction;
    private float m_hInput;
    private float m_vInput;
    
    // 게임 멈추기
    private bool m_bPauseToggle;
    public UnityAction onPauseEvent;

    // getter / setter
    public Camera PlayerCamera => m_camera;
    public Transform CamDirection => m_camDirection;

    public RaycastHit MouseHit
    {
        get
        {
            if (playerControllerInputBlocked)
                return new RaycastHit();

            return m_mouseHit;
        }
    }

    public bool MouseInput
    {
        get
        {
            return m_isMouseClickedGround && !playerControllerInputBlocked;
        }
    }

    public bool WalkInput
    {
        get
        {
            return m_walk && !playerControllerInputBlocked;
        }
    }
    
    public bool CrouchInput
    {
        get
        {
            return m_crouch && !playerControllerInputBlocked;
        }
    }
    
    public bool ReadyToThrowInput
    {
        get
        {
            return m_throwReady && !playerControllerInputBlocked;
        }
    }
    
    public bool ThrowInput
    {
        get
        {
            return m_throwSomething && !playerControllerInputBlocked;
        }
    }

    public bool InteractionInput
    {
        get { return m_bInteraction && !playerControllerInputBlocked; }
    }

    public float VInput
    {
        get
        {
            if (playerControllerInputBlocked)
                return 0;
            
            return m_vInput;
        }
    }

    public float HInput
    {
        get
        {
            if (playerControllerInputBlocked)
                return 0;

            return m_hInput;
        }
    }

    public bool IsPaused => m_bPauseToggle;


    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Debug.Log("플레이어 Input은 오직 하나만 존재할 수 있습니다.");
            Destroy(gameObject);
        }
        
        // reset
        playerControllerInputBlocked = false;

        // 마우스 입력을 감지할 카메라 설정
        if (m_camera == null)
        {
            m_camera = Camera.main;
        }

        m_camDirection = GameObject.FindGameObjectWithTag("CameraDirection").transform;
    }

    private void Update()
    {
        CalcMouseHit();

        if (Input.GetKeyDown(m_command.playerWalk))
        {
            m_walk = !m_walk;
        }
        if (Input.GetKeyDown(m_command.playerCrouch))
        {
            m_crouch = !m_crouch;
        }

        m_throwReady = Input.GetKeyDown(m_command.playerThrowReady);
        
        m_throwSomething = Input.GetKeyDown(m_command.playerThrowSomething);
        if (m_throwReady && m_throwSomething) m_throwReady = false;
        
        m_bInteraction = Input.GetKeyDown(m_command.playerInteraction);

        m_hInput = Input.GetAxis("Horizontal");
        m_vInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(m_command.pause))
        {
            PauseGame();
        }
    }

    private void CalcMouseHit()
    {
        if (!Input.GetButtonDown("Fire1")) return;
        
        Ray _ray = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out var _hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            m_mouseHit = _hit;
            m_isMouseClickedGround = true;
            return;
        }

        m_isMouseClickedGround = false;
    }

    public void PauseGame()
    {
        onPauseEvent?.Invoke();
        m_bPauseToggle = !m_bPauseToggle;
        playerControllerInputBlocked = !playerControllerInputBlocked;
        Time.timeScale = Time.timeScale > 0 ? 0 : 1;
    }
}
