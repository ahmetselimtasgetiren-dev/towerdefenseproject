using UnityEngine;

namespace TowerDefenseIncremental
{
    [CreateAssetMenu(menuName = "Aether/Tower Data", fileName = "TowerData")]
    public sealed class TowerData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, Min(0)] private int selectionSlot;
        [SerializeField] private SpriteFactory.Shape shape;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Color projectileColor = Color.white;
        [SerializeField, Min(0f)] private float range = 2f;
        [SerializeField, Min(0)] private int damage = 1;
        [SerializeField, Min(0.01f)] private float cooldown = 1f;
        [SerializeField, Min(0)] private int cost = 10;

        public string Id => id;
        public string DisplayName => displayName;
        public int SelectionSlot => selectionSlot;
        public SpriteFactory.Shape Shape => shape;
        public Color Color => color;
        public Color ProjectileColor => projectileColor;
        public float Range => range;
        public int Damage => damage;
        public float Cooldown => cooldown;
        public int Cost => cost;

        private void OnValidate()
        {
            id = id?.Trim();
            displayName = displayName?.Trim();
        }
    }
}
