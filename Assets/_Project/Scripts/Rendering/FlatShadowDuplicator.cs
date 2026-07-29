using UnityEngine;

namespace TowerDefenseIncremental.Rendering
{
    /// <summary>
    /// Creates a SpriteRenderer duplicate behind this renderer. ExecuteAlways gives artists a live Scene-view preview.
    /// The duplicate is synchronized in LateUpdate so its order follows a normal Y-sort component every frame.
    /// </summary>
    [ExecuteAlways, DefaultExecutionOrder(1000), RequireComponent(typeof(SpriteRenderer))]
    public sealed class FlatShadowDuplicator : MonoBehaviour
    {
        [SerializeField] private Vector2 shadowOffset = new(.12f, -.12f);
        [SerializeField, Range(0f, 1f)] private float darkenAmount = .35f;
        [Tooltip("Normally negative so the shadow remains behind its front renderer.")]
        [SerializeField] private int sortingOrderOffset = -1;
        [SerializeField, HideInInspector] private FlatShadowChild shadowChild;

        private SpriteRenderer front;
        private SpriteRenderer shadow;

        private void Awake() => CacheFront();
        private void OnEnable() => Synchronize();
        private void OnValidate() => Synchronize();
        private void LateUpdate() => Synchronize();

        private void OnDestroy()
        {
            if (shadowChild != null && shadowChild.owner == this)
                FlatShadowUtility.DestroyObject(shadowChild.gameObject);
        }

        private void CacheFront()
        {
            if (front == null) front = GetComponent<SpriteRenderer>();
        }

        private void Synchronize()
        {
            CacheFront();
            if (front == null || !gameObject.scene.IsValid()) return;
            EnsureShadow();
            if (shadow == null) return;

            shadow.transform.localPosition = new Vector3(shadowOffset.x, shadowOffset.y, 0f);
            shadow.transform.localRotation = Quaternion.identity;
            shadow.transform.localScale = Vector3.one;
            shadow.sprite = front.sprite;
            shadow.color = FlatShadowUtility.DarkenValue(front.color, darkenAmount);
            shadow.flipX = front.flipX;
            shadow.flipY = front.flipY;
            shadow.drawMode = front.drawMode;
            shadow.size = front.size;
            shadow.tileMode = front.tileMode;
            shadow.maskInteraction = front.maskInteraction;
            shadow.sharedMaterial = front.sharedMaterial;
            shadow.sortingLayerID = front.sortingLayerID;
            shadow.sortingOrder = front.sortingOrder + sortingOrderOffset;
            shadow.enabled = front.enabled;
        }

        private void EnsureShadow()
        {
            if (shadowChild == null)
            {
                foreach (var candidate in GetComponentsInChildren<FlatShadowChild>(true))
                    if (candidate.owner == this) { shadowChild = candidate; break; }
            }
            if (shadowChild == null)
            {
                var child = new GameObject("Flat Shadow");
                child.transform.SetParent(transform, false);
                shadowChild = child.AddComponent<FlatShadowChild>();
                shadowChild.owner = this;
            }
            shadow = shadowChild.GetComponent<SpriteRenderer>();
            if (shadow == null) shadow = shadowChild.gameObject.AddComponent<SpriteRenderer>();
        }
    }
}
