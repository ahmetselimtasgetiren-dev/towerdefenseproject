using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class ProjectilePool : MonoBehaviour
    {
        private readonly List<Projectile> pool = new();

        public void Initialize()
        {
            for (var index = 0; index < 32; index++)
                Create();
        }

        public Projectile Get()
        {
            foreach (var projectile in pool)
            {
                if (!projectile.gameObject.activeSelf)
                    return projectile;
            }

            return Create();
        }

        private Projectile Create()
        {
            var projectileObject = new GameObject("Pooled Bolt");
            projectileObject.transform.position = Vector2.zero;
            var projectile = projectileObject.AddComponent<Projectile>();
            pool.Add(projectile);
            projectileObject.SetActive(false);
            return projectile;
        }
    }
}
