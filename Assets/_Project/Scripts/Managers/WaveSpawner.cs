using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class WaveSpawner : MonoBehaviour
    {
        private readonly Queue<SpawnTicket> pending = new();
        private readonly List<WaveData> waves = new();

        private GameManager game;
        private PathManager path;
        private EnemyPool enemyPool;
        private float timer;

        public bool IsSpawning => pending.Count > 0;

        public void Initialize(
            GameManager owner,
            PathManager route,
            EnemyPool pool,
            IEnumerable<WaveData> waveDefinitions)
        {
            game = owner;
            path = route;
            enemyPool = pool;
            waves.AddRange(waveDefinitions);
            waves.Sort((left, right) => left.WaveNumber.CompareTo(right.WaveNumber));
        }

        public WaveData GetNextWave(int currentWave)
        {
            foreach (var wave in waves)
            {
                if (wave != null && wave.WaveNumber > currentWave)
                    return wave;
            }

            return null;
        }

        public bool HasWaveAfter(int currentWave) => GetNextWave(currentWave) != null;

        public void BeginWave(WaveData wave)
        {
            pending.Clear();
            timer = wave.DelayBeforeWave;

            foreach (var group in wave.Groups)
            {
                if (group?.Enemy == null)
                    continue;

                for (var i = 0; i < group.Count; i++)
                    pending.Enqueue(new SpawnTicket(group));
            }
        }

        private void Update()
        {
            if (!IsSpawning)
            {
                game?.TryCompleteWave();
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f)
                return;

            var ticket = pending.Dequeue();
            enemyPool.Get(
                ticket.Enemy,
                game,
                path.Waypoints,
                ticket.HealthMultiplier,
                ticket.SpeedMultiplier);
            timer = ticket.SpawnInterval;
        }

        private readonly struct SpawnTicket
        {
            public readonly EnemyData Enemy;
            public readonly float SpawnInterval;
            public readonly float HealthMultiplier;
            public readonly float SpeedMultiplier;

            public SpawnTicket(EnemySpawnGroup group)
            {
                Enemy = group.Enemy;
                SpawnInterval = group.SpawnInterval;
                HealthMultiplier = group.HealthMultiplier;
                SpeedMultiplier = group.SpeedMultiplier;
            }
        }
    }
}
