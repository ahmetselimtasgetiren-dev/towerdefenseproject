using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseIncremental
{
    [DisallowMultipleComponent]
    public sealed class GraphPanZoom : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private float zoomSpeed = 0.12f;
        [SerializeField] private Vector2 zoomLimits = new(0.65f, 1.6f);

        private Vector2 dragStart;
        private Vector2 contentStart;

        public void Initialize(RectTransform graphContent) => content = graphContent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (content == null)
                return;

            dragStart = eventData.position;
            contentStart = content.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (content == null)
                return;

            content.anchoredPosition = contentStart + (eventData.position - dragStart);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (content == null)
                return;

            var next = Mathf.Clamp(content.localScale.x + Mathf.Sign(eventData.scrollDelta.y) * zoomSpeed, zoomLimits.x, zoomLimits.y);
            content.localScale = Vector3.one * next;
        }
    }
}
