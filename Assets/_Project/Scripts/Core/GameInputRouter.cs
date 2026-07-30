using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefenseIncremental
{
    public sealed class GameInputRouter : MonoBehaviour
    {
        public event Action StartWavePressed;
        public event Action RestartPressed;
        public event Action<int> TowerSelectionRequested;
        public event Action<Vector2> TowerPlacementRequested;

        private InputActionMap gameplay;
        private InputAction startWave;
        private InputAction restart;
        private InputAction selectTower1;
        private InputAction selectTower2;
        private InputAction selectTower3;
        private InputAction placeTower;
        private InputAction point;

        public void Initialize(InputActionAsset actions)
        {
            if (actions == null)
            {
                Debug.LogError("InputSystem_Actions asset is missing.");
                return;
            }

            gameplay = actions.FindActionMap("Gameplay", true);
            startWave = gameplay.FindAction("StartWave", true);
            restart = gameplay.FindAction("Restart", true);
            selectTower1 = gameplay.FindAction("SelectTower1", true);
            selectTower2 = gameplay.FindAction("SelectTower2", true);
            selectTower3 = gameplay.FindAction("SelectTower3", true);
            placeTower = gameplay.FindAction("PlaceTower", true);
            point = gameplay.FindAction("Point", true);

            startWave.performed += OnStartWave;
            restart.performed += OnRestart;
            selectTower1.performed += OnSelectTower1;
            selectTower2.performed += OnSelectTower2;
            selectTower3.performed += OnSelectTower3;
            placeTower.performed += OnPlaceTower;
            gameplay.Enable();
        }

        private void OnDestroy()
        {
            if (gameplay == null)
                return;

            startWave.performed -= OnStartWave;
            restart.performed -= OnRestart;
            selectTower1.performed -= OnSelectTower1;
            selectTower2.performed -= OnSelectTower2;
            selectTower3.performed -= OnSelectTower3;
            placeTower.performed -= OnPlaceTower;
            gameplay.Disable();
        }

        private void OnStartWave(InputAction.CallbackContext _) => StartWavePressed?.Invoke();
        private void OnRestart(InputAction.CallbackContext _) => RestartPressed?.Invoke();
        private void OnSelectTower1(InputAction.CallbackContext _) => TowerSelectionRequested?.Invoke(0);
        private void OnSelectTower2(InputAction.CallbackContext _) => TowerSelectionRequested?.Invoke(1);
        private void OnSelectTower3(InputAction.CallbackContext _) => TowerSelectionRequested?.Invoke(2);
        private void OnPlaceTower(InputAction.CallbackContext _) => TowerPlacementRequested?.Invoke(point.ReadValue<Vector2>());
    }
}
