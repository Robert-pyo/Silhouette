using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    public Camera levelViewCamera;
    public float m_cameraSpeed;

    public TextMeshProUGUI levelText;

    private GameObject m_selectedChapter;

    private List<LevelNode> m_levelNodes = new List<LevelNode>();

    private int m_currentIndex;

    private void Awake()
    {
        if (!levelViewCamera)
        {
            Debug.LogError("레벨 뷰 카메라가 존재하지 않습니다.");
        }
    }

    private void Start()
    {
        StartCoroutine(ChangeLevelView(0));
    }

    private IEnumerator ChangeLevelView(int index)
    {
        Vector3 _targetPos = m_levelNodes[index].transform.position + new Vector3(-10f, 15f, -10f);
        levelText.text = "Level " + m_levelNodes[index].levelData.level;
        bool _onContinue = true;

        while (_onContinue)
        {
            if ((_targetPos - levelViewCamera.transform.position).sqrMagnitude < 0.1f)
            {
                _onContinue = false;
                continue;
            }

            levelViewCamera.transform.position = Vector3.Lerp(levelViewCamera.transform.position, _targetPos, m_cameraSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void SelectNextLevel()
    {
        if (m_currentIndex == m_levelNodes.Count - 1) return;
        StopAllCoroutines();

        m_currentIndex++;
        StartCoroutine(ChangeLevelView(m_currentIndex));
    }

    public void SelectPrevLevel()
    {
        if (m_currentIndex == 0) return;
        StopAllCoroutines();

        m_currentIndex--;
        StartCoroutine(ChangeLevelView(m_currentIndex));
    }

    // 챕터 선택 시 이벤트에 추가
    public void SelectChapter(GameObject chapterObj)
    {
        m_selectedChapter = chapterObj;

        m_levelNodes = m_selectedChapter.GetComponent<NodeLinker>().nodeList;
    }

    public void BackToChapterSelect()
    {
        m_selectedChapter.SetActive(false);
        gameObject.SetActive(false);
    }
}
