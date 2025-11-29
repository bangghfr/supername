using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "Generation/Level Settings")]
public class LevelSettings : ScriptableObject
{
    [Header("Map Size")]
    public int mapWidth = 160;
    public int mapHeight = 100;
    public bool matchBackgroundSize = false;
    public SpriteRenderer background;
    public float tileSize = 1f;

    [Header("Rooms")]
    public int maxRooms = 12;
    public int roomMinSize = 6;
    public int roomMaxSize = 12;

    [Header("Corridors")]
    public int corridorWidth = 2;
    public float extraConnectorChance = 0.15f;
    public float additionalConnectionChance = 0.25f;

    [Header("Difficulty Gradient / Depth Shift")]
    public bool gradientDepth = false;
    public float depthStrength = 1.2f;

    [Header("Generation Variants")]
    public float sideTunnelChance = 0.1f;
    public float bigRoomChance = 0.15f;
    public float shaftChance = 0.1f;

    [Header("Diagnostics")]
    public bool debugDraw = false;
}
