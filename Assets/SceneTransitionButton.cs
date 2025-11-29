using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class SceneTransitionButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        // Получаем компонент Button из этого объекта
        button = GetComponent<Button>();

        // Убедитесь, что кнопка существует
        if (button != null)
        {
            // Добавляем обработчик нажатия кнопки
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    // Метод, который вызывается при нажатии на кнопку
    void OnButtonClicked()
    {
        // Переход на сцену с индексом 1
        SceneManager.LoadScene(1);
        YG2.InterstitialAdvShow();
    }
}
