using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [HideInInspector] public Scene currentScene;
    [HideInInspector] public Scene prevScene;
    [HideInInspector] public Scene nextScene;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    public void ChangeToNextScene(string nextSceneName)
    {
        prevScene = currentScene;
        SceneManager.LoadSceneAsync(nextSceneName);
        currentScene = SceneManager.GetSceneByName(nextSceneName);
    }
}
