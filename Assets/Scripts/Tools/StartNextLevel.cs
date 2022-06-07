using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNextLevel : MonoBehaviour
{
    public string nextLevelName;

    public void ContinueNextLevel()
    {
        SceneController.Instance.ChangeToNextScene(nextLevelName);
    }
}
