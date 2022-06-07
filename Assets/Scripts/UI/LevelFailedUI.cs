using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

public class LevelFailedUI : MonoBehaviour
{
    public ModalWindowManager lvFailWindow;

    private void Awake()
    {
        Init();

        SceneController.Instance.onSceneChangeEvent -= Init;
        SceneController.Instance.onSceneChangeEvent += Init;
    }
    
    private void Init()
    {
        if (!GameManager.Instance.Player) return;
        GameManager.Instance.Player.onDeadEvent = ActivateUI;
    }

    // private void FixedUpdate()
    // {
    //     print(lvFailWindow);
    // }

    private void ActivateUI()
    {
        print(lvFailWindow);
        lvFailWindow.gameObject.SetActive(true);
        lvFailWindow.OpenWindow();
    }
}
