using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorBlink : MonoBehaviour
{
    [Header("Colors")]
    public Color color1 = Color.white;
    public Color color2 = Color.red;

    [Header("Smooth Blink Settings")]
    public float smoothSpeed = 2f; // скорость плавного перехода

    [Header("Horror Mode")]
    public bool horrorMode = false; // ФЛАЖОК
    public float minFlickerSpeed = 0.02f;
    public float maxFlickerSpeed = 0.2f;
    public float glitchChance = 0.25f; // шанс резкого скачка

    private Image image;
    private float t;
    private bool forward = true;

    void Awake()
    {
        image = GetComponent<Image>();

        if (!image)
        {
            Debug.LogError("Image component not found!");
            enabled = false;
            return;
        }

        image.color = color1;
    }

    void Update()
    {
        if (!horrorMode)
        {
            SmoothBlink();
        }
    }

    void SmoothBlink()
    {
        t += Time.deltaTime * smoothSpeed * (forward ? 1 : -1);
        t = Mathf.Clamp01(t);

        image.color = Color.Lerp(color1, color2, Mathf.SmoothStep(0, 1, t));

        if (t >= 1f) forward = false;
        if (t <= 0f) forward = true;
    }

    public void SetHorrorMode(bool value)
    {
        horrorMode = value;

        if (horrorMode)
            StartCoroutine(HorrorFlicker());
        else
            StopAllCoroutines();
    }

    IEnumerator HorrorFlicker()
    {
        while (horrorMode)
        {
            // Иногда резкий скачок
            if (Random.value < glitchChance)
            {
                image.color = Random.value > 0.5f ? color1 : color2;
            }
            else
            {
                // Плавный, но быстрый переход
                image.color = Color.Lerp(
                    image.color,
                    Random.value > 0.5f ? color1 : color2,
                    Random.Range(0.4f, 1f)
                );
            }

            // Иногда микропауза (эффект "сбой лампы")
            if (Random.value < 0.15f)
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

            yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));
        }
    }
}
