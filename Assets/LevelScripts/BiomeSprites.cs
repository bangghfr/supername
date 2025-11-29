using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CaveBiome", menuName = "ScriptableObjects/CaveBiome", order = 1)]
public class CaveBiomeSprites : ScriptableObject
{
    public string biomeName; // переименовал, чтобы не конфликтовать с классом
    public Tile floorTile;
    public Tile wallTile;

    // Если нужно хранить несколько вариантов спрайтов
    public BiomeSprites[] extraSprites;
}

[System.Serializable]
public class BiomeSprites
{
    public string name;
    public Sprite floor;
    public Sprite wall;
}
