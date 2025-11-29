using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DisallowMultipleComponent]
public class HollowKnightCaveGeneratorFull : MonoBehaviour
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
        public GameObject[] platforms;
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

    private void Start() => GenerateLevel();

    public void GenerateLevel()
    {
        ClearPrevious();
        mapParent = new GameObject("CaveMap").transform;
        mapParent.parent = transform;

        GenerateRooms();
        ConnectRoomsMST();
        BuildDecorAndPlatforms();
        PlacePlayerAndExit();
    }

    private void GenerateRooms()
    {
        int targetRooms = 10;
        for (int i = 0; i < targetRooms; i++)
        {
            Vector2 pos = new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            float size = Random.Range(roomSizeRange.x, roomSizeRange.y);
            Biome biome = biomes[Random.Range(0, biomes.Length)];
            Room r = new Room(pos, size, roomShapePoints, roomNoise, biome, platformPrefab);
            r.CreateRoomGO(mapParent, floorMaterial, wallMaterial, floorSortingLayer, floorSortingOrder, wallSortingLayer, wallSortingOrder, decorSmallPrefab, decorLargePrefab);
            rooms.Add(r);
        }
    }

    private void ConnectRoomsMST()
    {
        if (rooms.Count < 2) return;

        List<(Room, Room)> edges = new List<(Room, Room)>();
        HashSet<Room> connected = new HashSet<Room> { rooms[0] };
        List<Room> remaining = new List<Room>(rooms.Skip(1));

        while (remaining.Count > 0)
        {
            float minDist = float.MaxValue;
            Room a = null, b = null;

            foreach (var c in connected)
            {
                foreach (var r in remaining)
                {
                    float d = Vector2.Distance(c.Center, r.Center);
                    if (d < minDist)
                    {
                        minDist = d;
                        a = c;
                        b = r;
                    }
                }
            }

            edges.Add((a, b));
            connected.Add(b);
            remaining.Remove(b);
        }

        foreach (var edge in edges)
            edge.Item1.CreateCorridorTo(edge.Item2, corridorWidth, corridorBends, wallMaterial, wallSortingLayer, wallSortingOrder);
    }

    private void BuildDecorAndPlatforms()
    {
        foreach (var r in rooms)
            r.BuildDecor();
    }

    private void PlacePlayerAndExit()
    {
        if (rooms.Count == 0) return;
        if (playerSpawnPrefab) Instantiate(playerSpawnPrefab, rooms[0].Center, Quaternion.identity);
        if (exitPrefab) Instantiate(exitPrefab, rooms[rooms.Count - 1].Center, Quaternion.identity);
    }

    private void ClearPrevious()
    {
        if (mapParent != null) DestroyImmediate(mapParent.gameObject);
        rooms.Clear();
    }

    #region Room Class
    private class Room
    {
        public Vector2 Center;
        public float Size;
        public Biome Biome;
        private Vector2[] polygon;
        private GameObject roomGO;
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
                float radius = Size * (1 + (Mathf.PerlinNoise(Mathf.Cos(angle), Mathf.Sin(angle)) - 0.5f) * noise);
                pts.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius + Center);
            }
            return ChaikinSmooth(pts, 2).ToArray();
        }

        private List<Vector2> ChaikinSmooth(List<Vector2> pts, int iterations)
        {
            for (int iter = 0; iter < iterations; iter++)
            {
                List<Vector2> next = new List<Vector2>();
                for (int i = 0; i < pts.Count; i++)
                {
                    Vector2 p0 = pts[i];
                    Vector2 p1 = pts[(i + 1) % pts.Count];
                    next.Add(Vector2.Lerp(p0, p1, 0.25f));
                    next.Add(Vector2.Lerp(p0, p1, 0.75f));
                }
                pts = next;
            }
            return pts;
        }

        public void CreateRoomGO(Transform parent, Material floorMat, Material wallMat, string floorLayer, int floorOrder, string wallLayer, int wallOrder, GameObject decorSmall, GameObject decorLarge)
        {
            roomGO = new GameObject("Room");
            roomGO.transform.parent = parent;

            MeshFilter mf = roomGO.AddComponent<MeshFilter>();
            MeshRenderer mr = roomGO.AddComponent<MeshRenderer>();
            mr.material = floorMat;
            mr.material.color = Biome.tint;
            mr.sortingLayerName = floorLayer;
            mr.sortingOrder = floorOrder;
            mf.mesh = PolygonToMesh(polygon);

            PolygonCollider2D col = roomGO.AddComponent<PolygonCollider2D>();
            col.points = polygon;

            PlacePlatforms();
        }

        private void PlacePlatforms()
        {
            if (platformPrefab == null) return;
            int count = Random.Range(2, 5);
            float step = polygon.Length / (float)count;

            for (int i = 0; i < count; i++)
            {
                int idx = Mathf.FloorToInt(i * step) % polygon.Length;
                Vector2 pos = polygon[idx];
                pos.y += Random.Range(0.5f, Size * 0.3f);
                GameObject plat = Object.Instantiate(platformPrefab, pos, Quaternion.identity, roomGO.transform);
                plat.GetComponent<SpriteRenderer>().color = Biome.tint;
            }
        }

        public void CreateCorridorTo(Room other, float width, int bends, Material wallMat, string wallLayer, int wallOrder)
        {
            Vector2 start = Center;
            Vector2 end = other.Center;

            List<Vector2> path = new List<Vector2> { start, end };

            List<Vector2> verts = new List<Vector2>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector2 dir = (path[i + 1] - path[i]).normalized;
                Vector2 normal = new Vector2(-dir.y, dir.x) * width * 0.5f;
                verts.Add(path[i] + normal);
                verts.Add(path[i] - normal);
                verts.Add(path[i + 1] - normal);
                verts.Add(path[i + 1] + normal);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts.Select(v => (Vector3)v).ToArray();

            List<int> tris = new List<int>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                int idx = i * 4;
                tris.AddRange(new int[] { idx, idx + 1, idx + 2, idx, idx + 2, idx + 3 });
            }
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            GameObject corridorGO = new GameObject("Corridor");
            corridorGO.transform.parent = roomGO.transform;
            MeshFilter mf = corridorGO.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = corridorGO.AddComponent<MeshRenderer>();
            mr.material = wallMat;
            mr.material.color = Color.Lerp(Biome.tint, other.Biome.tint, 0.5f);
            mr.sortingLayerName = wallLayer;
            mr.sortingOrder = wallOrder;

            PolygonCollider2D col = corridorGO.AddComponent<PolygonCollider2D>();
            col.points = verts.ToArray();

            // Вырез коллайдера комнаты на входе туннеля
            if (roomGO.TryGetComponent<PolygonCollider2D>(out PolygonCollider2D roomCol))
            {
                List<Vector2> newPoints = new List<Vector2>();
                foreach (var p in polygon)
                {
                    bool insideTunnel = false;
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Vector2 a = path[i];
                        Vector2 b = path[i + 1];
                        Vector2 ab = b - a;
                        Vector2 ap = p - a;
                        float proj = Vector2.Dot(ap, ab.normalized);
                        if (proj >= 0 && proj <= ab.magnitude)
                        {
                            Vector2 closest = a + ab.normalized * proj;
                            if (Vector2.Distance(p, closest) < width)
                            {
                                insideTunnel = true;
                                break;
                            }
                        }
                    }
                    if (!insideTunnel)
                        newPoints.Add(p);
                }
                if (newPoints.Count >= 3)
                    roomCol.points = newPoints.ToArray();
            }
        }

        public void BuildDecor()
        {
            void SpawnDecor(GameObject[] prefabs, float yOffset = 0f)
            {
                if (prefabs == null || prefabs.Length == 0) return;
                int step = Mathf.Max(1, polygon.Length / prefabs.Length);
                for (int i = 0; i < polygon.Length; i += step)
                {
                    Vector2 pos = polygon[i];
                    pos.y += yOffset;
                    Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)], pos, Quaternion.identity, roomGO.transform);
                }
            }

            SpawnDecor(Biome.floorDecor, -0.2f);
            SpawnDecor(Biome.wallDecor);
            SpawnDecor(Biome.ceilingDecor, 0.2f);
            SpawnDecor(Biome.background, 0.5f);
        }

        private Mesh PolygonToMesh(Vector2[] poly)
        {
            Mesh mesh = new Mesh();
            if (poly.Length < 3) return mesh;

            Vector3[] verts = new Vector3[poly.Length + 1];
            for (int i = 0; i < poly.Length; i++) verts[i] = poly[i];
            Vector2 centroid = Vector2.zero;
            foreach (var p in poly) centroid += p;
            centroid /= poly.Length;
            verts[poly.Length] = centroid;

            List<int> tris = new List<int>();
            for (int i = 0; i < poly.Length; i++)
            {
                int a = i;
                int b = (i + 1) % poly.Length;
                int c = poly.Length;
                tris.Add(a); tris.Add(b); tris.Add(c);
            }

            mesh.vertices = verts;
            mesh.triangles = tris.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
    #endregion
}
