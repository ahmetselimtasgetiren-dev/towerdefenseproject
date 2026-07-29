using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class WaveSpawner : MonoBehaviour
    {
        private GameManager game;
        private PathManager path;
        private int remaining;
        private float timer;

        public bool IsSpawning => remaining > 0;

        public void Initialize(GameManager owner, PathManager route) { game = owner; path = route; }
        public void BeginWave(int wave) { remaining = 5 + wave * 3; timer = .3f; }

        private void Update()
        {
            if (!IsSpawning) { game?.TryCompleteWave(); return; }
            timer -= Time.deltaTime;
            if (timer > 0f) return;

            var tough = game.Wave >= 3 && remaining % 4 == 0;
            var color = tough ? new Color(.77f, .39f, .81f) : new Color(.93f, .38f, .48f);
            var go = SpriteFactory.CreateEnemy(tough ? "Aether Brute" : "Aether Wisp", path.Waypoints[0], color, 3, tough);
            var enemy = go.AddComponent<Enemy>();
            enemy.Initialize(game, path.Waypoints, 17 + game.Wave * 7 + (tough ? 18 : 0), 1.25f + game.Wave * .06f - (tough ? .12f : 0f), tough ? 3 : 2);
            game.RegisterEnemy(enemy);
            remaining--;
            timer = Mathf.Max(.32f, .82f - game.Wave * .045f);
        }
    }
}
