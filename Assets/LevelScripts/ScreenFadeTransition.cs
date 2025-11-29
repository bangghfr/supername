using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeTransition : MonoBehaviour
{
    [Header("UI Image for Fade")]
    public Image fadeImage; // Full-screen UI Image, black

    [Header("Fade Settings")]
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0f, 0f, 0f, 0f); // Start transparent
        }
    }

    public void FadeTransition(System.Action onComplete)
    {
        StartCoroutine(FadeCoroutine(onComplete));
    }

    private IEnumerator FadeCoroutine(System.Action onComplete)
    {
        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeImage != null)
                fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }

        // Execute action (level generation / load)
        onComplete?.Invoke();

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeImage != null)
                fadeImage.color = new Color(0f, 0f, 0f, 1f - Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
    }
}
