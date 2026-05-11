using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// This container adjusts its RectTransform size to match the contained LayoutContent's total size.
    /// </summary>
    [ExecuteAlways]
    public class SimpleContainerNode : ContainerNode, ClickTarget {

        [SerializeField] LayoutNode LayoutContents;

        private void OnValidate () {
            RefreshLayoutDeferred();
        }

        protected override void RefreshLayout () {
            if (LayoutContents != null) {
                LayoutSizePixels = LayoutContents.TotalSizePixels;
                rectTransform.sizeDelta = TotalSizePixels;
            }
        }

        public override bool InputIsDisabled => inputDisabled;
        private bool inputDisabled;

        public void SetInputDisabled (bool disabled) {
            inputDisabled = disabled;
        }

        public virtual void UpdateMouseHover (bool firstFrame, HoverParams highlightParams) { }
        public virtual void EndMouseHover () { }
        public virtual void ApplyMouseClick (ClickParams clickParams) { }

    }

}