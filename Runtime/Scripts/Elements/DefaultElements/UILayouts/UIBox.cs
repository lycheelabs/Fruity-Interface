using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(BoxCollider))]
    /// <summary>
    /// This box adjusts its rect size and collider size to fit the ContentsNode's size (with padding).
    /// </summary>
    public class UIBox : LayoutNode, ClickTarget {

        public LayoutNode ContentsNode;
        public float Padding = 12f;

        private BoxCollider _boxCollider;
        public BoxCollider boxCollider {
            get {
                _boxCollider = _boxCollider ?? GetComponent<BoxCollider>();
                return _boxCollider;
            }
        }

        public void Update() {
            if (ContentsNode != null) {
                var padSize = Padding + 5;
                var size = ContentsNode.TotalSizePixels + new Vector2(padSize, padSize);
                if (rectTransform != null) rectTransform.sizeDelta = size;
                if (boxCollider != null) boxCollider.size = new Vector3(size.x, size.y, 1);
            }
        }

        public override bool InputIsDisabled => disabled;
        private bool disabled = false;
        public void SetInputDisabled(bool disabled) {
            this.disabled = disabled;
        }

        public void MouseHovering(bool firstFrame, HighlightParams highlightParams) {}
        public void MouseHoverEnd() { }
        public void MouseClick(ClickParams clickParams) { }

    }

}