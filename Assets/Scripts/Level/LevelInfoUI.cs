using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class LevelInfoUI : MonoBehaviour
{
    public TextMeshProUGUI levelConditionText;

    private void OnEnable()
    {
        Invoke(nameof(Init), 0.3f);
        
        GameManager.Instance.onProgressUpdateEvent += UpdateLevelProgress;
    }

    private void Init()
    {
        UpdateLevelProgress(0);
    }

    private void UpdateLevelProgress(int activeWardCount)
    {
        levelConditionText.text = $"Activate Vision Ward : {activeWardCount.ToString()} / {GameManager.Instance.levelData.shouldActivateVisionWardCount.ToString()}";
    }
}
