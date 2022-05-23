using UnityEngine;
using UnityEngine.Events;
using Player;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    [HideInInspector]
    public PlayerController player;

    public UnityAction onItemUsed;

    private void Awake()
    {
        if (m_instance)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
    }
    
    
}
