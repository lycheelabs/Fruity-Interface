using TMPro;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class SliderNode : LayoutNode, DragTarget {

        // Prefab config
        public RectTransform barBounds;
        public RectTransform bar;
        public BoxCollider barCollider;
        public RectTransform handle;
        
        public RectTransform counterBounds;
        public TextMeshProUGUI counter;

        // Public config
        public bool valueIsPercentage = true;
        public bool showValue = true;
        public float valueTextWidth = 80f;

        // Value
        private int min = 0;
        private int max = 100;
        private int value = 50;

        // Animation
        private bool isHighlighted;
        private float highlightTween;

        // Effect
        private SliderEffect _effect;
        private SliderEffect TryGetEffect => _effect ??= GetComponent<SliderEffect>();

        void Update() {
            highlightTween = highlightTween.MoveTowardsUnscaled(isHighlighted, 10);
            handle.localScale = Vector3.one * (1 + 0.2f * highlightTween);         
        }

        protected override void RefreshLayout() {
            var counterWidth = showValue ? valueTextWidth : 0f;
            counterBounds.gameObject.SetActive(showValue);
            counterBounds.sizeDelta = new Vector2(counterWidth, counterBounds.sizeDelta.y);

            var totalWidth = LayoutSizePixels.x;
            barBounds.sizeDelta = new Vector2(totalWidth - counterWidth, barBounds.sizeDelta.y);

            var barSize = barBounds.rect.size;
            var handleSize = handle.rect.size;
            barCollider.size = new Vector3(barSize.x, handleSize.y, 1);

            UpdateCounter();
        }

        public void Configure (int min, int max, int value) {
            this.min = min;
            this.max = max;
            SetValueSilent(value);
        }

        public void SetValueSilent (int newValue) {
            value = ClampValue(newValue);
            var progress = Progress(value);
            handle.anchoredPosition = new Vector2((progress - 0.5f) * BarWidth(), 0);
            UpdateCounter();
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
            var width = BarWidth();
            if (width <= 0) return;

            var local = bar.transform.InverseTransformPoint(dragParams.MouseWorldPosition);
            var progress = Mathf.Clamp01(local.x / width + 0.5f);
            var intValue = Mathf.RoundToInt(Mathf.Lerp(min, max, progress));
            AnimateTo(intValue);
        }

        public void ApplyMouseDrag(DragParams dragParams) {
            var width = BarWidth();
            if (width <= 0) return;

            var local = bar.transform.InverseTransformPoint(dragParams.MouseWorldPosition);
            var progress = Mathf.Clamp01(local.x / width + 0.5f);
            var intValue = Mathf.RoundToInt(Mathf.Lerp(min, max, progress));
            AnimateTo(intValue);
        }

        public void CancelMouseDrag() { }

        private int ClampValue (int v) {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private float Progress (int v) {
            if (max <= min) return 0;
            return Mathf.Clamp01((v - min) / (float)(max - min));
        }

        private float BarWidth() {
            var handleHeight = handle.rect.height;
            return bar.rect.width - handleHeight * 0.8f;
        }

        private void AnimateTo(int newValue) {
            value = ClampValue(newValue);
            var progress = Progress(value);
            handle.anchoredPosition = new Vector2((progress - 0.5f) * BarWidth(), 0);
            UpdateCounter();
            TryGetEffect?.OnValueChanged(value);
        }

        private void UpdateCounter () {
            if (counter != null) {
                counter.text = valueIsPercentage ? value + "%" : value.ToString();
            }
        }

    }

}
