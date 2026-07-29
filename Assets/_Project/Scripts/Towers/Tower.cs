using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class Tower : MonoBehaviour
    {
        private GameManager game;
        private float range;
        private int damage;
        private float cooldown;
        private float timer;
        private Color projectileColor;

        public void Initialize(GameManager owner, float attackRange, int attackDamage, float attackCooldown, Color boltColor)
        {
            game = owner;
            range = attackRange;
            damage = attackDamage;
            cooldown = attackCooldown;
            projectileColor = boltColor;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            var target = game.FindTarget(transform.position, range);
            if (target == null) return;
            game.Fire(transform.position, target, damage, projectileColor);
            timer = cooldown;
        }
    }
}
