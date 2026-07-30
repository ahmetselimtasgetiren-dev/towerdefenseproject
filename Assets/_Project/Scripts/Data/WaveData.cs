using System;
using UnityEngine;

namespace TowerDefenseIncremental
{
    [CreateAssetMenu(menuName = "Aether/Wave Data", fileName = "WaveData")]
    public sealed class WaveData : ScriptableObject
    {
        [SerializeField, Min(1)] private int waveNumber = 1;
        [SerializeField, Min(0f)] private float delayBeforeWave = 0.3f;
        [SerializeField] private EnemySpawnGroup[] groups = Array.Empty<EnemySpawnGroup>();

        public int WaveNumber => waveNumber;
        public float DelayBeforeWave => delayBeforeWave;
        public EnemySpawnGroup[] Groups => groups;
    }

    [Serializable]
    public sealed class EnemySpawnGroup
    {
        [SerializeField] private EnemyData enemy;
        [SerializeField, Min(1)] private int count = 1;
        [SerializeField, Min(0.01f)] private float spawnInterval = 0.75f;
        [SerializeField, Min(0.01f)] private float healthMultiplier = 1f;
        [SerializeField, Min(0.01f)] private float speedMultiplier = 1f;

        public EnemyData Enemy => enemy;
        public int Count => count;
        public float SpawnInterval => spawnInterval;
        public float HealthMultiplier => healthMultiplier;
        public float SpeedMultiplier => speedMultiplier;
    }
}
