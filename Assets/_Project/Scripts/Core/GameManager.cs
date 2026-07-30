using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefenseIncremental
{
    public enum RunState { Preparation, WaveActive, Won, Lost }

    public sealed class GameManager : MonoBehaviour
    {
        public const int StartingLives = 12;
        public RunState State { get; private set; } = RunState.Preparation;
        public int Lives { get; private set; }
        public int Wave { get; private set; }
        public string Message { get; private set; }
        public IReadOnlyList<Enemy> Enemies => enemies;

        private readonly List<Enemy> enemies = new();
        private PathManager path;
        private EconomyManager economy;
        private WaveSpawner spawner;
        private ProjectilePool pool;
        private TowerPlacementManager placement;
        private MetaProgressionManager meta;
        private RunHud hud;
        private GameInputRouter input;

        public void Initialize(
            PathManager pathManager,
            EconomyManager economyManager,
            WaveSpawner waveSpawner,
            ProjectilePool projectilePool,
            EnemyPool enemyPool,
            TowerPlacementManager placementManager,
            MetaProgressionManager metaManager,
            RunHud runHud,
            GameInputRouter inputRouter,
            IEnumerable<TowerData> towerDefinitions,
            IEnumerable<WaveData> waveDefinitions)
        {
            path = pathManager;
            economy = economyManager;
            spawner = waveSpawner;
            pool = projectilePool;
            placement = placementManager;
            meta = metaManager;
            hud = runHud;
            input = inputRouter;

            input.StartWavePressed += StartNextWave;
            input.RestartPressed += Restart;
            SetupCamera();
            path.BuildBoard();
            economy.ResetGold(90);
            Lives = StartingLives;
            pool.Initialize();
            placement.Initialize(this, path, economy, input, meta, towerDefinitions);
            spawner.Initialize(this, path, enemyPool, waveDefinitions);
            hud.Initialize(this, economy, meta);
            SetMessage("Build your defense, then launch Wave 1.");
        }

        private void OnDestroy()
        {
            if (input == null)
                return;

            input.StartWavePressed -= StartNextWave;
            input.RestartPressed -= Restart;
        }

        private void Restart()
        {
            if (State is RunState.Won or RunState.Lost)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void StartNextWave()
        {
            if (State != RunState.Preparation) return;

            var nextWave = spawner.GetNextWave(Wave);
            if (nextWave == null)
            {
                FinishRun(RunState.Won);
                return;
            }

            Wave = nextWave.WaveNumber;
            State = RunState.WaveActive;
            GameEvents.RunStateChanged?.Invoke(State);
            spawner.BeginWave(nextWave);
            SetMessage($"Wave {Wave} is on the way.");
            GameEvents.WaveStarted?.Invoke(Wave);
        }

        public void SetMessage(string value)
        {
            Message = value;
            GameEvents.MessageChanged?.Invoke(Message);
        }

        public void RegisterEnemy(Enemy enemy) => enemies.Add(enemy);
        public void UnregisterEnemy(Enemy enemy) => enemies.Remove(enemy);

        public Enemy FindTarget(Vector2 origin, float range)
        {
            Enemy best = null;
            foreach (var enemy in enemies)
                if (enemy != null && !enemy.IsDead && Vector2.Distance(origin, enemy.transform.position) <= range && (best == null || enemy.PathProgress > best.PathProgress)) best = enemy;
            return best;
        }

        public void Fire(Vector2 origin, Enemy target, int damage, Color color) => pool.Get().Launch(origin, target, damage, color);

        public void EnemyDefeated(Enemy enemy, int reward)
        {
            UnregisterEnemy(enemy);
            economy.Add(reward);
            GameEvents.EnemyKilled?.Invoke(reward);
        }

        public void EnemyEscaped(Enemy enemy, int damage)
        {
            UnregisterEnemy(enemy);
            Lives = Mathf.Max(0, Lives - damage);
            GameEvents.LivesChanged?.Invoke(Lives);
            if (Lives == 0) FinishRun(RunState.Lost);
        }

        public void TryCompleteWave()
        {
            if (State != RunState.WaveActive || spawner.IsSpawning || enemies.Count > 0) return;
            if (!spawner.HasWaveAfter(Wave)) FinishRun(RunState.Won);
            else
            {
                State = RunState.Preparation;
                GameEvents.RunStateChanged?.Invoke(State);
                SetMessage("Wave cleared. Spend your gold, then launch the next wave.");
                GameEvents.WaveCompleted?.Invoke(Wave);
            }
        }

        private void FinishRun(RunState result)
        {
            State = result;
            GameEvents.RunStateChanged?.Invoke(State);
            var gained = meta.AwardRun(result == RunState.Won, Wave);
            SetMessage(result == RunState.Won
                ? $"Victory! Earned {gained} cores. Press R to restart."
                : $"Base overrun. Earned {gained} cores. Press R to retry.");
        }

        private static void SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null) { camera = new GameObject("Main Camera").AddComponent<Camera>(); camera.tag = "MainCamera"; }
            camera.transform.position = new Vector3(0, 0, -10); camera.orthographic = true; camera.orthographicSize = 5.5f; camera.backgroundColor = new Color(.055f, .08f, .13f);
        }
    }
}
