using System.Collections.Generic;
using UnityEngine;

namespace Teaching.Octopus
{
	public class EnemyPool
	{
		private readonly Transform _parent;
		private readonly Dictionary<EnemyType, GameObject?> _prefabs;
		private readonly Dictionary<EnemyType, Queue<GameObject>> _pooledEnemies = new();
		
		public EnemyPool(Transform parent, Dictionary<EnemyType, GameObject?> prefabs)
		{
			_parent = parent;
			_prefabs = prefabs;
		}
		
		public GameObject? Get(EnemyType type)
		{
			//Dictionary<EnemyType, GameObject?> _prefabs;
			if (!_prefabs.TryGetValue(type, out var prefab) || prefab == null)
			{
				Debug.LogWarning($"[EnemyPool] Префаб для {type} не назначен в инспекторе.");
				return null;
			}

			var queue = GetOrCreateQueue(type);

			while (queue.Count > 0)
			{
				var enemy = queue.Dequeue();

				if (enemy != null)
				{
					enemy.transform.SetParent(_parent, false);
					enemy.name = $"Enemy_{type}";
					return enemy;
				}
			}

			var newEnemy = Object.Instantiate(prefab, _parent);
			newEnemy.name = $"Enemy_{type}";
			return newEnemy;
		}

		public void Release(EnemyType type, GameObject enemy)
		{
			var queue = GetOrCreateQueue(type);

			enemy.SetActive(false);
			enemy.transform.SetParent(_parent, false);
			queue.Enqueue(enemy);
		}

		private Queue<GameObject> GetOrCreateQueue(EnemyType type)
		{
			//Dictionary<EnemyType, Queue<GameObject>> _pooledEnemies
			if (!_pooledEnemies.TryGetValue(type, out var queue))
			{
				queue = new Queue<GameObject>();
				_pooledEnemies[type] = queue;
			}

			return queue;
		}
	}
}
