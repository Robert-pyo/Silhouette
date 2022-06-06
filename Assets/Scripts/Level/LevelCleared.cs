using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCleared : MonoBehaviour
{
    public void LevelComplete()
    {
        LevelManager.Instance.currentLevel.isCleared = true;
    }
}
