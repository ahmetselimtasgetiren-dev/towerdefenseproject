using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class RunHud : MonoBehaviour
    {
        private GameManager game;
        private EconomyManager economy;
        private MetaProgressionManager meta;
        private TowerPlacementManager placement;
        private GUIStyle title;
        private GUIStyle label;

        public void Initialize(GameManager owner, EconomyManager cash, MetaProgressionManager progression)
        {
            game = owner;
            economy = cash;
            meta = progression;
            placement = FindAnyObjectByType<TowerPlacementManager>();
        }

        private void OnGUI()
        {
            if (game == null) return;
            title ??= new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, normal = { textColor = new Color(.68f, .90f, 1f) } };
            label ??= new GUIStyle(GUI.skin.label) { fontSize = 15, normal = { textColor = new Color(.86f, .92f, 1f) } };
            GUI.Label(new Rect(18, 12, 600, 34), "AETHER", title);
            GUI.Label(new Rect(18, 48, 700, 26), $"Gold  {economy.Gold}     Home  {game.Lives}/{GameManager.StartingLives}     Wave  {game.Wave}/5     Cores  {meta.Cores}", label);
            GUI.Label(new Rect(18, 76, 900, 26), game.Message, label);
            if (game.State == RunState.Preparation && GUI.Button(new Rect(18, 108, 164, 28), game.Wave == 0 ? "Launch Wave 1" : "Launch Next Wave")) game.StartNextWave();
            if (game.State is RunState.Won or RunState.Lost)
            {
                GUI.Label(new Rect(18, 110, 300, 24), "Press R to restart.", label);
                return;
            }

            var selected = placement == null ? "Bolt Square (30)" : $"{placement.SelectedLabel} ({placement.SelectedCost})";
            GUI.Label(new Rect(200, 112, 760, 24), $"[1] Bolt  [2] Ember  [3] Prism     Selected: {selected}     Click to construct", label);
        }
    }
}
