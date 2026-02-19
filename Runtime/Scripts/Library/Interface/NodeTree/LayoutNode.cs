using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface {

    [RequireComponent(typeof(RectTransform))]
    public class LayoutNode : InterfaceNode, ILayoutElement, ILayoutController {

        public Vector2 LayoutSizePixels;
        public Vector2 LayoutPaddingPixels;

        public Vector2 TotalSizePixels => LayoutSizePixels + LayoutPaddingPixels;
        public float TotalWidthPixels => (LayoutSizePixels.x + LayoutPaddingPixels.x) * transform.localScale.x;
        public float TotalHeightPixels => (LayoutSizePixels.y + LayoutPaddingPixels.y) * transform.localScale.y;

        private RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform ??= GetComponent<RectTransform>();
        
        public void OnValidate () {
            if (!rectTransform) return;
            RefreshLayoutDeferred();
        }

        public void RefreshLayoutDeferred () {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        // ILayoutElement API
        public float minWidth => TotalSizePixels.x;
        public float preferredWidth => TotalSizePixels.x;
        public float flexibleWidth => -1;

        public float minHeight => TotalSizePixels.y;
        public float preferredHeight => TotalSizePixels.y;
        public float flexibleHeight => -1;

        public int layoutPriority => 1;

        // Obsolete API
        public void CalculateLayoutInputHorizontal () { }
        public void CalculateLayoutInputVertical () { }

        public void SetLayoutHorizontal () { RefreshLayout(); }
        public void SetLayoutVertical () { RefreshLayout(); }

        protected virtual void RefreshLayout () { }

    }

}