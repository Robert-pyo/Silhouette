using UnityEngine;
using Michsky.UI.ModernUIPack;

public class PauseUI : MonoBehaviour
{
    public ModalWindowManager modalWindow;
    
    private void Awake()
    {
        PlayerInput.Instance.onPauseEvent += PauseEnable;
    }

    private void PauseEnable()
    {
        if (!modalWindow.gameObject.activeSelf)
        {
            modalWindow.gameObject.SetActive(true);
            modalWindow.OpenWindow();
        }
        else
        {
            modalWindow.CloseWindow();
            modalWindow.gameObject.SetActive(false);
        }
    }
}
