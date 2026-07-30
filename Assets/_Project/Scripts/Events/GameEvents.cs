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
        public static Action<int> CoresChanged;
        public static Action<string, int> SkillNodeChanged;
        public static Action<string> MessageChanged;
        public static Action<RunState> RunStateChanged;
    }
}
