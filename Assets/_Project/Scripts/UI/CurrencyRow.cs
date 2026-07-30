using TMPro;
using TowerDefenseIncremental.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefenseIncremental
{
    [DisallowMultipleComponent]
    public sealed class CurrencyRow : MonoBehaviour
    {
        private Image icon;
        private TextMeshProUGUI label;
        private TextMeshProUGUI value;

        public string CurrencyId { get; private set; }

        public void Build(string currencyId, string displayName, Sprite sprite, Color accent)
        {
            CurrencyId = currencyId;
            var rect = transform as RectTransform;
            rect.sizeDelta = new Vector2(256f, 74f);

            var background = gameObject.AddComponent<Image>();
            background.color = new Color(0.11f, 0.106f, 0.106f);
            gameObject.AddComponent<FlatShadowDuplicatorUI>();

            RuntimeUIFactory.CreateImage(
                "Accent",
                transform,
                null,
                accent,
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(0f, 0.5f),
                Vector2.zero,
                new Vector2(4f, 0f));

            icon = RuntimeUIFactory.CreateImage(
                "Icon",
                transform,
                sprite,
                Color.white,
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(36f, 0f),
                new Vector2(40f, 40f));
            icon.preserveAspect = true;

            label = RuntimeUIFactory.CreateText(
                "Currency Name",
                transform,
                displayName.ToUpperInvariant(),
                14f,
                new Color(0.89f, 0.75f, 0.73f),
                TextAlignmentOptions.Left,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(72f, -13f),
                new Vector2(-84f, 20f));

            value = RuntimeUIFactory.CreateText(
                "Currency Value",
                transform,
                "0",
                27f,
                accent,
                TextAlignmentOptions.Left,
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 0.5f),
                new Vector2(72f, -7f),
                new Vector2(-84f, 40f));
        }

        public void SetValue(int amount, int? maximum = null)
        {
            if (value == null)
                return;

            value.text = maximum.HasValue ? $"{amount:N0}<size=50%>/{maximum.Value:N0}</size>" : amount.ToString("N0");
        }
    }
}
