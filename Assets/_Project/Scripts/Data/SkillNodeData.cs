using UnityEngine;

namespace TowerDefenseIncremental
{
    public enum SkillEffectType
    {
        TowerUnlock,
        AttackSpeed,
        TargetingRange
    }

    [CreateAssetMenu(menuName = "Aether/Skill Node Data", fileName = "SkillNode")]
    public sealed class SkillNodeData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea, SerializeField] private string description;
        [Min(0), SerializeField] private int cost = 100;
        [Min(1), SerializeField] private int maxLevel = 1;
        [SerializeField] private SkillNodeData[] prerequisites;
        [SerializeField] private SkillEffectType effectType;
        [SerializeField] private float effectPerLevel;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2 treePosition;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public int Cost => cost;
        public int MaxLevel => maxLevel;
        public SkillNodeData[] Prerequisites => prerequisites;
        public SkillEffectType EffectType => effectType;
        public float EffectPerLevel => effectPerLevel;
        public Sprite Icon => icon;
        public Vector2 TreePosition => treePosition;
    }
}
