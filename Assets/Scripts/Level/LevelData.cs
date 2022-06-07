using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    public int level;

    [Header("Level Conditions")]
    public int shouldActivateVisionWardCount;

    [Header("Optional Conditions")]
    public int killEnemyCount;
}
