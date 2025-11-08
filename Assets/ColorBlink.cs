using UnityEngine;
using UnityEngine.UI;

public class ColorBlink : MonoBehaviour
{
    public Color color1 = Color.white;  // Начальный цвет
    public Color color2 = Color.red;    // Цвет для мигания
    public float blinkInterval = 0.5f;  // Время между сменой цвета (в секундах)

    private Image rend;
    private bool isColor1 = true;

    void Start()
    {
        rend = GetComponent<Image>();
        if (rend == null)
        {
            Debug.LogError("Компонент Renderer не найден");
        }
        else
        {
            rend.material.color = color1;
        }
        InvokeRepeating("BlinkColor", blinkInterval, blinkInterval);
    }

    void BlinkColor()
    {
        if (rend != null)
        {
            rend.material.color = isColor1 ? color2 : color1;
            isColor1 = !isColor1;
        }
    }
}