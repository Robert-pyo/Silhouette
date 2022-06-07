using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Player;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;
    public static GameManager Instance => s_instance;
    
    private PlayerController m_player;
    public PlayerController Player => m_player;

    [Header("Level Conditions")]
    public LevelData levelData;
    private int m_currentLevel;
    [SerializeField] private int activatedVisionCount;

    public UnityAction onItemUsed;

    public UnityAction onWireCreate;
    public UnityAction onVisionWardActivated;
    public UnityAction onVisionWardDeactivated;

    public UnityAction<int> onProgressUpdateEvent;
    public UnityAction<bool> conditionCompleteEvent;

    private void Awake()
    {
        if (!s_instance)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        SceneController.Instance.onSceneChangeEvent += SyncCurrentLevel;

        if (!levelData) return;
        m_currentLevel = levelData.level;
        activatedVisionCount = 0;
    }

    public void OnWardEnabled()
    {
        onVisionWardActivated?.Invoke();
        activatedVisionCount++;
        onWireCreate?.Invoke();

        if (activatedVisionCount == levelData.shouldActivateVisionWardCount)
        {
            conditionCompleteEvent?.Invoke(true);
        }

        onProgressUpdateEvent?.Invoke(activatedVisionCount);
    }

    public void OnWardDisabled()
    {
        onVisionWardDeactivated?.Invoke();
        activatedVisionCount--;

        if (activatedVisionCount != levelData.shouldActivateVisionWardCount)
        {
            conditionCompleteEvent?.Invoke(false);
        }
        
        onProgressUpdateEvent?.Invoke(activatedVisionCount);
    }

    private void SyncCurrentLevel()
    {
        print("SyncCurrentLevel Called!");
        levelData = LevelManager.Instance.currentLevel.data;
        activatedVisionCount = 0;

        if (!GameObject.FindGameObjectWithTag("Player"))
        {
            return;
        }

        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        print("Player Found");
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
