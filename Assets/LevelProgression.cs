using UnityEngine;

public static class LevelProgression
{
    // Множители сложности
    public static float MapSizeMultiplier = 1.0f;
    public static float RoomCountMultiplier = 1.0f;
    public static float CorridorWidthMultiplier = 1.0f;

    // Новый множитель врагов
    public static float EnemyCountMultiplier = 1.0f;

    // Метод для увеличения сложности при переходе на новый уровень
    public static void IncreaseDifficulty(float mapMult, float roomMult, float corridorMult, float enemyMult)
    {
        MapSizeMultiplier *= mapMult;
        RoomCountMultiplier *= roomMult;
        CorridorWidthMultiplier *= corridorMult;
        EnemyCountMultiplier *= enemyMult;
    }
}

