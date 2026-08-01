using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseIncremental
{
    public sealed class TowerPlacementManager : MonoBehaviour
    {
        private readonly List<Tower> towers = new();
        private readonly List<TowerData> definitions = new();

        private GameManager game;
        private PathManager path;
        private EconomyManager economy;
        private GameInputRouter input;
        private MetaProgressionManager progression;
        private int selectedIndex;
        private bool placementQueued;
        private Vector2 queuedScreenPosition;

        private TowerData SelectedDefinition => definitions.Count == 0 ? null : definitions[selectedIndex];

        public int SelectedCost => SelectedDefinition != null ? SelectedDefinition.Cost : 0;
        public string SelectedLabel => SelectedDefinition != null ? SelectedDefinition.DisplayName : "No Tower Data";

        public void Initialize(
            GameManager owner,
            PathManager route,
            EconomyManager cash,
            GameInputRouter inputRouter,
            MetaProgressionManager metaProgression,
            IEnumerable<TowerData> towerDefinitions)
        {
            game = owner;
            path = route;
            economy = cash;
            input = inputRouter;
            progression = metaProgression;
            definitions.AddRange(towerDefinitions);
            definitions.RemoveAll(definition => definition == null);
            definitions.Sort((left, right) => left.SelectionSlot.CompareTo(right.SelectionSlot));
            selectedIndex = 0;
            input.TowerSelectionRequested += SelectTower;
            input.TowerPlacementRequested += QueueTowerPlacement;
        }

        private void OnDestroy()
        {
            if (input == null)
                return;

            input.TowerSelectionRequested -= SelectTower;
            input.TowerPlacementRequested -= QueueTowerPlacement;
        }

        private void Update()
        {
            if (!placementQueued)
                return;

            placementQueued = false;
            TryPlaceTower(queuedScreenPosition);
        }

        private void QueueTowerPlacement(Vector2 screenPosition)
        {
            queuedScreenPosition = screenPosition;
            placementQueued = true;
        }

        private void TryPlaceTower(Vector2 screenPosition)
        {
            if (game == null ||
                game.State is RunState.Won or RunState.Lost ||
                EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            var definition = SelectedDefinition;
            if (definition == null)
            {
                game.SetMessage("No TowerData assets are configured.");
                return;
            }

            var point = (Vector2)Camera.main.ScreenToWorldPoint(screenPosition);
            point = new Vector2(Mathf.Round(point.x * 2f) / 2f, Mathf.Round(point.y * 2f) / 2f);
            if (!path.IsBuildable(point) ||
                towers.Exists(tower => tower != null && Vector2.Distance(tower.transform.position, point) < 0.55f))
            {
                game.SetMessage("Build on open ground, clear of the aether road.");
                return;
            }

            if (!economy.TrySpend(definition.Cost))
            {
                game.SetMessage($"Need {definition.Cost} crystals for {definition.DisplayName}.");
                return;
            }

            var towerObject = new GameObject(definition.DisplayName);
            towerObject.transform.position = point;
            var tower = towerObject.AddComponent<Tower>();
            var rangeMultiplier = 1f + progression.GetEffectTotal(SkillEffectType.TargetingRange);
            var cooldownMultiplier = Mathf.Max(
                0.25f,
                1f - progression.GetEffectTotal(SkillEffectType.AttackSpeed));
            tower.Initialize(
                game,
                definition.Range * rangeMultiplier,
                definition.Damage,
                definition.Cooldown * cooldownMultiplier,
                definition.ProjectileColor);
            towers.Add(tower);
            game.SetMessage($"{definition.DisplayName} constructed. Press 1, 2, or 3 to change tower.");
        }

        private void SelectTower(int requestedIndex)
        {
            if (definitions.Count == 0)
                return;

            var next = Mathf.Clamp(requestedIndex, 0, definitions.Count - 1);
            if (next == selectedIndex)
                return;

            selectedIndex = next;
            game?.SetMessage($"Selected {SelectedLabel} - {SelectedCost} crystals.");
        }
    }
}
