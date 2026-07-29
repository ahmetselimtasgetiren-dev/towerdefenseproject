using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class EconomyManager : MonoBehaviour
    {
        public int Gold { get; private set; }

        public void ResetGold(int value)
        {
            Gold = value;
            GameEvents.GoldChanged?.Invoke(Gold);
        }

        public void Add(int value)
        {
            Gold += value;
            GameEvents.GoldChanged?.Invoke(Gold);
        }

        public bool TrySpend(int value)
        {
            if (Gold < value) return false;
            Gold -= value;
            GameEvents.GoldChanged?.Invoke(Gold);
            return true;
        }
    }
}
