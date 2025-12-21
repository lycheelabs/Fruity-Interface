using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(BoxCollider))]
    /// <summary>
    /// This box adjusts its rect size and collider size to fit the ContentsNode's size (with padding).
    /// </summary>
    public class PromptContainerNode : ContainerNode, ClickTarget {

        public LayoutNode ContentsNode;

        private BoxCollider _boxCollider;
        public BoxCollider boxCollider {
            get {
                _boxCollider = _boxCollider ?? GetComponent<BoxCollider>();
                return _boxCollider;
            }
        }

        public override bool InputIsDisabled => inputDisabled;
        private bool inputDisabled;

        public void LateUpdate () {
            Layout();
        }

        protected override void Layout () {
            if (ContentsNode != null) {
                LayoutSizePixels = ContentsNode.TotalSizePixels;
                var paddedSize = TotalSizePixels;
                if (rectTransform != null) rectTransform.sizeDelta = paddedSize;
                if (boxCollider != null) boxCollider.size = new Vector3(paddedSize.x, paddedSize.y, 1);
            }
        }

        public void SetInputDisabled(bool disabled) {
            inputDisabled = disabled;
        }

        public void MouseHovering(bool firstFrame, HighlightParams highlightParams) {}
        public void MouseHoverEnd() { }
        public void MouseClick(ClickParams clickParams) { }

    }

}