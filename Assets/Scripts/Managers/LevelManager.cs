using System.Collections.Generic;
using LitJson;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChapterData
{
    public string chapterName;

    public List<LevelData> levelList;

    public bool isChapterCleared;
}

[System.Serializable]
public class LevelData
{
    public string level;

    public bool isCleared;
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager s_instance;
    public static LevelManager Instance => s_instance;

    private string m_savePath;

    // 현재 레벨
    public LevelData currentLevel;

    // 전체 레벨 상황
    public List<ChapterData> chapterDataList;

    private void Awake()
    {
        if (!s_instance)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
        
        // 세이브 경로 지정
        m_savePath = Path.Combine(Application.dataPath, "LevelData/Level.json");

        // 씬 바뀔 때 레벨 저장해주기
        SceneController.Instance.onSceneChangeEvent += SaveLevelData;
        SceneController.Instance.onSceneChangeEvent += SyncCurrentLevel;
    }

    public void SyncCurrentLevel()
    {
        bool _isLevelSelected = false;
        
        foreach (ChapterData _data in chapterDataList)
        {
            foreach (LevelData _lvData in _data.levelList)
            {
                if (SceneController.Instance.currentScene.name != _lvData.level) continue;

                currentLevel = _lvData;
                _isLevelSelected = true;
                break;
            }

            if (_isLevelSelected) break;
        }
    }

    public void SaveLevelData()
    {
        StringBuilder _sb = new StringBuilder();
        JsonWriter _writer = new JsonWriter(_sb);

        _writer.PrettyPrint = true;

        JsonMapper.ToJson(chapterDataList, _writer);
        
        File.WriteAllText(m_savePath, _sb.ToString());
    }

    public void LoadLevelData()
    {
        string _json = File.ReadAllText(m_savePath);

        chapterDataList = JsonMapper.ToObject<List<ChapterData>>(_json);
    }
}
