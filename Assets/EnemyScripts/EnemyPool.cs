using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] public GameObject _HPrefab;
    [SerializeField] public int _poolSize;

    private List<GameObject> _allBullets;
    public static EnemyPool instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            _allBullets = new List<GameObject>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i <_poolSize; i++)
        {
            CreateEnemy();
        }
    }

    public GameObject GetEnemy()
    {
        foreach (GameObject bullet in _allBullets)
        {
            if (bullet.activeInHierarchy)
            {
                return bullet;
            }           
        }
        return GetEnemy();
    }

    public void ReleaseEnemy(GameObject bullet)
    {
        bullet.SetActive(false);               
    }

    public GameObject CreateEnemy()
    {
        GameObject obj = Instantiate(_HPrefab);
        obj.SetActive(false);
        _allBullets.Add(obj);   
        return obj;
    }
}
