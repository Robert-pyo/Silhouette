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

    public UnityAction onSceneChangeEvent;
    public UnityAction onSceneInEvent;
    public UnityAction onSceneOutEvent;

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
        StartCoroutine(ChangeScene(nextSceneName));
    }

    private IEnumerator ChangeScene(string nextSceneName)
    {
        onSceneOutEvent?.Invoke();

        yield return new WaitForSeconds(2f);

        prevScene = currentScene;
        SceneManager.LoadSceneAsync(nextSceneName);
        currentScene = SceneManager.GetSceneByName(nextSceneName);

        yield return new WaitUntil(() => currentScene.isLoaded);

        onSceneChangeEvent?.Invoke();
        onSceneInEvent?.Invoke();
    }

    public void ChangeSceneAdditively(string targetSceneName)
    {
        onSceneOutEvent?.Invoke();

        prevScene = currentScene;
        SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        currentScene = SceneManager.GetSceneByName(targetSceneName);

        onSceneChangeEvent?.Invoke();
        onSceneInEvent?.Invoke();
    }
}
