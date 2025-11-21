using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CaveGenerator2D : MonoBehaviour
{
    [Header("Map Size")]
    public int width = 120;
    public int height = 80;

    [Header("Room Settings")]
    public int maxRooms = 18;
    public int roomMinSize = 6;
    public int roomMaxSize = 15;
    public float connectionChance = 0.60f;  // 60% шанс что комната будет соединена
    public float secretPassageChance = 0.05f; // маленький шанс секретного хода

    [Header("Tiles")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject tunnelDownPrefab;

    int[,] map;
    List<RectInt> rooms = new List<RectInt>();

    void Start()
    {
        Generate();
        DrawMap();
    }

    public void GenerateNextLevel(Transform player)
    {
        // Удаляем старые тайлы
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Генерим новый уровень
        Generate();
        DrawMap();

        // Перемещаем игрока в центр первой комнаты
        if (rooms.Count > 0)
        {
            Vector2Int startPos = Vector2Int.RoundToInt(rooms[0].center);
            player.position = new Vector3(startPos.x, startPos.y, 0f);
        }
    }

    void PlaceExitTunnel()
    {
        if (rooms.Count == 0) return;

        RectInt lastRoom = rooms[rooms.Count - 1];

        Vector2Int c = Vector2Int.RoundToInt(lastRoom.center);

        Vector2Int pos = new Vector2Int(
            c.x,
            lastRoom.yMin + 1
        );

        GameObject tunnel = Instantiate(
            tunnelDownPrefab,
            new Vector3(pos.x, pos.y, 0),
            Quaternion.identity,
            transform
        );

        var trig = tunnel.GetComponent<NextLevelTrigger>();
        trig.generator = this;
    }



    void Generate()
    {
        map = new int[width, height];

        // 1. Заполнение стенами
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1; // wall

        // 2. Создание комнат
        for (int i = 0; i < maxRooms; i++)
        {
            int w = Random.Range(roomMinSize, roomMaxSize);
            int h = Random.Range(roomMinSize, roomMaxSize);
            int x = Random.Range(1, width - w - 1);
            int y = Random.Range(1, height - h - 1);

            RectInt newRoom = new RectInt(x, y, w, h);

            bool overlaps = false;
            foreach (var r in rooms)
                if (newRoom.Overlaps(r))
                {
                    overlaps = true;
                    break;
                }

            if (!overlaps)
            {
                CreateRoom(newRoom);
                rooms.Add(newRoom);
            }
        }

        // 3. Соединяем только часть комнат
        rooms.Shuffle();

        for (int i = 0; i < rooms.Count - 1; i++)
        {
            if (Random.value < connectionChance)
                ConnectRooms(rooms[i], rooms[i + 1]);
        }

        // 4. Секретные ходы
        foreach (var r in rooms)
        {
            if (Random.value < secretPassageChance)
                CarveSmallPassage(r);
        }

        // 5. Рамка
        CreateBorders();
    }

    void CreateRoom(RectInt r)
    {
        for (int x = r.xMin; x < r.xMax; x++)
            for (int y = r.yMin; y < r.yMax; y++)
                map[x, y] = 0;
    }

    void ConnectRooms(RectInt r1, RectInt r2)
    {
        Vector2Int p1 = Vector2Int.RoundToInt(r1.center);
        Vector2Int p2 = Vector2Int.RoundToInt(r2.center);


        int corridorWidth = 3; // шире игрока

        if (Random.value < 0.5f)
        {
            CarveHorizontal(p1.x, p2.x, p1.y, corridorWidth);
            CarveVertical(p1.y, p2.y, p2.x, corridorWidth);
        }
        else
        {
            CarveVertical(p1.y, p2.y, p1.x, corridorWidth);
            CarveHorizontal(p1.x, p2.x, p2.y, corridorWidth);
        }
    }

    void CarveHorizontal(int x1, int x2, int y, int w)
    {
        if (x2 < x1) { int t = x1; x1 = x2; x2 = t; }
        for (int x = x1; x <= x2; x++)
            for (int dy = -w / 2; dy <= w / 2; dy++)
                SafeCarve(x, y + dy);
    }

    void CarveVertical(int y1, int y2, int x, int w)
    {
        if (y2 < y1) { int t = y1; y1 = y2; y2 = t; }
        for (int y = y1; y <= y2; y++)
            for (int dx = -w / 2; dx <= w / 2; dx++)
                SafeCarve(x + dx, y);
    }

    void CarveSmallPassage(RectInt room)
    {
        Vector2Int pos = new Vector2Int(
            Random.Range(room.xMin, room.xMax),
            room.yMin
        );

        for (int dx = 0; dx < 2; dx++)
            for (int dy = -1; dy < 1; dy++)
                SafeCarve(pos.x + dx, pos.y + dy);
    }

    void SafeCarve(int x, int y)
    {
        if (x > 1 && x < width - 1 && y > 1 && y < height - 1)
            map[x, y] = 0;
    }

    void CreateBorders()
    {
        for (int x = 0; x < width; x++)
        {
            map[x, 0] = 1;
            map[x, height - 1] = 1;
        }
        for (int y = 0; y < height; y++)
        {
            map[0, y] = 1;
            map[width - 1, y] = 1;
        }
    }

    void DrawMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = (map[x, y] == 0) ? floorPrefab : wallPrefab;
                Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}

