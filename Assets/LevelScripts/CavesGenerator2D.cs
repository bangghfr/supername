using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static CavesGenerator2D;

[DisallowMultipleComponent]
public class CavesGenerator2D : MonoBehaviour
{
    [System.Serializable]
    public class Biome
    {
        public string name;
        public Color color = Color.white;
        public GameObject[] floorDecor;
    }

    [Header("Map Settings")]
    public int mapWidth = 200;
    public int mapHeight = 140;
    public Color backgroundColor = Color.black; // Стены
    public int roomCount = 12;

    [Header("Room Settings")]
    public Vector2 roomWidthRange = new Vector2(8f, 22f);
    public Vector2 roomHeightRange = new Vector2(6f, 14f);
    [Tooltip("0 = perfect rectangle, >0 adds corner noise")]
    public float rectCornerNoise = 0.20f;
    public float minRoomDistance = 2f;

    [Header("Corridor Settings")]
    public float corridorWidth = 2.5f;

    [Header("Prefabs")]
    public GameObject decorPrefab;
    public GameObject playerSpawnPrefab;
    public GameObject exitPrefab;

    [Header("Biomes")]
    public Biome[] biomes;

    private List<Room> rooms = new List<Room>();
    private List<Edge> corridors = new List<Edge>();
    private Transform mapParent;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;                // Префаб врага
    public int baseEnemies = 3;                   // На 1 уровне
    public int maxLevels = 10;                    // Всего уровней
    public float minEnemyDistance = 4f;           // Минимальная дистанция между врагами

    void Start()
    {
        GenerateLevel();
    }
    void SetLayerRecursively(Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
        {
            SetLayerRecursively(child, layer);
        }
    }
    public void GenerateLevel()
    {
        ClearPrevious();
        mapParent = new GameObject("CaveMap").transform;
        mapParent.parent = transform;
        // Изменяем слой GameObject, к которому прикреплен Transform
        mapParent.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Если нужно изменить слой рекурсивно для всех дочерних объектов
        SetLayerRecursively(mapParent, LayerMask.NameToLayer("Ignore Raycast"));

        // 1) Создание комнат
        int attempts = 0;
        while (rooms.Count < roomCount && attempts < roomCount * 100)
        {
            attempts++;
            Room r = CreateRandomRoom();
            if (r == null) continue;
            if (rooms.Any(other => RoomsOverlap(r, other))) continue;
            rooms.Add(r);
        }

        if (rooms.Count == 0) return;

        // 2) Генерация коридоров через MST
        var edges = GenerateAllEdges();
        var mst = BuildMST(rooms, edges);
        corridors.AddRange(mst);

        // 3) Спавн игрока и выхода
        PlacePlayerAndExit();

        // 4) Генерация визуальной карты
        GenerateMapTexture();

        // 5) Генерация коллайдеров по внешней границе
        GenerateExternalWallColliders();
    }

    private void ClearPrevious()
    {
        if (mapParent != null) DestroyImmediate(mapParent.gameObject);
        rooms.Clear();
        corridors.Clear();
    }

    private Room CreateRandomRoom()
    {
        float w = Random.Range(roomWidthRange.x, roomWidthRange.y);
        float h = Random.Range(roomHeightRange.x, roomHeightRange.y);
        Vector2 pos = new Vector2(
            Random.Range(10 + w / 2f, mapWidth - 10 - w / 2f),
            Random.Range(10 + h / 2f, mapHeight - 10 - h / 2f)
        );
        if (biomes == null || biomes.Length == 0) return null;
        Biome biome = biomes[Random.Range(0, biomes.Length)];
        return new Room(pos, w, h, rectCornerNoise, biome);
    }

    private bool RoomsOverlap(Room a, Room b)
    {
        Rect ra = a.GetAABB();
        Rect rb = b.GetAABB();
        ra.xMin -= minRoomDistance;
        ra.yMin -= minRoomDistance;
        ra.xMax += minRoomDistance;
        ra.yMax += minRoomDistance;
        return ra.Overlaps(rb);
    }

