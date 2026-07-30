using UnityEngine;

namespace TowerDefenseIncremental
{
    [CreateAssetMenu(menuName = "Aether/Enemy Data", fileName = "EnemyData")]
    public sealed class EnemyData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Color color = Color.white;
        [SerializeField, Min(1)] private int baseHealth = 10;
        [SerializeField, Min(0.01f)] private float moveSpeed = 1f;
        [SerializeField, Min(0)] private int rewardOnDeath = 1;
        [SerializeField, Min(1)] private int leakDamage = 1;
        [SerializeField] private bool tough;

        public string Id => id;
        public string DisplayName => displayName;
        public Color Color => color;
        public int BaseHealth => baseHealth;
        public float MoveSpeed => moveSpeed;
        public int RewardOnDeath => rewardOnDeath;
        public int LeakDamage => leakDamage;
        public bool Tough => tough;

        private void OnValidate()
        {
            id = id?.Trim();
            displayName = displayName?.Trim();
        }
    }
}
