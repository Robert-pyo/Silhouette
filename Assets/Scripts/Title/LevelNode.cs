using UnityEngine;
using UnityEngine.Events;

public class LevelNode : MonoBehaviour
{
    // 길이 연결될 시작지점과 끝지점
    public Transform startPos;
    public Transform endPos;

    // 씬 이동에 쓰일 레벨 정보
    public LevelInfo levelData;
    [SerializeField] private bool m_isPrevLevelCleared;
    private bool m_isStartingLevel;

    public UnityAction onLevelClickedEvent;

    private void OnEnable()
    {
        bool _isLevelSelected = false;

        foreach (ChapterInfo _chData in LevelManager.Instance.chapterDataList)
        {
            for (int i = 0; i < _chData.levelList.Count; ++i)
            {
                if (_chData.levelList[i].level != name) continue;

                levelData = _chData.levelList[i];

                if (i < 1)
                {
                    m_isStartingLevel = true;
                    break;
                }

                m_isPrevLevelCleared = _chData.levelList[i - 1].isCleared;
            }

            if (_isLevelSelected) break;
        }
    }

    private void OnMouseDown()
    {
        if (!m_isStartingLevel && !m_isPrevLevelCleared) return;

        SceneController.Instance.ChangeToNextScene(levelData.level);
        onLevelClickedEvent?.Invoke();
    }
}
