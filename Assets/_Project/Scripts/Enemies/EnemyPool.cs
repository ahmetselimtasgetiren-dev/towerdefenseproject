using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class EnemyPool : MonoBehaviour
    {
        private const int InitialCountPerType = 8;

        private readonly Dictionary<EnemyData, List<Enemy>> pools = new();

        public void Initialize(IEnumerable<EnemyData> enemyTypes)
        {
            foreach (var data in enemyTypes)
            {
                if (data == null || pools.ContainsKey(data))
                    continue;

                pools.Add(data, new List<Enemy>());
                for (var i = 0; i < InitialCountPerType; i++)
                    Create(data);
            }
        }

        public Enemy Get(
            EnemyData data,
            GameManager game,
            Vector2[] path,
            float healthMultiplier,
            float speedMultiplier)
        {
            if (!pools.TryGetValue(data, out var enemies))
            {
                enemies = new List<Enemy>();
                pools.Add(data, enemies);
            }

            foreach (var enemy in enemies)
            {
                if (!enemy.gameObject.activeSelf)
                {
                    enemy.Spawn(game, path, healthMultiplier, speedMultiplier);
                    return enemy;
                }
            }

            var created = Create(data);
            created.Spawn(game, path, healthMultiplier, speedMultiplier);
            return created;
        }

        public void Release(Enemy enemy)
        {
            if (enemy != null)
                enemy.gameObject.SetActive(false);
        }

        private Enemy Create(EnemyData data)
        {
            var enemyObject = SpriteFactory.CreateEnemy(
                data.DisplayName,
                Vector2.zero,
                data.Color,
                3,
                data.Tough);
            enemyObject.transform.SetParent(transform, true);

            var enemy = enemyObject.AddComponent<Enemy>();
            enemy.ConfigurePool(this, data);
            pools[data].Add(enemy);
            enemyObject.SetActive(false);
            return enemy;
        }
    }
}
