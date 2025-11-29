using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class CavesGenerator2D : MonoBehaviour
{
    [System.Serializable]
    public class Biome
    {
        public string name;
        public Sprite floorSprite;
        public Sprite wallSprite;
        public Sprite ceilingSprite;
        public GameObject[] floorDecor;
        public GameObject[] wallDecor;
        public GameObject[] ceilingDecor;
        public GameObject[] background;
        public GameObject[] smallPlatforms; // Used for enemies, chests, etc.
        public Color tint = Color.white;
    }

    [Header("Cave Settings")]
    public int mapWidth = 200;
    public int mapHeight = 140;

    [Header("Room Settings")]
    public Vector2 roomSizeRange = new Vector2(6f, 12f);
    public int roomShapePoints = 20;
    public float roomNoise = 0.8f;

    [Header("Corridors")]
    public float corridorWidth = 2.2f;
    public int corridorBends = 2;

    [Header("Rendering")]
    public Material floorMaterial;
    public Material wallMaterial;
    public string floorSortingLayer = "Default";
    public int floorSortingOrder = 0;
    public string wallSortingLayer = "Foreground";
    public int wallSortingOrder = 10;

    [Header("Prefabs")]
    public GameObject decorSmallPrefab;
    public GameObject decorLargePrefab;
    public GameObject exitPrefab;
    public GameObject playerSpawnPrefab;
    public GameObject platformPrefab;

    [Header("Biomes")]
    public Biome[] biomes;

    private List<Room> rooms = new List<Room>();
    private Transform mapParent;

    // New variables
    public int roomCount = 10;
    private float minRoomDistance = 1.5f;

    private void Start() => GenerateLevel();

    public void GenerateLevel()
    {
        ClearPrevious();
        mapParent = new GameObject("CaveMap").transform;
        mapParent.parent = transform;

        GenerateRooms();
        ConnectRooms();
        GenerateDecorAndPlatforms();
        PlacePlayerAndExit();
    }

    private void ClearPrevious()
    {
        if (mapParent != null) DestroyImmediate(mapParent.gameObject);
        rooms.Clear();
    }

    private void GenerateRooms()
    {
        int created = 0;
        int attempts = 0;

        while (created < roomCount && attempts < roomCount * 40)
        {
            attempts++;

            Vector2 pos = new Vector2(
                Random.Range(10, mapWidth - 10),
                Random.Range(10, mapHeight - 10)
            );

            float size = Random.Range(roomSizeRange.x, roomSizeRange.y);
            Biome biome = biomes[Random.Range(0, biomes.Length)];
            Room r = new Room(pos, size, roomShapePoints, roomNoise, biome, platformPrefab);

            if (r.polygon == null || r.polygon.Length < 5 || ShouldSkipCollider(r, rooms)) continue;

            r.CreateGameObject(mapParent, floorMaterial, wallMaterial, floorSortingLayer, floorSortingOrder, wallSortingLayer, wallSortingOrder, decorSmallPrefab, decorLargePrefab);
            rooms.Add(r);
            created++;
        }
    }

    private bool ShouldSkipCollider(Room room, List<Room> existingRooms)
    {
        foreach (var otherRoom in existingRooms)
        {
            if (RoomsOverlap(room, otherRoom, minRoomDistance))
                return true;
        }
        return false;
    }

    private bool RoomsOverlap(Room a, Room b, float minDist)
    {
        float distance = Vector2.Distance(a.Center, b.Center);
        return distance < (a.Size + b.Size) * 0.8f + minDist;
    }

    private void ConnectRooms()
    {
        if (rooms.Count < 2) return;

        rooms.Sort((a, b) => a.Center.x.CompareTo(b.Center.x));

        for (int i = 0; i < rooms.Count - 1; i++)
        {
            rooms[i].CreateCorridorTo(rooms[i + 1], corridorWidth, corridorBends, wallMaterial, wallSortingLayer, wallSortingOrder);
        }
    }

    private void GenerateDecorAndPlatforms()
    {
        foreach (var room in rooms)
            room.BuildDecor();
    }

    private void PlacePlayerAndExit()
    {
        if (rooms.Count == 0) return;

        Vector2 avg = Vector2.zero;
        foreach (var r in rooms) avg += r.Center;
        avg /= rooms.Count;

        Room centerRoom = rooms.OrderBy(r => Vector2.Distance(r.Center, avg)).First();
        Instantiate(playerSpawnPrefab, centerRoom.Center, Quaternion.identity);

        var distantRooms = rooms
            .OrderByDescending(r => Vector2.Distance(r.Center, centerRoom.Center))
            .Take(6)
            .ToList();

        int exitsCount = RandomExitCount();

        for (int i = 0; i < exitsCount; i++)
        {
            Room target = distantRooms[Random.Range(0, distantRooms.Count)];
            distantRooms.Remove(target);
            Instantiate(exitPrefab, target.Center, Quaternion.identity);
        }
    }

    private int RandomExitCount()
    {
        float r = Random.value;
        if (r < 0.65f) return 1;
        if (r < 0.9f) return 2;
        return 3;
    }

    private void BuildEnemiesAndChests(Room room)
    {
        void SpawnEnemies(GameObject[] enemyPrefabs)
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
            int count = Random.Range(2, 5);
            for (int i = 0; i < count; i++)
            {
                int idx = Random.Range(0, room.polygon.Length);
                Vector2 pos = room.polygon[idx];
                Object.Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], pos, Quaternion.identity, room.roomGO.transform);
            }
        }

        void SpawnChests(GameObject[] chestPrefabs)
        {
            if (chestPrefabs == null || chestPrefabs.Length == 0) return;
            int count = Random.Range(1, 3);
            for (int i = 0; i < count; i++)
            {
                int idx = Random.Range(0, room.polygon.Length);
                Vector2 pos = room.polygon[idx];
                Object.Instantiate(chestPrefabs[Random.Range(0, chestPrefabs.Length)], pos, Quaternion.identity, room.roomGO.transform);
            }
        }

        SpawnEnemies(room.Biome.smallPlatforms);  // Use specific biome instance for enemies
        SpawnChests(room.Biome.smallPlatforms);   // Use specific biome instance for chests
    }

    private void DrawColliderOutline(Vector2[] points, GameObject go)
    {
        LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;

        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
        }
        lineRenderer.SetPosition(points.Length, new Vector3(points[0].x, points[0].y, 0));
    }

    #region Room Class
    public class Room
    {
        public Vector2 Center;
        public float Size;
        public Biome Biome;
        public Vector2[] polygon; // Polygon that defines the room's shape
        public GameObject roomGO;
        private GameObject platformPrefab;

        public Room(Vector2 center, float size, int points, float noise, Biome biome, GameObject platformPrefab)
        {
            Center = center;
            Size = size;
            Biome = biome;
            this.platformPrefab = platformPrefab;
            polygon = GeneratePolygon(points, noise);
        }

        private Vector2[] GeneratePolygon(int points, float noise)
        {
            List<Vector2> pts = new List<Vector2>();
            float angleStep = 360f / points;

            for (int i = 0; i < points; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                // Using PerlinNoise to make rooms more varied
                float radius = Size * (1 + Mathf.PerlinNoise(Mathf.Cos(angle) * 0.5f, Mathf.Sin(angle) * 0.5f) * noise);
                pts.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius + Center);
            }
            return ChaikinSmooth(pts, 2).ToArray();
        }

        private List<Vector2> ChaikinSmooth(List<Vector2> pts, int iterations)
        {
            for (int iter = 0; iter < iterations; iter++)
            {
                List<Vector2> next = new List<Vector2>();
                for (int i = 0; i < pts.Count - 1; i++)
                {
                    Vector2 p0 = pts[i];
                    Vector2 p1 = pts[i + 1];
                    Vector2 q = (p0 + p1) * 0.25f;  // Point between p0 and p1
                    Vector2 r = (p0 + p1) * 0.75f;  // Point after q

                    next.Add(p0);
                    next.Add(q);
                    next.Add(r);
                }
                next.Add(pts[pts.Count - 1]);
                pts = next;
            }
            return pts;
        }

        public void CreateGameObject(Transform parent, Material floorMat, Material wallMat, string floorLayer, int floorOrder, string wallLayer, int wallOrder, GameObject decorPrefab, GameObject largePrefab)
        {
            roomGO = new GameObject("Room");
            roomGO.transform.parent = parent;

            // Create floor and walls
            CreateFloor(floorMat, floorLayer, floorOrder);
            CreateWalls(wallMat, wallLayer, wallOrder);
        }

        public void CreateFloor(Material floorMat, string layer, int order)
        {
            GameObject floor = new GameObject("Floor");
            floor.transform.parent = roomGO.transform;
            SpriteRenderer sr = floor.AddComponent<SpriteRenderer>();
            sr.sprite = Biome.floorSprite;
            sr.sortingLayerName = layer;
            sr.sortingOrder = order;
        }

        public void CreateWalls(Material wallMat, string layer, int order)
        {
            GameObject walls = new GameObject("Walls");
            walls.transform.parent = roomGO.transform;
            SpriteRenderer sr = walls.AddComponent<SpriteRenderer>();
            sr.sprite = Biome.wallSprite;
            sr.sortingLayerName = layer;
            sr.sortingOrder = order;
        }

        public void BuildDecor()
        {
            // Place decorative objects based on room edges
            Vector2[] edgePositions = GetEdgePositions();
            BuildDecor(edgePositions);
        }

        private void BuildDecor(Vector2[] edgePositions)
        {
            void SpawnDecor(GameObject[] prefabs, Vector2[] positions)
            {
                if (prefabs == null || prefabs.Length == 0) return;

                foreach (var pos in positions)
                {
                    Instantiate(prefabs[Random.Range(0, prefabs.Length)], pos, Quaternion.identity, roomGO.transform);
                }
            }

            // Spawn decoration along room edges
            SpawnDecor(Biome.floorDecor, edgePositions);
        }

        private Vector2[] GetEdgePositions()
        {
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < polygon.Length; i++)
            {
                positions.Add(polygon[i]);
            }
            return positions.ToArray();
        }

        public void CreateCorridorTo(Room other, float width, int bends, Material material, string layer, int order)
        {
            // Corridor generation logic goes here
        }
    }
    #endregion
}
