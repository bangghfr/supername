using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneTransitionWithMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Image fadeImage;                // Черный фон для затемнения
    public GameObject confirmWindow;       // Окно подтверждения

    [Header("Settings")]
    public float fadeDuration = 1f;        // Скорость затемнения

    private bool isFading = false;

    void Start()
    {
        // При входе на сцену - плавное появление
        fadeImage.gameObject.SetActive(true);
        confirmWindow.SetActive(false);
        StartCoroutine(Fade(1, 0)); // Fade In
    }

    /// <summary>
    /// Вызывается, когда игрок подходит к порталу и нажимает "Е".
    /// Показывает меню.
    /// </summary>
    public void ShowConfirmMenu()
    {
        confirmWindow.SetActive(true);
    }

    /// <summary>
    /// Кнопка "Отмена"
    /// </summary>
    public void CancelTransition()
    {
        confirmWindow.SetActive(false);
    }

    /// <summary>
    /// Кнопка "Перейти"
    /// </summary>
    public void AcceptTransition()
    {
        if (!isFading)
            StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        isFading = true;

        // Плавное затемнение
        yield return StartCoroutine(Fade(0, 1));

        // Загрузка новой сцены
        SceneManager.LoadScene(1);
    }

    private IEnumerator Fade(float start, float end)
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = end;
        fadeImage.color = c;
    }
}
