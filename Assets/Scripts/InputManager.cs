using UnityEngine;
using UnityEngine.Events;
using Player;

[System.Serializable]
public struct tPlayerCommand
{
    public KeyCode playerWalk;
    public KeyCode playerCrouch;
    public KeyCode playerThrowReady;
    public KeyCode playerThrowSomething;
}

public class InputManager : MonoBehaviour
{
    private static InputManager m_instance;
    public static InputManager Instance => m_instance;

    public tPlayerCommand command;

    public Camera baseCamera;

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
}