    private List<Edge> GenerateAllEdges()
    {
        var list = new List<Edge>();
        for (int i = 0; i < rooms.Count; i++)
            for (int j = i + 1; j < rooms.Count; j++)
                list.Add(new Edge(rooms[i], rooms[j]));
        return list;
    }

    private List<Edge> BuildMST(List<Room> nodes, List<Edge> edges)
    {
        edges.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        var ds = new DisjointSet(nodes.Count);
        var mapIndex = nodes.Select((r, idx) => new { r, idx }).ToDictionary(x => x.r, x => x.idx);
        List<Edge> result = new List<Edge>();
        foreach (var e in edges)
        {
            int ia = mapIndex[e.a];
            int ib = mapIndex[e.b];
            if (ds.Find(ia) != ds.Find(ib))
            {
                ds.Union(ia, ib);
                result.Add(e);
            }
        }
        return result;
    }

    private void PlacePlayerAndExit()
    {
        if (rooms.Count == 0) return;
        Room centerRoom = rooms.OrderBy(r => Vector2.Distance(r.Center, new Vector2(mapWidth / 2f, mapHeight / 2f))).First();
        if (playerSpawnPrefab != null)
            Instantiate(playerSpawnPrefab, new Vector3(centerRoom.Center.x, centerRoom.Center.y, -0.1f), Quaternion.identity, mapParent);

        Room farRoom = rooms.OrderByDescending(r => Vector2.Distance(r.Center, centerRoom.Center)).First();
        Vector2 edgePoint = ProjectToEdge(farRoom.Center);
        if (!rooms.Any(r => r.PointInside(edgePoint)))
            corridors.Add(new Edge(farRoom, new Room(edgePoint, 0, 0, 0, null)));

        if (exitPrefab != null)
            Instantiate(exitPrefab, new Vector3(edgePoint.x, edgePoint.y, -0.1f), Quaternion.identity, mapParent);
    }

    private Vector2 ProjectToEdge(Vector2 p)
    {
        List<(float dist, Vector2 pos)> edges = new List<(float, Vector2)>
        {
            (p.x, new Vector2(1f, Mathf.Clamp(p.y, 1f, mapHeight - 1f))),
            (mapWidth - p.x, new Vector2(mapWidth - 1f, Mathf.Clamp(p.y, 1f, mapHeight - 1f))),
            (p.y, new Vector2(Mathf.Clamp(p.x, 1f, mapWidth - 1f), 1f)),
            (mapHeight - p.y, new Vector2(Mathf.Clamp(p.x, 1f, mapWidth - 1f), mapHeight - 1f))
        };
        var sorted = edges.OrderByDescending(e => e.dist).Take(3).ToList();
        return sorted[Random.Range(0, sorted.Count)].pos;
    }

    private void GenerateMapTexture()
    {
        Texture2D tex = new Texture2D(mapWidth, mapHeight);
        Color[] pixels = new Color[mapWidth * mapHeight];

        for (int i = 0; i < pixels.Length; i++) pixels[i] = backgroundColor;

        foreach (var room in rooms)
        {
            Rect aabb = room.GetAABB();
            int minX = Mathf.Clamp(Mathf.FloorToInt(aabb.xMin), 0, mapWidth - 1);
            int maxX = Mathf.Clamp(Mathf.CeilToInt(aabb.xMax), 0, mapWidth - 1);
            int minY = Mathf.Clamp(Mathf.FloorToInt(aabb.yMin), 0, mapHeight - 1);
            int maxY = Mathf.Clamp(Mathf.CeilToInt(aabb.yMax), 0, mapHeight - 1);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    if (room.PointInside(p)) pixels[y * mapWidth + x] = Color.clear;
                }
        }

