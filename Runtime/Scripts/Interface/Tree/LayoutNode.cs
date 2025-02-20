
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    [RequireComponent(typeof(RectTransform))]
    public abstract class LayoutNode : InterfaceNode {

        public Vector2 LayoutSizePixels;
        public Vector2 LayoutPaddingPixels;

        public Vector2 TotalSizePixels => LayoutSizePixels + LayoutPaddingPixels;
        public float TotalWidthPixels => LayoutSizePixels.x + LayoutPaddingPixels.x;
        public float TotalHeightPixels => LayoutSizePixels.y + LayoutPaddingPixels.y;

        private RectTransform _rectTransform;
        public RectTransform rectTransform {
            get {
                _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

    }

}