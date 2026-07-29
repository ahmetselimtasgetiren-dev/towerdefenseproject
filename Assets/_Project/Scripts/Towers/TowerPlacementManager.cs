using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefenseIncremental
{
    public enum TowerKind { Bolt, Ember, Prism }

    public sealed class TowerPlacementManager : MonoBehaviour
    {
        private readonly List<Tower> towers = new();
        private GameManager game;
        private PathManager path;
        private EconomyManager economy;

        public TowerKind SelectedKind { get; private set; } = TowerKind.Bolt;
        public int SelectedCost => DefinitionFor(SelectedKind).cost;
        public string SelectedLabel => DefinitionFor(SelectedKind).displayName;

        public void Initialize(GameManager owner, PathManager route, EconomyManager cash)
        {
            game = owner;
            path = route;
            economy = cash;
        }

        private void Update()
        {
            SelectWithKeyboard();
            if (game == null || game.State is RunState.Won or RunState.Lost || Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.position.ReadValue().y > Screen.height - 140)
                return;

            var point = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            point = new Vector2(Mathf.Round(point.x * 2f) / 2f, Mathf.Round(point.y * 2f) / 2f);
            if (!path.IsBuildable(point) || towers.Exists(tower => tower != null && Vector2.Distance(tower.transform.position, point) < .55f))
            {
                game.SetMessage("Build on open ground, clear of the aether road.");
                return;
            }

            var definition = DefinitionFor(SelectedKind);
            if (!economy.TrySpend(definition.cost))
            {
                game.SetMessage($"Need {definition.cost} gold for {definition.displayName}.");
                return;
            }

            var towerObject = SpriteFactory.CreateTower(definition.displayName, definition.shape, point, definition.color, 2);
            var tower = towerObject.AddComponent<Tower>();
            tower.Initialize(game, definition.range, definition.damage, definition.cooldown, definition.projectileColor);
            towers.Add(tower);
            game.SetMessage($"{definition.displayName} constructed. Press 1, 2, or 3 to change tower.");
        }

        private void SelectWithKeyboard()
        {
            if (Keyboard.current == null) return;
            var next = SelectedKind;
            if (Keyboard.current.digit1Key.wasPressedThisFrame) next = TowerKind.Bolt;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) next = TowerKind.Ember;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) next = TowerKind.Prism;
            if (next == SelectedKind) return;
            SelectedKind = next;
            game?.SetMessage($"Selected {SelectedLabel} - {SelectedCost} gold.");
        }

        private static TowerDefinition DefinitionFor(TowerKind kind) => kind switch
        {
            TowerKind.Ember => new TowerDefinition("Ember Triangle", 45, SpriteFactory.Shape.Triangle, new Color(.98f, .48f, .25f), new Color(1f, .79f, .30f), 2.65f, 17, .90f),
            TowerKind.Prism => new TowerDefinition("Prism Hex", 55, SpriteFactory.Shape.Hexagon, new Color(.48f, .85f, .65f), new Color(.55f, 1f, .77f), 3.15f, 6, .28f),
            _ => new TowerDefinition("Bolt Square", 30, SpriteFactory.Shape.Square, new Color(.30f, .71f, 1f), new Color(.65f, .90f, 1f), 2.35f, 8, .55f)
        };

        private readonly struct TowerDefinition
        {
            public readonly string displayName;
            public readonly int cost;
            public readonly SpriteFactory.Shape shape;
            public readonly Color color;
            public readonly Color projectileColor;
            public readonly float range;
            public readonly int damage;
            public readonly float cooldown;

            public TowerDefinition(string displayName, int cost, SpriteFactory.Shape shape, Color color, Color projectileColor, float range, int damage, float cooldown)
            {
                this.displayName = displayName; this.cost = cost; this.shape = shape; this.color = color; this.projectileColor = projectileColor;
                this.range = range; this.damage = damage; this.cooldown = cooldown;
            }
        }
    }
}
