using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Text;

public class LevelManager : MonoBehaviour
{
    private static LevelManager s_instance;
    public static LevelManager Instance => s_instance;

    public List<ChapterInfo> chapterInfoList = new List<ChapterInfo>();

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

        SaveLevelData(true);
    }

    public void SaveLevelData(bool isCleared)
    {
        List<ChapterData> _data = new List<ChapterData>();
        foreach (ChapterInfo _info in chapterInfoList)
        {
            _data.Add(_info.data);
            //for (int i = 0; i < _info.levelList.Count; ++i)
            //{
            //    m_levelInfoDic.Add(_info.levelList[i].levelNumber, isCleared);
            //}
        }

        StringBuilder _sb = new StringBuilder();
        JsonWriter _writer = new JsonWriter(_sb);

        _writer.PrettyPrint = true;

        JsonMapper.ToJson(_data, _writer);
        string _path = Path.Combine(Application.dataPath, "LevelData/Level.json");
        File.WriteAllText(_path, _sb.ToString());
    }
}
