using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public ushort levelNumber;

    public bool isCleared;
}

public class LevelInfo : MonoBehaviour
{
    public LevelData data;

    public void ClearLevel()
    {
        data.isCleared = true;
    }
}
