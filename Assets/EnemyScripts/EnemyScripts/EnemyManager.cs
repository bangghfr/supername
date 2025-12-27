using System.Collections.Generic;
using UnityEngine;

namespace Teaching.Octopus
{
	public class EnemyManager : MonoBehaviour
	{
		[SerializeField] private Transform _parent;
		
		[Header("Пример 1. Враги (Simple Enemy Factory)")]
		[SerializeField] private GameObject _walkerEnemyPrefab;
		[SerializeField] private GameObject _runnerEnemyPrefab;
		[SerializeField] private GameObject _flyerEnemyPrefab;
		
		[SerializeField] private Transform _flyerSpawnPosition;

		private EnemyPool _enemyPool = null!;
		private EnemyFactory _enemyFactory = null!;

		private void Awake()
		{
			var parent = _parent != null ? _parent : transform;

			_enemyPool = new EnemyPool(parent, new Dictionary<EnemyType, GameObject?>
			{
				{ EnemyType.Walker, _walkerEnemyPrefab },
				{ EnemyType.Runner, _runnerEnemyPrefab },
				{ EnemyType.Flyer,  _flyerEnemyPrefab }
			});

			_enemyFactory = new EnemyFactory(_enemyPool);
			
			SpawnEnemy(EnemyType.Flyer, _flyerSpawnPosition.position);
		}

		public GameObject SpawnEnemy(EnemyType type, Vector3 position)
		{
			return _enemyFactory.Create(type, position);
		}

		public void DespawnEnemy(EnemyType type, GameObject enemy)
		{
			_enemyFactory.Release(type, enemy);
		}
	}
}
