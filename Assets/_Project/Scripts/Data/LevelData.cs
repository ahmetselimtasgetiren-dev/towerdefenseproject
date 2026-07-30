using UnityEngine;

namespace TowerDefenseIncremental
{
    [CreateAssetMenu(menuName = "Aether/Level Data", fileName = "Level")]
    public sealed class LevelData : ScriptableObject
    {
        [SerializeField] private Vector2[] waypoints;
        [SerializeField] private Vector2 buildMinimum = new(-8.5f, -4.7f);
        [SerializeField] private Vector2 buildMaximum = new(8.5f, 4.7f);
        [Min(0f), SerializeField] private float pathClearance = 0.72f;

        public Vector2[] Waypoints => waypoints;
        public Vector2 BuildMinimum => buildMinimum;
        public Vector2 BuildMaximum => buildMaximum;
        public float PathClearance => pathClearance;
    }
}
