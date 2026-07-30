using UnityEngine;
using UnityEngine.InputSystem;

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
            var towers = Resources.LoadAll<TowerData>("TowerData");
            var waves = Resources.LoadAll<WaveData>("WaveData");
            var enemyTypes = Resources.LoadAll<EnemyData>("EnemyData");
            var skillNodes = Resources.LoadAll<SkillNodeData>("SkillNodeData");
            var level = Resources.Load<LevelData>("LevelData/DefaultLevel");
            var actions = Resources.Load<InputActionAsset>("InputSystem_Actions");

            var game = gameObject.AddComponent<GameManager>();
            var path = gameObject.AddComponent<PathManager>();
            var economy = gameObject.AddComponent<EconomyManager>();
            var spawner = gameObject.AddComponent<WaveSpawner>();
            var projectilePool = gameObject.AddComponent<ProjectilePool>();
            var enemyPool = gameObject.AddComponent<EnemyPool>();
            var placement = gameObject.AddComponent<TowerPlacementManager>();
            var meta = gameObject.AddComponent<MetaProgressionManager>();
            var hud = gameObject.AddComponent<RunHud>();
            var input = gameObject.AddComponent<GameInputRouter>();

            input.Initialize(actions);
            enemyPool.Initialize(enemyTypes);
            meta.Initialize(skillNodes);
            path.Initialize(level);
            game.Initialize(
                path,
                economy,
                spawner,
                projectilePool,
                enemyPool,
                placement,
                meta,
                hud,
                input,
                towers,
                waves);
        }
    }
}
