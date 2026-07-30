using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class MetaProgressionManager : MonoBehaviour
    {
        private readonly Dictionary<string, int> levels = new();
        private readonly List<SkillNodeData> nodes = new();

        public int Cores { get; private set; }
        public IReadOnlyList<SkillNodeData> Nodes => nodes;

        public void Initialize(IEnumerable<SkillNodeData> definitions)
        {
            nodes.Clear();
            nodes.AddRange(definitions.Where(node => node != null && !string.IsNullOrWhiteSpace(node.Id)));

            var save = SaveSystem.LoadMeta();
            Cores = Mathf.Max(0, save.cores);
            levels.Clear();

            foreach (var entry in save.skillNodes ?? Array.Empty<SkillNodeSaveData>())
            {
                var node = FindNode(entry.id);
                if (node != null)
                    levels[node.Id] = Mathf.Clamp(entry.level, 0, node.MaxLevel);
            }

            GameEvents.CoresChanged?.Invoke(Cores);
        }

        public int AwardRun(bool won, int completedWave)
        {
            var award = Mathf.Max(1, completedWave) + (won ? 5 : 0);
            Cores += award;
            Save();
            GameEvents.CoresChanged?.Invoke(Cores);
            return award;
        }

        public int GetLevel(SkillNodeData node) =>
            node != null && levels.TryGetValue(node.Id, out var level) ? level : 0;

        public bool CanUpgrade(SkillNodeData node)
        {
            if (node == null || GetLevel(node) >= node.MaxLevel || Cores < node.Cost)
                return false;

            return node.Prerequisites == null ||
                   node.Prerequisites.All(prerequisite => GetLevel(prerequisite) > 0);
        }

        public bool CanRefund(SkillNodeData node)
        {
            if (node == null || GetLevel(node) <= 0)
                return false;

            return nodes.All(candidate =>
                candidate.Prerequisites == null ||
                !candidate.Prerequisites.Contains(node) ||
                GetLevel(candidate) == 0);
        }

        public bool TryUpgrade(SkillNodeData node)
        {
            if (!CanUpgrade(node))
                return false;

            Cores -= node.Cost;
            levels[node.Id] = GetLevel(node) + 1;
            NotifyChanged(node);
            return true;
        }

        public bool TryRefund(SkillNodeData node)
        {
            if (!CanRefund(node))
                return false;

            Cores += node.Cost;
            levels[node.Id] = GetLevel(node) - 1;
            NotifyChanged(node);
            return true;
        }

        public SkillNodeData FindNode(string id) =>
            nodes.Find(node => string.Equals(node.Id, id, StringComparison.Ordinal));

        public float GetEffectTotal(SkillEffectType effectType) =>
            nodes
                .Where(node => node.EffectType == effectType)
                .Sum(node => node.EffectPerLevel * GetLevel(node));

        private void NotifyChanged(SkillNodeData node)
        {
            Save();
            GameEvents.CoresChanged?.Invoke(Cores);
            GameEvents.SkillNodeChanged?.Invoke(node.Id, GetLevel(node));
        }

        private void Save()
        {
            SaveSystem.SaveMeta(new MetaSaveData
            {
                cores = Cores,
                skillNodes = nodes
                    .Select(node => new SkillNodeSaveData
                    {
                        id = node.Id,
                        level = GetLevel(node)
                    })
                    .ToArray()
            });
        }
    }
}
