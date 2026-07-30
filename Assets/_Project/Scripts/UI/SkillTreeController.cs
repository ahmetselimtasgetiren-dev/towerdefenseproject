using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefenseIncremental
{
    [DisallowMultipleComponent]
    public sealed class SkillTreeController : MonoBehaviour
    {
        private readonly Dictionary<string, SkillNodeButton> buttons = new();
        private MetaProgressionManager progression;
        private UIAssetCatalog assets;
        private DetailCardUI detailCard;
        private SkillNodeData selectedNode;

        public void Initialize(MetaProgressionManager manager, UIAssetCatalog catalog)
        {
            progression = manager;
            assets = catalog;
            Build();
            GameEvents.SkillNodeChanged += OnSkillNodeChanged;

            if (progression.Nodes.Count > 0)
                SelectNode(progression.Nodes[0].Id);
        }

        private void OnDestroy()
        {
            GameEvents.SkillNodeChanged -= OnSkillNodeChanged;
        }

        public void Refresh()
        {
            foreach (var node in progression.Nodes)
                RefreshNode(node);

            RefreshDetail();
        }

        private void Build()
        {
            var background = gameObject.AddComponent<Image>();
            background.color = new Color(0.035f, 0.055f, 0.09f, 0.985f);

            RuntimeUIFactory.CreateText(
                "Heading",
                transform,
                "PERMANENT UPGRADE TREE",
                28f,
                new Color(0.48f, 0.96f, 1f),
                TextAlignmentOptions.Left,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
                new Vector2(42f, -24f),
                new Vector2(-300f, 54f));

            var graphViewport = RuntimeUIFactory.CreateImage(
                "Graph Viewport",
                transform,
                null,
                new Color(0.055f, 0.08f, 0.13f, 0.9f),
                new Vector2(0f, 0f),
                new Vector2(0.72f, 1f),
                new Vector2(0.5f, 0.5f),
                new Vector2(18f, -30f),
                new Vector2(-54f, -116f));
            graphViewport.gameObject.AddComponent<RectMask2D>();

            var graphContent = RuntimeUIFactory.CreateRect(
                "Graph Content",
                graphViewport.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1100f, 760f));

            var panZoom = graphViewport.gameObject.AddComponent<GraphPanZoom>();
            panZoom.Initialize(graphContent);

            BuildNodes(graphContent);
            BuildConnectors(graphContent);

            var detailRect = RuntimeUIFactory.CreateRect(
                "Detail Card",
                transform,
                new Vector2(0.74f, 0.13f),
                new Vector2(0.98f, 0.88f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
            detailCard = detailRect.gameObject.AddComponent<DetailCardUI>();
            detailCard.Build(assets);
            detailCard.UpgradeRequested += UpgradeSelected;
            detailCard.RefundRequested += RefundSelected;
        }

        private void BuildNodes(RectTransform graphContent)
        {
            foreach (var node in progression.Nodes)
            {
                var nodeRect = RuntimeUIFactory.CreateRect(
                    node.DisplayName,
                    graphContent,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    node.TreePosition,
                    new Vector2(126f, 126f));
                var button = nodeRect.gameObject.AddComponent<SkillNodeButton>();
                button.Build(node.Id, assets != null ? assets.NodeFrameBase : null, node.Icon, SelectNode);
                buttons.Add(node.Id, button);
            }
        }

        private void BuildConnectors(RectTransform graphContent)
        {
            foreach (var node in progression.Nodes)
            {
                if (node.Prerequisites == null)
                    continue;

                foreach (var prerequisite in node.Prerequisites)
                {
                    if (prerequisite == null ||
                        !buttons.TryGetValue(prerequisite.Id, out var startButton) ||
                        !buttons.TryGetValue(node.Id, out var endButton))
                        continue;

                    var line = RuntimeUIFactory.CreateImage(
                        $"{prerequisite.DisplayName} to {node.DisplayName}",
                        graphContent,
                        null,
                        new Color(0.24f, 0.65f, 0.72f, 0.8f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        Vector2.zero,
                        Vector2.zero);
                    line.transform.SetAsFirstSibling();
                    line.gameObject.AddComponent<ConnectorLineRenderer>().Initialize(
                        startButton.transform as RectTransform,
                        endButton.transform as RectTransform,
                        line.color,
                        5f);
                }
            }
        }

        private void SelectNode(string id)
        {
            selectedNode = progression.FindNode(id);
            Refresh();
        }

        private void UpgradeSelected()
        {
            if (progression.TryUpgrade(selectedNode))
                Refresh();
        }

        private void RefundSelected()
        {
            if (progression.TryRefund(selectedNode))
                Refresh();
        }

        private void RefreshNode(SkillNodeData node)
        {
            var level = progression.GetLevel(node);
            var prerequisitesMet = node.Prerequisites == null ||
                System.Array.TrueForAll(node.Prerequisites, prerequisite => progression.GetLevel(prerequisite) > 0);
            var frame = !prerequisitesMet
                ? assets != null ? assets.NodeLocked : null
                : level > 0
                    ? assets != null ? assets.NodeGlowRing : null
                    : assets != null ? assets.NodeFrameBase : null;
            buttons[node.Id].SetState(frame, true, level, node.MaxLevel);
        }

        private void RefreshDetail()
        {
            if (selectedNode == null)
                return;

            detailCard.Show(
                selectedNode.DisplayName,
                selectedNode.Description,
                selectedNode.Cost,
                progression.GetLevel(selectedNode),
                selectedNode.MaxLevel,
                progression.CanUpgrade(selectedNode),
                progression.CanRefund(selectedNode));
        }

        private void OnSkillNodeChanged(string _, int __) => Refresh();
    }
}
