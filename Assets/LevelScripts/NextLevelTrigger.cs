using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    // Настройки увеличения сложности
    public float mapSizeMultiplier = 1.2f;
    public float roomCountMultiplier = 1.2f;
    public float corridorWidthMultiplier = 1.1f;

    // Новый параметр — множитель количества врагов
    public float enemyMultiplier = 1.15f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Увеличиваем прогрессию (теперь передаём также множитель врагов)
        LevelProgression.IncreaseDifficulty(mapSizeMultiplier, roomCountMultiplier, corridorWidthMultiplier, enemyMultiplier);

        // Переходим на следующую сцену по индексу
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Если следующая сцена есть в Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Если сцена последняя, можно вернуться на первую
            SceneManager.LoadScene(0);
        }
    }

    private void Awake()
    {
        // Закрепляем объект выхода
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        // Убедимся, что Collider2D триггер
        BoxCollider2D bc = gameObject.GetComponent<BoxCollider2D>();
        if (bc != null) bc.isTrigger = true;
    }
}
