using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    public class TooltipBubble : LayoutNode {

        // Components
        public RectTransform root;
        public Image backing;
        public Image arrow;
        public LayoutNode contentsNode;

        // Graphics config
        public float arrowOffset = 1; // Shifts the arrow relative to the backing perimeter
        public float arrowLength = 32; // Shifts the whole bubble to make space for the arrow
        public float arrowClearance = 32; // Affects how close the arrow can shift laterally towards bubble edges
        public float lerpSpeed = 1;
        public float MinimumSize = 60;
        private float screenEdgePadding = 10;

        // ---------------------------------------

        // Entry
        private bool active;
        private float activeTween;
        private bool overrideHidden;
        private float overrideHiddenTween;

        // Size
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
            overrideHiddenTween = overrideHiddenTween.MoveTowards(overrideHidden, 8 * lerpSpeed);
            RefreshSize();
            RefreshPosition();
        }

        protected override void RefreshLayout() {
            scale = 1;
            if (!Application.isPlaying) {
                activeTween = 1;
            }
            RefreshSize();
            if (!Application.isPlaying) {
                arrow.rectTransform.anchoredPosition = new Vector3(
                    TotalSizePixels.x / 2f - arrowOffset, 0, 0
                );
            }
        }

        public void Show(WorldAnchor position, Direction offsetDirection, float scale = 1f) {
            BeginShow();

            SetArrowDirection(offsetDirection.Reverse());
            SetPosition(position, offsetDirection, scale * 0.9f);
        }

        public void Show () {
            BeginShow();
        }

        public void Hide() {
            active = false;
        }

        public void OverrideHidden(bool hidden) {
            overrideHidden = hidden;
        }

        private void BeginShow() {
            if (!active) {
                activeTween = 0;
            }
            active = true;
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
                (TotalSizePixels.x / 2f - arrowOffset) * direction.XIndex(), 
                (TotalSizePixels.y / 2f - arrowOffset) * direction.ZIndex()
            );
        }

        private void SetPosition (WorldAnchor newAnchor, Direction newOffsetDirection, float newScale) {
            SetPosition(newAnchor.ScreenVector(), newOffsetDirection, newScale);
        }

        private void SetPosition(Vector3 newPosition, Direction newOffsetDirection, float newScale) {
            offsetDirection = newOffsetDirection;
            scale = newScale;

            screenPosition = newPosition;
            var offset = TotalSizePixels / 2f + new Vector2(arrowLength, arrowLength);
            screenPosition.x += offset.x * newOffsetDirection.XIndex() * newScale;
            screenPosition.y += offset.y * newOffsetDirection.ZIndex() * newScale;

            RefreshPosition();
        }

        private void RefreshSize () {
            if (contentsNode != null) {
                LayoutSizePixels = contentsNode.TotalSizePixels;
            }
            LayoutSizePixels.x = Mathf.Max(LayoutSizePixels.x, MinimumSize);
            LayoutSizePixels.y = Mathf.Max(LayoutSizePixels.y, MinimumSize);
            root.sizeDelta = TotalSizePixels;
            root.transform.localScale = Vector3.one * Tweens.EaseOutQuad(activeTween - overrideHiddenTween) * scale;
        }

        private void RefreshPosition () {
            var clampedPosition = screenPosition;
            var size = (TotalSizePixels + new Vector2(screenEdgePadding, screenEdgePadding));
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
                var maxAdjust = (TotalSizePixels - new Vector2(arrowClearance, arrowClearance)) / 2f;
                maxAdjust.x = Mathf.Max(maxAdjust.x, 0);
                maxAdjust.y = Mathf.Max(maxAdjust.y, 0);
                var unclampedAdjust = arrowAdjust;
                arrowAdjust.x = Mathf.Clamp(arrowAdjust.x, -maxAdjust.x, maxAdjust.x);
                arrowAdjust.y = Mathf.Clamp(arrowAdjust.y, -maxAdjust.y, maxAdjust.y);
                var adjustOverflow = (unclampedAdjust - arrowAdjust) * perpVector;

                arrow.rectTransform.anchoredPosition = new Vector2(
                    (TotalSizePixels.x / 2f - arrowOffset) * arrowDir.XIndex() + arrowAdjust.x,
                    (TotalSizePixels.y / 2f - arrowOffset) * arrowDir.ZIndex() + arrowAdjust.y
                );

                // Calculate scale adjustment
                var parallelShift = Vector2.Dot(shift, arrowVector);
                var arrowScale = Mathf.Clamp01(1f - (Mathf.Abs(parallelShift) + adjustOverflow.magnitude) / arrowLength);

                arrow.rectTransform.localScale = Vector3.one * arrowScale;
            }
        }

    }

}
