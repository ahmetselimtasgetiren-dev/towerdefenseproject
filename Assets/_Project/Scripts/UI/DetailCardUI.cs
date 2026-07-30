using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefenseIncremental
{
    [DisallowMultipleComponent]
    public sealed class DetailCardUI : MonoBehaviour
    {
        private TextMeshProUGUI title;
        private TextMeshProUGUI description;
        private TextMeshProUGUI cost;
        private TextMeshProUGUI level;
        private Button upgrade;
        private Button refund;

        public event Action UpgradeRequested;
        public event Action RefundRequested;

        public void Build(UIAssetCatalog assets)
        {
            var background = gameObject.AddComponent<Image>();
            background.sprite = assets != null ? assets.PanelFrame : null;
            background.color = background.sprite != null ? Color.white : new Color(0.125f, 0.125f, 0.122f);

            title = RuntimeUIFactory.CreateText(
                "Title",
                transform,
                "SELECT A NODE",
                28f,
                new Color(1f, 0.70f, 0.68f),
                TextAlignmentOptions.Left,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(26f, -68f),
                new Vector2(-52f, 42f));

            description = RuntimeUIFactory.CreateText(
                "Description",
                transform,
                "Choose an upgrade node to inspect its permanent effect.",
                15f,
                new Color(0.89f, 0.75f, 0.73f),
                TextAlignmentOptions.TopLeft,
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(26f, -126f),
                new Vector2(-52f, -238f));

            cost = RuntimeUIFactory.CreateText(
                "Cost",
                transform,
                "COST --",
                17f,
                new Color(1f, 0.70f, 0.68f),
                TextAlignmentOptions.Left,
                new Vector2(0f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 0f),
                new Vector2(26f, 92f),
                new Vector2(-26f, 36f));

            level = RuntimeUIFactory.CreateText(
                "Level",
                transform,
                "LEVEL --",
                17f,
                new Color(0.48f, 0.96f, 1f),
                TextAlignmentOptions.Right,
                new Vector2(0.5f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-26f, 92f),
                new Vector2(-26f, 36f));

            upgrade = RuntimeUIFactory.CreateButton(
                "Upgrade",
                transform,
                assets != null ? assets.UpgradeButton : null,
                new Color(1f, 0.70f, 0.68f),
                "UPGRADE",
                15f,
                () => UpgradeRequested?.Invoke(),
                new Vector2(0f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 0f),
                new Vector2(26f, 24f),
                new Vector2(-34f, 54f));

            refund = RuntimeUIFactory.CreateButton(
                "Refund",
                transform,
                assets != null ? assets.RefundButton : null,
                new Color(0.21f, 0.21f, 0.21f),
                "REFUND",
                15f,
                () => RefundRequested?.Invoke(),
                new Vector2(0.5f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-26f, 24f),
                new Vector2(-34f, 54f));

            SetActions(false, false);
        }

        public void Show(string displayName, string details, int nodeCost, int currentLevel, int maximumLevel, bool canUpgrade, bool canRefund)
        {
            title.text = displayName.ToUpperInvariant();
            description.text = details.ToUpperInvariant();
            cost.text = $"COST {nodeCost:N0}";
            level.text = $"LEVEL {currentLevel:00}/{maximumLevel:00}";
            SetActions(canUpgrade, canRefund);
        }

        private void SetActions(bool canUpgrade, bool canRefund)
        {
            upgrade.interactable = canUpgrade;
            refund.interactable = canRefund;
        }
    }
}
