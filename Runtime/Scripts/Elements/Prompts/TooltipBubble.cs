using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    public class TooltipBubble : InterfaceNode {

        // Components
        public RectTransform root;
        public Image backing;
        public Image arrow;
        public LayoutNode contentsNode;

        // Graphics config
        public int arrowOffset = 1; // Shifts the arrow relative to the backing perimeter
        public int arrowLength = 32; // Shifts the whole bubble to make space for the arrow
        public int arrowClearance = 32; // Affects how close the arrow can shift laterally towards bubble edges
        public float lerpSpeed = 1;
        private float screenEdgePadding = 10;

        // ---------------------------------------

        // Entry
        private bool active;
        private float activeTween;

        // Size
        private Vector2 rectSize;
        private float scale;

        // Position
        private Vector3 screenPosition;
        private WorldAnchor basePosition;
        private Direction offsetDirection;


        private void Awake () {
            root.transform.localScale = Vector3.zero;
        }

        private void LateUpdate() {
            activeTween = activeTween.MoveTowards(active, 8 * lerpSpeed);
            RefreshSize();
            RefreshPosition();
        }

        private void OnValidate() {
            scale = 1;
            activeTween = 1;
            RefreshSize();

            arrow.rectTransform.anchoredPosition = new Vector3(rectSize.x / 2f - arrowOffset, 0, 0);
        }

        public void Show(WorldAnchor position, Direction offsetDirection, float scale = 1f) {
            active = true;

            SetArrowDirection(offsetDirection.Reverse());
            SetPosition(position, offsetDirection, scale * 0.9f);
        }

        public void Show () {
            active = true;
        }

        public void Hide() {
            active = false;
        }

        public void SetBubbleShift (Vector2 shift) {
            backing.rectTransform.anchoredPosition = shift;
            //backing.rectTransform.anchoredPosition = new Vector3(0, Mathf.Sin(Time.time * 1.4f)) * 1.2f;
        }

        // ---------------------------------------------------------------------------------------------

        private void SetArrowDirection(Direction direction) {
            arrow.enabled = direction != Direction.NONE;
            arrow.rectTransform.localEulerAngles = new Vector3(0, 0, direction.Angle());

            arrow.rectTransform.anchoredPosition = new Vector2(
                (rectSize.x / 2f - arrowOffset) * direction.XIndex(), 
                (rectSize.y / 2f - arrowOffset) * direction.ZIndex()
            );
        }

        private void SetPosition (WorldAnchor newAnchor, Direction newOffsetDirection, float newScale) {
            SetPosition(newAnchor.ScreenVector(), newOffsetDirection, newScale);
        }

        private void SetPosition(Vector3 newPosition, Direction newOffsetDirection, float newScale) {
            offsetDirection = newOffsetDirection;
            scale = newScale;

            screenPosition = newPosition;
            var offset = rectSize / 2f + new Vector2(arrowLength, arrowLength);
            screenPosition.x += offset.x * newOffsetDirection.XIndex() * newScale;
            screenPosition.y += offset.y * newOffsetDirection.ZIndex() * newScale;

            RefreshPosition();
        }

        private void RefreshSize () {
            if (contentsNode != null) {
                rectSize = contentsNode.TotalSizePixels;
            } else {
                rectSize = new Vector2(100, 100);
            }
            root.sizeDelta = rectSize;
            root.transform.localScale = Vector3.one * Tweens.EaseOutQuad(activeTween) * scale;
        }

        private void RefreshPosition () {
            var clampedPosition = screenPosition;
            var size = (rectSize + new Vector2(screenEdgePadding, screenEdgePadding));
            var screenSize = FruityUI.ScreenBounds.BoxedCanvasSize;
            var maxOffset = (screenSize - size * scale) / 2f;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -maxOffset.x, maxOffset.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -maxOffset.y, maxOffset.y);
            root.anchoredPosition = clampedPosition;

            if (offsetDirection != Direction.NONE && activeTween > 0) {
                var shift = clampedPosition - screenPosition;
                var arrowDir = offsetDirection.Reverse();
                var arrowVector = arrowDir.ToVector2();
                var perpVector = arrowDir.IsHorizontal() ? new Vector2(0, 1) : new Vector2(1, 0);
                var perpShift = shift * perpVector;

                // Calculate offset adjustment
                var arrowAdjust = -perpVector * (perpShift / scale);
                var maxAdjust = (rectSize - new Vector2(arrowClearance, arrowClearance)) / 2f;
                maxAdjust.x = Mathf.Max(maxAdjust.x, 0);
                maxAdjust.y = Mathf.Max(maxAdjust.y, 0);
                var unclampedAdjust = arrowAdjust;
                arrowAdjust.x = Mathf.Clamp(arrowAdjust.x, -maxAdjust.x, maxAdjust.x);
                arrowAdjust.y = Mathf.Clamp(arrowAdjust.y, -maxAdjust.y, maxAdjust.y);
                var adjustOverflow = (unclampedAdjust - arrowAdjust) * perpVector;

                arrow.rectTransform.anchoredPosition = new Vector2(
                    (rectSize.x / 2f - arrowOffset) * arrowDir.XIndex() + arrowAdjust.x,
                    (rectSize.y / 2f - arrowOffset) * arrowDir.ZIndex() + arrowAdjust.y
                );

                // Calculate scale adjustment
                var parallelShift = Vector2.Dot(shift, arrowVector);
                var arrowScale = Mathf.Clamp01(1f - (Mathf.Abs(parallelShift) + adjustOverflow.magnitude) / arrowLength);

                arrow.rectTransform.localScale = Vector3.one * arrowScale;
            }
        }

    }

}