using UnityEngine;

public class Restart : MonoBehaviour
{
    public void RestartThisScene()
    {
        SceneController.Instance.ChangeToNextScene(SceneController.Instance.currentScene.name);
    }
}
