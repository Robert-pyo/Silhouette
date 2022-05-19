using UnityEngine;
using UnityEngine.Events;
using Player;

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
}
