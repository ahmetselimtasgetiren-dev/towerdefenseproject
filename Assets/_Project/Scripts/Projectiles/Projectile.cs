using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class Projectile : MonoBehaviour
    {
        private Enemy target;
        private int damage;

        public void Launch(Vector2 origin, Enemy enemy, int hitDamage, Color color)
        {
            transform.position = origin;
            target = enemy;
            damage = hitDamage;
            GetComponent<SpriteRenderer>().color = color;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (target == null || target.IsDead) { gameObject.SetActive(false); return; }
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, 12f * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.transform.position) >= .08f) return;
            target.TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
