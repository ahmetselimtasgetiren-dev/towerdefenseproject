using UnityEngine;

namespace TowerDefenseIncremental
{
    [RequireComponent(typeof(Health))]
    public sealed class Enemy : MonoBehaviour, IDamageable
    {
        private EnemyPool pool;
        private EnemyData data;
        private GameManager game;
        private Vector2[] path;
        private float speed;
        private float segmentLength;
        private int waypoint;
        private Health health;
        private float cachedPathProgress;
        private bool resolved;

        public bool IsDead => !gameObject.activeInHierarchy || health == null || health.IsDead;
        public float PathProgress => cachedPathProgress;

        private void Awake() => health = GetComponent<Health>();

        private void OnEnable()
        {
            if (health != null)
                health.Died += Die;
        }

        private void OnDisable()
        {
            if (health != null)
                health.Died -= Die;
        }

        public void ConfigurePool(EnemyPool owner, EnemyData template)
        {
            pool = owner;
            data = template;
        }

        public void Spawn(
            GameManager owner,
            Vector2[] route,
            float healthMultiplier,
            float speedMultiplier)
        {
            game = owner;
            path = route;
            speed = data.MoveSpeed * speedMultiplier;
            waypoint = 0;
            cachedPathProgress = 0f;
            resolved = false;
            segmentLength = path.Length > 1 ? Vector2.Distance(path[0], path[1]) : 0f;

            transform.position = path[0];
            health.Initialize(Mathf.Max(1, Mathf.RoundToInt(data.BaseHealth * healthMultiplier)));
            gameObject.name = data.DisplayName;
            gameObject.SetActive(true);
            game.RegisterEnemy(this);
        }

        private void Update()
        {
            if (IsDead || path == null || path.Length < 2)
                return;

            var goal = path[waypoint + 1];
            transform.position = Vector2.MoveTowards(transform.position, goal, speed * Time.deltaTime);

            var remainingDistance = Vector2.Distance(transform.position, goal);
            cachedPathProgress = waypoint + (segmentLength > 0f ? 1f - remainingDistance / segmentLength : 1f);

            if (remainingDistance >= 0.01f)
                return;

            waypoint++;
            cachedPathProgress = waypoint;
            if (waypoint >= path.Length - 1)
            {
                Escape();
                return;
            }

            segmentLength = Vector2.Distance(path[waypoint], path[waypoint + 1]);
        }

        public void TakeDamage(int amount) => health.Damage(amount);

        private void Die()
        {
            if (resolved)
                return;

            resolved = true;
            game.EnemyDefeated(this, data.RewardOnDeath);
            pool.Release(this);
        }

        private void Escape()
        {
            if (resolved)
                return;

            resolved = true;
            game.EnemyEscaped(this, data.LeakDamage);
            pool.Release(this);
        }
    }
}
