using System;

namespace TowerDefenseIncremental
{
    public static class GameEvents
    {
        public static Action<int> GoldChanged;
        public static Action<int> LivesChanged;
        public static Action<int> EnemyKilled;
        public static Action<int> WaveStarted;
        public static Action<int> WaveCompleted;
    }
}
