using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTextManager : MonoBehaviour
{
    public TextMeshProUGUI sceneText;
    public float fadeDuration = 2f; // Длительность затухания
    private float fadeTimer = 0f;
    private bool hasFaded = false;
    private bool hasFlickered = false;

    private void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Устанавливаем красный цвет в зависимости от индекса сцены
        float redValue = Mathf.Clamp01(sceneIndex * 0.1f); // Например, каждый индекс добавляет 0.1 к красному
        sceneText.color = new Color(redValue, 0f, 0f, 1f);

        // Текст можно сразу показать
        sceneText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (hasFaded) return;

        // Плавное затухание
        fadeTimer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
        Color c = sceneText.color;
        sceneText.color = new Color(c.r, c.g, c.b, alpha);

        if (fadeTimer >= fadeDuration)
        {
            hasFaded = true;

            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            // Мерцание текста для сцен с индексом >= 7
            if (sceneIndex >= 7 && !hasFlickered)
            {
                StartCoroutine(FlickerOnce());
                hasFlickered = true;
            }
        }
    }

    private System.Collections.IEnumerator FlickerOnce()
    {
        sceneText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        sceneText.gameObject.SetActive(true);
    }
}
