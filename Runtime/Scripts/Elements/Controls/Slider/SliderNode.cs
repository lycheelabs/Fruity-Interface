using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface {

    public class SliderNode : LayoutNode, DragTarget {

        // Prefab config
        public RectTransform bounds;
        public RectTransform barBounds;
        public RectTransform bar;
        public BoxCollider barCollider;
        public Image handle;
        public Image barFill;
        
        public RectTransform counterBounds;
        public TextMeshProUGUI counter;

        // Public config
        public float width = 250f;
        public Color Color = Color.white;
        public bool showValue = true;
        public float valueTextWidth = 80f;
        public bool valueIsPercentage = true;

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
            handle.rectTransform.localScale = Vector3.one * (1 + 0.28f * highlightTween); 
            UpdateHandle();        
        }

        protected override void RefreshLayout() {
            var counterWidth = showValue ? valueTextWidth : 0f;
            counterBounds.gameObject.SetActive(showValue);
            counterBounds.sizeDelta = new Vector2(counterWidth, counterBounds.sizeDelta.y);

            barBounds.sizeDelta = new Vector2(width - counterWidth, barBounds.sizeDelta.y);

            var barSize = barBounds.rect.size;
            var handleSize = handle.rectTransform.rect.size;
            barCollider.size = new Vector3(barSize.x, handleSize.y, 1);

            bounds.sizeDelta = new Vector2(width, 60);
            LayoutSizePixels = new  Vector2(width, 60);

            handle.color = Color;
            barFill.color = Color;

            var progress = Progress(value);

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
            var handleHeight = handle.rectTransform.rect.height;
            return bar.rect.width - handleHeight * 0.8f;
        }

        private void AnimateTo(int newValue) {
            value = ClampValue(newValue);
            UpdateCounter();
            TryGetEffect?.OnValueChanged(value);
        }

        private void UpdateCounter () {
            if (counter != null) {
                counter.text = valueIsPercentage ? value + "%" : value.ToString();
            }
        }

        private void UpdateHandle () {
            if (handle != null) {
                handle.rectTransform.anchoredPosition = new Vector2((Progress(value) - 0.5f) * BarWidth(), 0);
            }
        }

    }

}
