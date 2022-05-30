using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChapterData
{
    public string chapterName;

    public ushort chapterNumber;

    public List<LevelData> levelList;

    public bool isClearedChapter;
}

public class ChapterInfo : MonoBehaviour
{
    public ChapterData data;

    public void ChapterClear()
    {
        data.isClearedChapter = true;
    }
}
