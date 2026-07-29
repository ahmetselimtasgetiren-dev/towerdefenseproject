using UnityEngine;
using UnityEngine.UI;

namespace TowerDefenseIncremental.Rendering
{
    /// <summary>uGUI equivalent of FlatShadowDuplicator. The shadow is a sibling placed immediately behind the Image.</summary>
    [ExecuteAlways, DefaultExecutionOrder(1000), RequireComponent(typeof(Image))]
    public sealed class FlatShadowDuplicatorUI : MonoBehaviour
    {
        [SerializeField] private Vector2 shadowOffset = new(8f, -8f);
        [SerializeField, Range(0f, 1f)] private float darkenAmount = .35f;
        [SerializeField, HideInInspector] private FlatShadowChild shadowChild;

        private Image front;
        private Image shadow;
        private RectTransform frontRect;
        private RectTransform shadowRect;

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
            if (front == null) front = GetComponent<Image>();
            if (frontRect == null) frontRect = transform as RectTransform;
        }

        private void Synchronize()
        {
            CacheFront();
            // A sibling is required: a child Image would usually draw over its parent Image in a Canvas.
            if (front == null || frontRect == null || frontRect.parent == null || !gameObject.scene.IsValid()) return;
            EnsureShadow();
            if (shadow == null) return;

            shadowRect.SetParent(frontRect.parent, false);
            shadowRect.anchorMin = frontRect.anchorMin;
            shadowRect.anchorMax = frontRect.anchorMax;
            shadowRect.pivot = frontRect.pivot;
            shadowRect.sizeDelta = frontRect.sizeDelta;
            shadowRect.anchoredPosition = frontRect.anchoredPosition + shadowOffset;
            shadowRect.localRotation = frontRect.localRotation;
            shadowRect.localScale = frontRect.localScale;
            shadowRect.SetSiblingIndex(Mathf.Max(0, frontRect.GetSiblingIndex() - 1));

            shadow.sprite = front.sprite;
            shadow.type = front.type;
            shadow.preserveAspect = front.preserveAspect;
            shadow.fillCenter = front.fillCenter;
            shadow.fillMethod = front.fillMethod;
            shadow.fillAmount = front.fillAmount;
            shadow.fillClockwise = front.fillClockwise;
            shadow.fillOrigin = front.fillOrigin;
            shadow.pixelsPerUnitMultiplier = front.pixelsPerUnitMultiplier;
            shadow.color = FlatShadowUtility.DarkenValue(front.color, darkenAmount);
            shadow.material = front.material;
            shadow.maskable = front.maskable;
            shadow.raycastTarget = false;
            shadow.enabled = front.enabled;
        }

        private void EnsureShadow()
        {
            if (shadowChild == null)
            {
                foreach (var candidate in frontRect.parent.GetComponentsInChildren<FlatShadowChild>(true))
                    if (candidate.owner == this) { shadowChild = candidate; break; }
            }
            if (shadowChild == null)
            {
                var child = new GameObject($"{name} Flat Shadow", typeof(RectTransform));
                child.transform.SetParent(frontRect.parent, false);
                shadowChild = child.AddComponent<FlatShadowChild>();
                shadowChild.owner = this;
            }
            shadowRect = shadowChild.transform as RectTransform;
            shadow = shadowChild.GetComponent<Image>();
            if (shadow == null) shadow = shadowChild.gameObject.AddComponent<Image>();
        }
    }
}
