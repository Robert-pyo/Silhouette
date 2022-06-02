using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Player;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    [HideInInspector]
    public PlayerController player;

    [Header("Level Conditions")]
    [SerializeField] private LevelData m_levelData;
    private int currentLevel;
    [SerializeField] private int activatedVisionCount;

    public UnityAction onItemUsed;

    public UnityAction onWireCreate;
    public UnityAction onVisionWardActivated;
    public UnityAction onVisionWardDeactivated;

    public UnityAction<bool> conditionCompleteEvent;

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

        SceneController.Instance.onSceneChangeEvent += SyncCurrentLevel;

        if (!m_levelData) return;
        currentLevel = m_levelData.level;
        activatedVisionCount = 0;
    }

    public void OnWardEnabled()
    {
        onVisionWardActivated?.Invoke();
        activatedVisionCount++;
        onWireCreate?.Invoke();

        if (activatedVisionCount == m_levelData.shouldActivateVisionWardCount)
        {
            conditionCompleteEvent?.Invoke(true);
        }
    }

    public void OnWardDisabled()
    {
        onVisionWardDeactivated?.Invoke();
        activatedVisionCount--;

        if (activatedVisionCount != m_levelData.shouldActivateVisionWardCount)
        {
            conditionCompleteEvent?.Invoke(false);
        }
    }

    private void SyncCurrentLevel()
    {
        print("SyncCurrentLevel Called!");
        m_levelData = LevelManager.Instance.currentLevel.data;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        // 에디터 상에서의 게임 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 애플리케이션에서의 종료
        Application.Quit();
        //
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
}
