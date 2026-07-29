using UnityEngine;

namespace TowerDefenseIncremental
{
    /// <summary>Creates the Phase 1 game composition in the otherwise empty sample scene.</summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            if (FindAnyObjectByType<GameBootstrap>() == null) new GameObject("Tower Defense Game").AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            var game = gameObject.AddComponent<GameManager>();
            var path = gameObject.AddComponent<PathManager>();
            var economy = gameObject.AddComponent<EconomyManager>();
            var spawner = gameObject.AddComponent<WaveSpawner>();
            var pool = gameObject.AddComponent<ProjectilePool>();
            var placement = gameObject.AddComponent<TowerPlacementManager>();
            var meta = gameObject.AddComponent<MetaProgressionManager>();
            var hud = gameObject.AddComponent<RunHud>();
            game.Initialize(path, economy, spawner, pool, placement, meta, hud);
        }
    }
}
