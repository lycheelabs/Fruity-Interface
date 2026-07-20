using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface {

    [RequireComponent(typeof(RectTransform))]
    public class LayoutNode : InterfaceNode, ILayoutController {

        public Vector2 LayoutSizePixels;
        public Vector2 LayoutPaddingPixels;

        public Vector2 TotalSizePixels => LayoutSizePixels + LayoutPaddingPixels;
        public float TotalWidthPixels => (LayoutSizePixels.x + LayoutPaddingPixels.x) * transform.localScale.x;
        public float TotalHeightPixels => (LayoutSizePixels.y + LayoutPaddingPixels.y) * transform.localScale.y;

        private RectTransform _rectTransform;
        public RectTransform rectTransform {
            get {
                if (_rectTransform == null)  _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
        
        public void OnValidate () {
            if (!rectTransform) return;
            RefreshLayoutDeferred();
        }

        public void RefreshLayoutDeferred () {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        // ILayoutController API
        public void SetLayoutHorizontal () { RefreshLayout(); }
        public void SetLayoutVertical () { RefreshLayout(); }
        protected virtual void RefreshLayout () { }

    }

}