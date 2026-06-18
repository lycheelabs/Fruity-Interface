using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class SliderNode : LayoutNode, DragTarget {

        public RectTransform barBounds;
        public BoxCollider barCollider;
        public RectTransform handle;

        private bool isHighlighted;
        private float highlightTween;

        private float value;

        void Start() {
            
        }

        void Update() {
            highlightTween = highlightTween.MoveTowardsUnscaled(isHighlighted, 10);
            handle.localScale = Vector3.one * (1 + 0.2f * highlightTween);

            var barSize = barBounds.rect.size;
            var handleSize = handle.rect.size;
            barCollider.size = new Vector3(barSize.x, handleSize.y, 1);
        }

        public void UpdateMouseHover(bool isFirstFrame, HoverParams highlightParams) {
            isHighlighted = true;
        }

        public void EndMouseHover() {
            isHighlighted = false;
        }

        public MouseDragMode GetDragMode(MouseButton dragButton) {
            return dragButton == MouseButton.Left ? MouseDragMode.DragOnly : MouseDragMode.Disabled;
        }

        public void UpdateMouseDragging(bool isFirstFrame, DragParams dragParams) {
            var local = barBounds.transform.InverseTransformPoint(dragParams.MouseWorldPosition);
            var progress = Mathf.Clamp01(local.x / BarWidth() + 0.5f);
            AnimateTo(progress);
        }

        public void ApplyMouseDrag(DragParams dragParams) {
            var local = barBounds.transform.InverseTransformPoint(dragParams.MouseWorldPosition);
            var progress = Mathf.Clamp01(local.x / BarWidth() + 0.5f);
            AnimateTo(progress);
        }

        public void CancelMouseDrag() { }

        private float BarWidth() {
            var handleHeight = handle.rect.height;
            return barBounds.rect.width - handleHeight * 0.8f;
        }

        private void AnimateTo(float progress) {
            value = progress;
            var xOffset = (progress - 0.5f) * BarWidth();
            handle.anchoredPosition = new Vector2(xOffset, 0);
        }

    }

}
