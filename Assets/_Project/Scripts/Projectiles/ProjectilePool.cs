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
            var projectileObject = SpriteFactory.Box(
                "Pooled Bolt",
                Vector2.zero,
                new Vector2(0.18f, 0.18f),
                new Color(1f, 0.89f, 0.28f),
                5);
            var projectile = projectileObject.AddComponent<Projectile>();
            pool.Add(projectile);
            projectileObject.SetActive(false);
            return projectile;
        }
    }
}
