using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject? HighestCenemyPrefab;
    [SerializeField] private GameObject? MediumCenemyPrefab;
    [SerializeField] private GameObject? DeepestCenemyPrefab;

    private EnemyFactory _enemyFactory = null!;
    private void Awake()
    {
        _enemyFactory = new EnemyFactory(transform, new Dictionary<EnemyType, GameObject?>
        {
            { EnemyType.HighestC, HighestCenemyPrefab},
            { EnemyType.MediumC, MediumCenemyPrefab},
            { EnemyType.HighestC, DeepestCenemyPrefab},
        });
    }
    public enum EnemyType
    {
        HighestC,
        MediumC,
        DeepestC
    }
    private void CreateHighestC()
    {
        _enemyFactory.Create(EnemyType.HighestC, new Vector3 (0,0,0));
    }
    private void CreateMediumC()
    {
        _enemyFactory.Create(EnemyType.HighestC, new Vector3(0, 0, 0));
    }
    private void CreateDeepestC()
    {
        _enemyFactory.Create(EnemyType.HighestC, new Vector3(0, 0, 0));
    }
}
