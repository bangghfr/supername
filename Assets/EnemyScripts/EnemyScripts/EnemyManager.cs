using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private readonly List<EnemyBase> enemies = new();
    public EnemyStatsSO stats;


    private void Awake() 
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Register(EnemyBase enemy)
    {
        if (!enemies.Contains(enemy)) enemies.Add(enemy);
    }

    public void Unregister(EnemyBase enemy)
    {
        if (enemies.Contains(enemy)) enemies.Remove(enemy);
    }
}