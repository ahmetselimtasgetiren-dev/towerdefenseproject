using UnityEngine;
using UnityEngine.Tilemaps;

namespace TowerDefenseIncremental.Rendering
{
    /// <summary>
    /// Duplicates one complete Tilemap as an offset shadow layer. It creates one TilemapRenderer, never one object per tile.
    /// Use RefreshShadowTiles after changing a static path at runtime, or enable automatic refresh for dynamic tilemaps.
    /// </summary>
    [ExecuteAlways, DefaultExecutionOrder(1000), RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
    public sealed class FlatShadowDuplicatorTilemap : MonoBehaviour
    {
        [SerializeField] private Vector2 shadowOffset = new(.12f, -.12f);
        [SerializeField, Range(0f, 1f)] private float darkenAmount = .35f;
        [SerializeField] private int sortingOrderOffset = -1;
        [SerializeField] private bool refreshTilesEveryFrame;
        [SerializeField, HideInInspector] private FlatShadowChild shadowChild;

        private Tilemap frontMap;
        private TilemapRenderer frontRenderer;
        private Tilemap shadowMap;
        private TilemapRenderer shadowRenderer;

        private void Awake() => CacheFront();
        private void OnEnable() { RefreshShadowTiles(); SynchronizeRenderer(); }
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            RefreshShadowTiles();
            SynchronizeRenderer();
        }
        private void LateUpdate()
        {
            if (refreshTilesEveryFrame && Application.isPlaying) RefreshShadowTiles();
            SynchronizeRenderer();
        }
        private void OnDestroy()
        {
            if (shadowChild != null && shadowChild.owner == this)
                FlatShadowUtility.DestroyObject(shadowChild.gameObject);
        }

        [ContextMenu("Refresh Shadow Tiles")]
        public void RefreshShadowTiles()
        {
            CacheFront();
            if (frontMap == null || !gameObject.scene.IsValid()) return;
            EnsureShadow();
            if (shadowMap == null) return;
            shadowMap.ClearAllTiles();
            var bounds = frontMap.cellBounds;
            if (bounds.size.x > 0 && bounds.size.y > 0 && bounds.size.z > 0)
                shadowMap.SetTilesBlock(bounds, frontMap.GetTilesBlock(bounds));
            shadowMap.color = FlatShadowUtility.DarkenValue(frontMap.color, darkenAmount);
            shadowMap.tileAnchor = frontMap.tileAnchor;
            shadowMap.orientation = frontMap.orientation;
            shadowMap.orientationMatrix = frontMap.orientationMatrix;
            shadowMap.animationFrameRate = frontMap.animationFrameRate;
        }

        private void SynchronizeRenderer()
        {
            CacheFront();
            if (frontRenderer == null || !gameObject.scene.IsValid()) return;
            EnsureShadow();
            if (shadowRenderer == null) return;
            shadowChild.transform.localPosition = transform.localPosition + new Vector3(shadowOffset.x, shadowOffset.y, 0f);
            shadowChild.transform.localRotation = transform.localRotation;
            shadowChild.transform.localScale = transform.localScale;
            shadowRenderer.sharedMaterial = frontRenderer.sharedMaterial;
            shadowRenderer.sortingLayerID = frontRenderer.sortingLayerID;
            shadowRenderer.sortingOrder = frontRenderer.sortingOrder + sortingOrderOffset;
            shadowRenderer.enabled = frontRenderer.enabled;
        }

        private void CacheFront()
        {
            if (frontMap == null) frontMap = GetComponent<Tilemap>();
            if (frontRenderer == null) frontRenderer = GetComponent<TilemapRenderer>();
        }

        private void EnsureShadow()
        {
            if (shadowChild == null && transform.parent != null)
                foreach (var candidate in transform.parent.GetComponentsInChildren<FlatShadowChild>(true))
                    if (candidate.owner == this) { shadowChild = candidate; break; }
            if (shadowChild == null)
            {
                var child = new GameObject("Tilemap Flat Shadow");
                child.transform.SetParent(transform.parent, false);
                shadowChild = child.AddComponent<FlatShadowChild>();
                shadowChild.owner = this;
            }
            shadowMap = shadowChild.GetComponent<Tilemap>();
            if (shadowMap == null) shadowMap = shadowChild.gameObject.AddComponent<Tilemap>();
            shadowRenderer = shadowChild.GetComponent<TilemapRenderer>();
            if (shadowRenderer == null) shadowRenderer = shadowChild.gameObject.AddComponent<TilemapRenderer>();
        }
    }
}
