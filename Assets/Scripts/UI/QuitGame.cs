using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void OnQuitGame()
    {
        GameManager.Instance.ExitGame();
    }
}
