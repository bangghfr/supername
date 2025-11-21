using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyManager;


public class EnemyFactory 
{
    private readonly Transform _parent;
    private readonly Dictionary<EnemyType, GameObject?> _prefabs;
    private EnemyPool BulletPool;
    public EnemyFactory(Transform parent, Dictionary<EnemyType, GameObject?> prefabs)
    {
        _parent = parent;
        _prefabs = prefabs;
    }

    public GameObject? Create(EnemyType type, Vector3 position)
    {
        if (!_prefabs.TryGetValue(type, out var prefab) || prefab == null)
        {
            Debug.LogWarning($"[EnemyFactory] Ïðåôàá äëÿ {type} íå íàçíà÷åí â èíñïåêòîðå.");
            return null;
        }

        //var enemy = Object.Instantiate(prefab, position, Quaternion.identity, _parent);
        var enemy = EnemyPool.instance.CreateEnemy();
        enemy.name = $"Enemy_{type}";
        return enemy;
    }



   
   
}

//public enum EnemyType
//{
//    //HighestC,
//    //HighestC_idle,
//    //HighestC_run,
//    //HighestC_death,
//    //HighestC_attack,
//    //HighestC_defend,
//    //HighestC_AgressivDedefend,
//    //MediumC,
//    //MediumC_idle,
//    //MediumC_run,
//    //MediumC_death,
//    //MediumC_attack,
//    //MediumC_defend,
//    //MediumC_death_idle,

//    //DeepestC_idle,
//    //DeepestC_run,
//    // DeepestC_death,
//    //DeepestC_idle,
//    //DeepestC_idle,
//    //DeepestC_idle,
//    HighestC,
//    MediumC,
//    DeepestC

//}