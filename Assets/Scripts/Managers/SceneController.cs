using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneController : MonoBehaviour
{
    private static SceneController s_instance;
    public static SceneController Instance => s_instance;
    
    [HideInInspector] public Scene currentScene;
    [HideInInspector] public Scene prevScene;
    [HideInInspector] public Scene nextScene;

    public UnityAction onSceneChangeEvent;

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
        
        currentScene = SceneManager.GetActiveScene();
    }

    public void ChangeToNextScene(string nextSceneName)
    {
        prevScene = currentScene;
        SceneManager.LoadSceneAsync(nextSceneName);
        currentScene = SceneManager.GetSceneByName(nextSceneName);
        
        onSceneChangeEvent?.Invoke();
    }
}
