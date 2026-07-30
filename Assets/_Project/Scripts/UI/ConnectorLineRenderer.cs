using UnityEngine;
using UnityEngine.UI;

namespace TowerDefenseIncremental
{
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class ConnectorLineRenderer : MonoBehaviour
    {
        [SerializeField] private RectTransform start;
        [SerializeField] private RectTransform end;
        [SerializeField] private float thickness = 4f;

        private RectTransform lineRect;

        public void Initialize(RectTransform startNode, RectTransform endNode, Color color, float lineThickness = 4f)
        {
            start = startNode;
            end = endNode;
            thickness = lineThickness;
            var image = GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            Refresh();
        }

        private void Awake() => lineRect = transform as RectTransform;
        private void OnEnable() => Refresh();
        private void LateUpdate() => Refresh();

        private void Refresh()
        {
            if (start == null || end == null)
                return;

            lineRect ??= transform as RectTransform;
            var startPoint = start.anchoredPosition;
            var endPoint = end.anchoredPosition;
            var delta = endPoint - startPoint;
            lineRect.anchoredPosition = (startPoint + endPoint) * 0.5f;
            lineRect.sizeDelta = new Vector2(delta.magnitude, thickness);
            lineRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }
    }
}
