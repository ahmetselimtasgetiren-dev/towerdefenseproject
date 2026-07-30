using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TowerDefenseIncremental
{
    internal static class RuntimeUIFactory
    {
        private static TMP_FontAsset runtimeFont;

        public static TMP_FontAsset Font
        {
            get
            {
                if (runtimeFont != null)
                    return runtimeFont;

                var sourceFont = Resources.Load<Font>("Fonts/PressStart2P-Regular");
                if (sourceFont == null)
                    sourceFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

                runtimeFont = TMP_FontAsset.CreateFontAsset(sourceFont);
                runtimeFont.name = "Aether Runtime TMP Font";
                return runtimeFont;
            }
        }

        public static RectTransform CreateRect(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return rect;
        }

        public static Image CreateImage(
            string name,
            Transform parent,
            Sprite sprite,
            Color color,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var rect = CreateRect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = sprite != null && sprite.border.sqrMagnitude > 0f ? Image.Type.Sliced : Image.Type.Simple;
            return image;
        }

        public static TextMeshProUGUI CreateText(
            string name,
            Transform parent,
            string value,
            float fontSize,
            Color color,
            TextAlignmentOptions alignment,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var rect = CreateRect(name, parent, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = Font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.raycastTarget = false;
            text.text = value;
            return text;
        }

        public static Button CreateButton(
            string name,
            Transform parent,
            Sprite sprite,
            Color color,
            string label,
            float fontSize,
            UnityAction clicked,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var image = CreateImage(name, parent, sprite, color, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(clicked);

            CreateText(
                "Label",
                image.transform,
                label,
                fontSize,
                new Color(0.30f, 0.07f, 0.08f),
                TextAlignmentOptions.Center,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(-24f, -12f));

            return button;
        }
    }
}
