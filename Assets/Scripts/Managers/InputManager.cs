using UnityEngine;
using UnityEngine.Events;
using Player;

public class InputManager : MonoBehaviour
{
    private static InputManager m_instance;
    public static InputManager Instance => m_instance;

    public tPlayerCommand command;

    public Camera baseCamera;

    // 플레이어 입력 이벤트
    public UnityAction onStartObjDrag;
    public UnityAction onEndObjDrag;
    public UnityAction onClimbing;

    private void Awake()
    {
        if (m_instance)
        {
            Destroy(gameObject);
            return;
        }
        
        m_instance = this;
    }

    public void GetPlayerInput()
    {
        // 플레이어 걷기
        if (Input.GetKeyDown(command.playerWalk))
        {
            GameManager.Instance.player.isWalking = !GameManager.Instance.player.isWalking;
        }

        // 플레이어 앉기
        if (Input.GetKeyDown(command.playerCrouch))
        {
            GameManager.Instance.player.isCrouching = !GameManager.Instance.player.isCrouching;
        }

        if (Input.GetKeyDown(command.playerThrowReady))
        {
            GameManager.Instance.player.isThrowingReady = !GameManager.Instance.player.isThrowingReady;
        }

        if (Input.GetKeyDown(command.playerThrowSomething))
        {
            if (!GameManager.Instance.player.isThrowingReady) return;
            
            GameManager.Instance.player.isThrowingSomething = true;
            GameManager.Instance.player.isThrowingReady = false;
        }
    }

    public void GetPlayerInteractionInput()
    {
        if (!GameManager.Instance.player.isInteractable) return;

        if (Input.GetKeyDown(command.playerInteraction))
        {
            GameManager.Instance.player.isActing = !GameManager.Instance.player.isActing;

            switch (GameManager.Instance.player.interactionType)
            {
                case EInteractionType.PushOrPull:
                {
                    if (!GameManager.Instance.player.isActing)
                    {
                        onEndObjDrag?.Invoke();
                        break;
                    }
                    onStartObjDrag?.Invoke();
                }
                break;

                case EInteractionType.Item:
                break;
            }
        }
    }
}
