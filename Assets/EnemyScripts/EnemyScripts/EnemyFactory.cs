using UnityEngine;

namespace Teaching.Octopus
{
	public enum EnemyType
	{
		Walker,
		Runner,
		Flyer,
		Swimmer,
		Fired,
		Slipper
	}
	
	public class EnemyFactory
	{
		private readonly EnemyPool _enemyPool;

		public EnemyFactory(EnemyPool enemyPool)
		{
			_enemyPool = enemyPool;
		}

		public GameObject? Create(EnemyType type, Vector3 position)
		{
			var enemy = _enemyPool.Get(type);

			if (enemy == null)
			{
				return null;
			}

			var enemyTransform = enemy.transform;
			enemyTransform.SetPositionAndRotation(position, Quaternion.identity);
			enemy.SetActive(true);
			return enemy;
		}

		public void Release(EnemyType type, GameObject enemy)
		{
			_enemyPool.Release(type, enemy);
		}
	}
}
