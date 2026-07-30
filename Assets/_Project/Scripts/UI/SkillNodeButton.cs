using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefenseIncremental
{
    [DisallowMultipleComponent]
    public sealed class SkillNodeButton : MonoBehaviour
    {
        private Image frame;
        private Image icon;
        private TextMeshProUGUI level;
        private Button button;
        private string nodeId;
        private Action<string> selected;

        public void Build(string id, Sprite frameSprite, Sprite iconSprite, Action<string> onSelected)
        {
            nodeId = id;
            selected = onSelected;

            frame = gameObject.AddComponent<Image>();
            frame.sprite = frameSprite;
            frame.type = Image.Type.Simple;

            button = gameObject.AddComponent<Button>();
            button.targetGraphic = frame;
            button.onClick.AddListener(Select);

            icon = RuntimeUIFactory.CreateImage(
                "Icon",
                transform,
                iconSprite,
                Color.white,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(-24f, -24f));
            icon.preserveAspect = true;

            level = RuntimeUIFactory.CreateText(
                "Level",
                transform,
                string.Empty,
                10f,
                Color.white,
                TextAlignmentOptions.BottomRight,
                Vector2.zero,
                Vector2.one,
                new Vector2(1f, 0f),
                new Vector2(-5f, 4f),
                new Vector2(-10f, -10f));
        }

        public void SetState(Sprite frameSprite, bool interactable, int currentLevel, int maximumLevel)
        {
            frame.sprite = frameSprite;
            button.interactable = interactable;
            level.text = currentLevel > 0 ? $"{currentLevel}/{maximumLevel}" : string.Empty;
            icon.color = interactable ? Color.white : new Color(1f, 1f, 1f, 0.3f);
        }

        private void Select() => selected?.Invoke(nodeId);
    }
}
