using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeFx : MonoBehaviour
{
    public Image fadeImage;

    public float fadeAmountPerLoop;

    private void Awake()
    {

    }

    private IEnumerator FadeIn()
    {
        Color _imgColor = fadeImage.color;

        while (_imgColor.a <= 1)
        {
            _imgColor.a = fadeAmountPerLoop;
            fadeImage.color = _imgColor;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void FadeOut()
    {

    }
}