        foreach (var edge in corridors)
        {
            Vector2 a = edge.a.Center;
            Vector2 b = edge.b.Center;
            int steps = Mathf.CeilToInt(Vector2.Distance(a, b));
            for (int i = 0; i <= steps; i++)
            {
                Vector2 p = Vector2.Lerp(a, b, i / (float)steps);
                int px = Mathf.Clamp(Mathf.RoundToInt(p.x), 0, mapWidth - 1);
                int py = Mathf.Clamp(Mathf.RoundToInt(p.y), 0, mapHeight - 1);
                int rad = Mathf.CeilToInt(corridorWidth / 2f);
                for (int dx = -rad; dx <= rad; dx++)
                    for (int dy = -rad; dy <= rad; dy++)
                    {
                        int x = px + dx;
                        int y = py + dy;
                        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                            pixels[y * mapWidth + x] = Color.clear;
                    }
            }
        }

        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        GameObject go = new GameObject("MapBackground");
        go.transform.parent = mapParent;
        go.transform.position = new Vector3(mapWidth / 2f, mapHeight / 2f, 0);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, mapWidth, mapHeight), Vector2.one * 0.5f, 1f);
        sr.sortingOrder = -10;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

    }

    private void GenerateExternalWallColliders()
    {
        // bool[,] карта: true = стена, false = пустота
        bool[,] map = new bool[mapWidth, mapHeight];

        // 1) Задаём стены
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                map[x, y] = true;

        // 2) "Вырезаем" комнаты
        foreach (var room in rooms)
        {
            Rect aabb = room.GetAABB();
            int minX = Mathf.Clamp(Mathf.FloorToInt(aabb.xMin), 0, mapWidth - 1);
            int maxX = Mathf.Clamp(Mathf.CeilToInt(aabb.xMax), 0, mapWidth - 1);
            int minY = Mathf.Clamp(Mathf.FloorToInt(aabb.yMin), 0, mapHeight - 1);
            int maxY = Mathf.Clamp(Mathf.CeilToInt(aabb.yMax), 0, mapHeight - 1);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    if (room.PointInside(p)) map[x, y] = false;
                }
        }

        // 3) "Вырезаем" коридоры
        foreach (var edge in corridors)
        {
            Vector2 a = edge.a.Center;
            Vector2 b = edge.b.Center;
            int steps = Mathf.CeilToInt(Vector2.Distance(a, b));
            for (int i = 0; i <= steps; i++)
            {
                Vector2 p = Vector2.Lerp(a, b, i / (float)steps);
                int px = Mathf.Clamp(Mathf.RoundToInt(p.x), 0, mapWidth - 1);
                int py = Mathf.Clamp(Mathf.RoundToInt(p.y), 0, mapHeight - 1);
                int rad = Mathf.CeilToInt(corridorWidth / 2f);
                for (int dx = -rad; dx <= rad; dx++)
                    for (int dy = -rad; dy <= rad; dy++)
                    {
                        int x = px + dx;
                        int y = py + dy;
                        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                            map[x, y] = false;
                    }
            }
        }

        // 4) Создаём коллайдеры только по внешней границе
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (map[x, y])
                {
                    if (y + 1 < mapHeight && !map[x, y + 1]) CreateWallCollider(x, y, Vector2.up);
                    if (y - 1 >= 0 && !map[x, y - 1]) CreateWallCollider(x, y, Vector2.down);
                    if (x - 1 >= 0 && !map[x - 1, y]) CreateWallCollider(x, y, Vector2.left);
                    if (x + 1 < mapWidth && !map[x + 1, y]) CreateWallCollider(x, y, Vector2.right);
                }
            }
        }
    }

    private void CreateWallCollider(int x, int y, Vector2 direction)
    {
        GameObject go = new GameObject("WallCollider");
        go.transform.parent = mapParent;
        go.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);

        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(1f, 1f);

        if (direction == Vector2.up) bc.offset = new Vector2(0, 0.5f);
        if (direction == Vector2.down) bc.offset = new Vector2(0, -0.5f);
        if (direction == Vector2.left) bc.offset = new Vector2(-0.5f, 0);
        if (direction == Vector2.right) bc.offset = new Vector2(0.5f, 0);
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    #region Helper classes

    public class Room
    {
        public Vector2 Center;
        public float Width;
        public float Height;
        public Biome Biome;
        public Vector2[] polygon;
        private float cornerNoise;

        public Room(Vector2 center, float width, float height, float cornerNoise, Biome biome)
        {
            Center = center;
            Width = width;
            Height = height;
            Biome = biome;
            this.cornerNoise = Mathf.Clamp01(cornerNoise);
            polygon = GenerateRectPolygon();
        }

        private Vector2[] GenerateRectPolygon()
        {
            float hw = Width / 2f;
            float hh = Height / 2f;
            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(-hw, -hh);
            corners[1] = new Vector2(hw, -hh);
            corners[2] = new Vector2(hw, hh);
            corners[3] = new Vector2(-hw, hh);

            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = corners[i].normalized;
                float maxn = Mathf.Min(Width, Height) * cornerNoise;
                float nx = Mathf.PerlinNoise((Center.x + i * 17.13f) * 0.07f, (Center.y) * 0.07f);
                float ny = Mathf.PerlinNoise((Center.x) * 0.07f, (Center.y + i * 29.7f) * 0.07f);
                float rnd = (nx + ny) * 0.5f - 0.5f;
                corners[i] += dir * rnd * maxn + Center;
            }
            return corners;
        }

        public Rect GetAABB()
        {
            float minX = polygon.Min(p => p.x);
            float maxX = polygon.Max(p => p.x);
            float minY = polygon.Min(p => p.y);
            float maxY = polygon.Max(p => p.y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public bool PointInside(Vector2 p)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].y > p.y) != (polygon[j].y > p.y)) &&
                    (p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
            }
            return inside;
        }
    }

    public class Edge
    {
        public Room a, b;
        public float Distance;
        public Edge(Room a, Room b)
        {
            this.a = a; this.b = b;
            Distance = Vector2.Distance(a.Center, b.Center);
        }
    }

    public class DisjointSet
    {
        int[] parent;
        int[] rank;
        public DisjointSet(int n)
        {
            parent = new int[n];
            rank = new int[n];
            for (int i = 0; i < n; i++) parent[i] = i;
        }
        public int Find(int x) => parent[x] != x ? (parent[x] = Find(parent[x])) : x;
        public void Union(int a, int b)
        {
            int ra = Find(a), rb = Find(b);
            if (ra == rb) return;
            if (rank[ra] < rank[rb]) parent[ra] = rb;
            else if (rank[rb] < rank[ra]) parent[rb] = ra;
            else { parent[rb] = ra; rank[ra]++; }
        }
    }
    private void SpawnEnemies()
    {
        if (enemyPrefab == null) return;
        if (rooms.Count == 0) return;

        // Определяем номер уровня по индексам сцен
        int level = Mathf.Clamp(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex, 0, maxLevels - 1);

        // Количество врагов растет плавно: 3, 4, 5, ..., 12
        int enemyCount = baseEnemies + level;

        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < enemyCount; i++)
        {
            // Выбираем случайную комнату
            Room room = rooms[Random.Range(0, rooms.Count)];

            Vector2 spawnPos;
            int attempts = 0;

            // Выбираем точку, пока она:
            // 1) находится внутри комнаты
            // 2) далеко от остальных врагов
            do
            {
                Rect aabb = room.GetAABB();
                float x = Random.Range(aabb.xMin, aabb.xMax);
                float y = Random.Range(aabb.yMin, aabb.yMax);
                spawnPos = new Vector2(x, y);

                attempts++;

            } while (
                (!room.PointInside(spawnPos) ||
                usedPositions.Exists(p => Vector2.Distance(p, spawnPos) < minEnemyDistance))
                && attempts < 60
            );

            usedPositions.Add(spawnPos);

            Instantiate(
                enemyPrefab,
                new Vector3(spawnPos.x, spawnPos.y, -0.2f),
                Quaternion.identity,
                mapParent
            );
        }
    }

    #endregion
}
