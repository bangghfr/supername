using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 60;
    public float fillProbability = 0.45f; // вероятность стены в начале
    public int smoothIterations = 6; // число итераций автоматов
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    private int[,] map;

    void Start()
    {
        GenerateMap();
        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }
        DrawMap();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        System.Random rand = new System.Random();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    // По краям оставить стену
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = rand.NextDouble() < fillProbability ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        int[,] newMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWalls = GetNeighborWallCount(x, y);

                if (neighborWalls > 4)
                    newMap[x, y] = 1; // Стена
                else if (neighborWalls < 4)
                    newMap[x, y] = 0; // Проход
                else
                    newMap[x, y] = map[x, y]; // Не менять
            }
        }

        map = newMap;
    }

    int GetNeighborWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        wallCount += map[neighborX, neighborY];
                    }
                }
                else
                {
                    // По краям считать как стену
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void DrawMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x, y, 0);
                if (map[x, y] == 1)
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity);
                }
                else
                {
                    Instantiate(floorPrefab, pos, Quaternion.identity);
                }
            }
        }
    }
}