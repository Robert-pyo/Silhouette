using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeFx : MonoBehaviour
{
    public Image fadeImage;

    public float fadeAmountPerLoop;

    private WaitForSeconds m_waitSeconds;

    private void Awake()
    {
        m_waitSeconds = new WaitForSeconds(0.01f);

        SceneController.Instance.onSceneInEvent += StartFadeIn;
        SceneController.Instance.onSceneOutEvent += StartFadeOut;
    }

    private void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        Color _imgColor = fadeImage.color;

        while (_imgColor.a >= 0.01)
        {
            _imgColor.a -= fadeAmountPerLoop;
            fadeImage.color = _imgColor;
            yield return m_waitSeconds;
        }
    }

    private IEnumerator FadeOut()
    {
        Color _imgColor = fadeImage.color;

        while (_imgColor.a <= 0.99)
        {
            _imgColor.a += fadeAmountPerLoop;
            fadeImage.color = _imgColor;
            yield return m_waitSeconds;
        }
    }
}
