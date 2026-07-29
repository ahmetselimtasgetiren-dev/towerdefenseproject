using System;
using UnityEngine;
namespace TowerDefenseIncremental
{
    public sealed class Health : MonoBehaviour
    {
        public int Current { get; private set; }
        public bool IsDead => Current <= 0;
        public event Action Died;
        public void Initialize(int maximum) => Current = maximum;
        public void Damage(int amount) { if (IsDead) return; Current = Mathf.Max(0, Current - amount); if (IsDead) Died?.Invoke(); }
    }
}
