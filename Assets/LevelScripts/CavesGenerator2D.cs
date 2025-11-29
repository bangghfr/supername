using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        public GameObject[] smallPlatforms;
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
        ConnectRooms();
        GenerateDecorAndPlatforms();
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
            r.CreateGameObject(mapParent, floorMaterial, wallMaterial, floorSortingLayer, floorSortingOrder, wallSortingLayer, wallSortingOrder, decorSmallPrefab, decorLargePrefab);
            rooms.Add(r);
        }
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

        public void CreateGameObject(Transform parent, Material floorMat, Material wallMat, string floorLayer, int floorOrder, string wallLayer, int wallOrder, GameObject decorSmall, GameObject decorLarge)
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

            List<Vector2> path = new List<Vector2> { start };
            for (int i = 0; i < bends; i++)
            {
                float t = (float)(i + 1) / (bends + 1);
                Vector2 bend = Vector2.Lerp(start, end, t);
                bend += new Vector2(Random.Range(-width, width), Random.Range(-width, width));
                path.Add(bend);
            }
            path.Add(end);

            GameObject corridorGO = new GameObject("Corridor");
            corridorGO.transform.parent = roomGO.transform;

            List<Vector2> verts = new List<Vector2>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector2 dir = (path[i + 1] - path[i]).normalized;
                Vector2 normal = new Vector2(-dir.y, dir.x) * width * 0.5f;
                verts.Add(path[i] + normal);
                verts.Insert(0, path[i] - normal);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts.Select(v => (Vector3)v).ToArray();
            List<int> tris = new List<int>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                int a = i;
                int b = i + 1;
                int c = verts.Count - 1 - i;
                int d = verts.Count - 2 - i;
                tris.AddRange(new int[] { a, b, c, a, c, d });
            }
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            MeshFilter mf = corridorGO.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = corridorGO.AddComponent<MeshRenderer>();
            mr.material = wallMat;
            mr.material.color = Color.Lerp(Biome.tint, other.Biome.tint, 0.5f);
            mr.sortingLayerName = wallLayer;
            mr.sortingOrder = wallOrder;

            PolygonCollider2D col = corridorGO.AddComponent<PolygonCollider2D>();
            col.points = verts.ToArray();
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
