using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseIncremental
{
    [DisallowMultipleComponent]
    public sealed class CurrencyHUDPanel : MonoBehaviour
    {
        private readonly Dictionary<string, CurrencyRow> rows = new();
        private float nextY;

        public CurrencyRow AddRow(string currencyId, string displayName, Sprite icon, Color accent)
        {
            if (rows.TryGetValue(currencyId, out var existing))
                return existing;

            var rect = RuntimeUIFactory.CreateRect(
                $"{displayName} Row",
                transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, nextY),
                new Vector2(256f, 74f));
            var row = rect.gameObject.AddComponent<CurrencyRow>();
            row.Build(currencyId, displayName, icon, accent);
            rows.Add(currencyId, row);
            nextY -= 82f;
            return row;
        }

        public void SetValue(string currencyId, int amount, int? maximum = null)
        {
            if (rows.TryGetValue(currencyId, out var row))
                row.SetValue(amount, maximum);
        }
    }
}
