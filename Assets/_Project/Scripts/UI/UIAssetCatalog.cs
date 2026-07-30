using UnityEngine;

namespace TowerDefenseIncremental
{
    [CreateAssetMenu(menuName = "Aether/UI Asset Catalog", fileName = "UIAssetCatalog")]
    public sealed class UIAssetCatalog : ScriptableObject
    {
        [Header("Currency")]
        [SerializeField] private Sprite gemsIcon;
        [SerializeField] private Sprite scrapIcon;
        [SerializeField] private Sprite crystalsIcon;

        [Header("Skill Tree")]
        [SerializeField] private Sprite nodeFrameBase;
        [SerializeField] private Sprite nodeGlowRing;
        [SerializeField] private Sprite nodeLocked;

        [Header("Chrome")]
        [SerializeField] private Sprite startButton;
        [SerializeField] private Sprite upgradeButton;
        [SerializeField] private Sprite refundButton;
        [SerializeField] private Sprite panelFrame;

        public Sprite GemsIcon => gemsIcon;
        public Sprite ScrapIcon => scrapIcon;
        public Sprite CrystalsIcon => crystalsIcon;
        public Sprite NodeFrameBase => nodeFrameBase;
        public Sprite NodeGlowRing => nodeGlowRing;
        public Sprite NodeLocked => nodeLocked;
        public Sprite StartButton => startButton;
        public Sprite UpgradeButton => upgradeButton;
        public Sprite RefundButton => refundButton;
        public Sprite PanelFrame => panelFrame;
    }
}
