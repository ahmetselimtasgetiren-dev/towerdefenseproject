using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TowerDefenseIncremental
{
    public sealed class RunHud : MonoBehaviour
    {
        private const string CrystalsCurrency = "crystals";
        private const string GemsCurrency = "gems";

        private GameManager game;
        private EconomyManager economy;
        private MetaProgressionManager meta;
        private TowerPlacementManager placement;
        private CurrencyHUDPanel currencies;
        private TextMeshProUGUI runStatus;
        private TextMeshProUGUI message;
        private TextMeshProUGUI selection;
        private TextMeshProUGUI launchLabel;
        private Button launchButton;
        private GameObject skillTreeRoot;
        private SkillTreeController skillTree;

        public void Initialize(GameManager owner, EconomyManager cash, MetaProgressionManager progression)
        {
            game = owner;
            economy = cash;
            meta = progression;
            placement = FindAnyObjectByType<TowerPlacementManager>();

            EnsureEventSystem();
            BuildCanvas();
            Subscribe();
            Refresh();
        }

        private void OnDestroy()
        {
            GameEvents.GoldChanged -= OnGoldChanged;
            GameEvents.LivesChanged -= OnLivesChanged;
            GameEvents.WaveStarted -= OnWaveChanged;
            GameEvents.WaveCompleted -= OnWaveChanged;
            GameEvents.MessageChanged -= OnMessageChanged;
            GameEvents.RunStateChanged -= OnRunStateChanged;
            GameEvents.CoresChanged -= OnCoresChanged;
        }

        private void Subscribe()
        {
            GameEvents.GoldChanged += OnGoldChanged;
            GameEvents.LivesChanged += OnLivesChanged;
            GameEvents.WaveStarted += OnWaveChanged;
            GameEvents.WaveCompleted += OnWaveChanged;
            GameEvents.MessageChanged += OnMessageChanged;
            GameEvents.RunStateChanged += OnRunStateChanged;
            GameEvents.CoresChanged += OnCoresChanged;
        }

        private void BuildCanvas()
        {
            var assets = Resources.Load<UIAssetCatalog>("UIAssetCatalog");
            var canvasObject = new GameObject("Aether Run HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1f;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var currencyRect = RuntimeUIFactory.CreateRect(
                "Currency HUD",
                canvasObject.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(24f, -24f),
                new Vector2(256f, 156f));
            currencies = currencyRect.gameObject.AddComponent<CurrencyHUDPanel>();
            currencies.AddRow(CrystalsCurrency, "Crystals", assets != null ? assets.CrystalsIcon : null, new Color(0.43f, 0.88f, 0f));
            currencies.AddRow(GemsCurrency, "Gems", assets != null ? assets.GemsIcon : null, new Color(1f, 0.33f, 0.33f));

            var statusPanel = RuntimeUIFactory.CreateImage(
                "Run Status Panel",
                canvasObject.transform,
                assets != null ? assets.PanelFrame : null,
                assets != null && assets.PanelFrame != null ? Color.white : new Color(0.125f, 0.125f, 0.122f, 0.96f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -24f),
                new Vector2(760f, 150f));
            statusPanel.raycastTarget = false;

            RuntimeUIFactory.CreateText(
                "Title",
                statusPanel.transform,
                "AETHER",
                30f,
                new Color(0.48f, 0.96f, 1f),
                TextAlignmentOptions.Top,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -20f),
                new Vector2(-40f, 40f));

            runStatus = RuntimeUIFactory.CreateText(
                "Run Status",
                statusPanel.transform,
                string.Empty,
                17f,
                new Color(0.89f, 0.75f, 0.73f),
                TextAlignmentOptions.Center,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -7f),
                new Vector2(-48f, -64f));

            message = RuntimeUIFactory.CreateText(
                "Message",
                canvasObject.transform,
                string.Empty,
                16f,
                new Color(0.89f, 0.92f, 1f),
                TextAlignmentOptions.Center,
                new Vector2(0.2f, 0f),
                new Vector2(0.8f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 118f),
                new Vector2(0f, 52f));

            selection = RuntimeUIFactory.CreateText(
                "Tower Selection",
                canvasObject.transform,
                string.Empty,
                14f,
                new Color(0.89f, 0.75f, 0.73f),
                TextAlignmentOptions.Left,
                new Vector2(0f, 0f),
                new Vector2(0.64f, 0f),
                new Vector2(0f, 0f),
                new Vector2(24f, 28f),
                new Vector2(-24f, 70f));

            launchButton = RuntimeUIFactory.CreateButton(
                "Launch Wave",
                canvasObject.transform,
                assets != null ? assets.StartButton : null,
                new Color(1f, 0.70f, 0.68f),
                "LAUNCH WAVE",
                19f,
                game.StartNextWave,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-24f, 24f),
                new Vector2(437f, 74f));
            launchLabel = launchButton.GetComponentInChildren<TextMeshProUGUI>();

            RuntimeUIFactory.CreateButton(
                "Skill Tree",
                canvasObject.transform,
                assets != null ? assets.RefundButton : null,
                new Color(0.48f, 0.96f, 1f),
                "UPGRADE TREE",
                13f,
                ToggleSkillTree,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-24f, -24f),
                new Vector2(260f, 58f));

            var skillTreeRect = RuntimeUIFactory.CreateRect(
                "Skill Tree Overlay",
                canvasObject.transform,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
            skillTreeRoot = skillTreeRect.gameObject;
            skillTree = skillTreeRoot.AddComponent<SkillTreeController>();
            skillTree.Initialize(meta, assets);

            RuntimeUIFactory.CreateButton(
                "Close",
                skillTreeRoot.transform,
                assets != null ? assets.RefundButton : null,
                new Color(1f, 0.70f, 0.68f),
                "CLOSE",
                13f,
                ToggleSkillTree,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-30f, -24f),
                new Vector2(180f, 54f));
            skillTreeRoot.SetActive(false);
        }

        private void ToggleSkillTree()
        {
            var show = !skillTreeRoot.activeSelf;
            skillTreeRoot.SetActive(show);
            if (show)
                skillTree.Refresh();
        }

        private void Refresh()
        {
            OnGoldChanged(economy.Gold);
            OnCoresChanged(meta.Cores);
            OnMessageChanged(game.Message);
            RefreshState();
        }

        private void RefreshState()
        {
            runStatus.text = $"HOME  {game.Lives:00}/{GameManager.StartingLives:00}     WAVE  {game.Wave:00}";
            selection.text = placement == null
                ? "[1] BOLT   [2] EMBER   [3] PRISM"
                : $"[1] BOLT   [2] EMBER   [3] PRISM     SELECTED: {placement.SelectedLabel.ToUpperInvariant()} ({placement.SelectedCost})";

            launchButton.gameObject.SetActive(game.State == RunState.Preparation);
            launchButton.interactable = game.State == RunState.Preparation;
            launchLabel.text = game.Wave == 0 ? "LAUNCH WAVE 1" : "LAUNCH NEXT WAVE";
        }

        private void OnGoldChanged(int amount) => currencies.SetValue(CrystalsCurrency, amount);
        private void OnCoresChanged(int amount) => currencies.SetValue(GemsCurrency, amount);
        private void OnLivesChanged(int _) => RefreshState();
        private void OnWaveChanged(int _) => RefreshState();
        private void OnMessageChanged(string value) => message.text = value ?? string.Empty;
        private void OnRunStateChanged(RunState _) => RefreshState();

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
                return;

            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystemObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }
    }
}
