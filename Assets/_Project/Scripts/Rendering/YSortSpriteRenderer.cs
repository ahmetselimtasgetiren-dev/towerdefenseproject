using UnityEngine;

namespace TowerDefenseIncremental.Rendering
{
    /// <summary>
    /// Optional front-renderer Y sort. FlatShadowDuplicator runs later and adds its relative offset to this result.
    /// Do not put this component on the generated "Flat Shadow" child.
    /// </summary>
    [DefaultExecutionOrder(0), RequireComponent(typeof(SpriteRenderer))]
    public sealed class YSortSpriteRenderer : MonoBehaviour
    {
        [SerializeField] private int baseSortingOrder;
        [SerializeField, Min(1)] private int ordersPerWorldUnit = 100;
        private SpriteRenderer spriteRenderer;

        private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();
        private void LateUpdate()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingOrder = baseSortingOrder - Mathf.RoundToInt(transform.position.y * ordersPerWorldUnit);
        }
    }
}
