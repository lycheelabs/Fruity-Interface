using UnityEditor;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    [RequireComponent(typeof(RectTransform))]
    public class LayoutNode : InterfaceNode {

        public Vector2 LayoutSizePixels;
        public Vector2 LayoutPaddingPixels;

        public Vector2 TotalSizePixels => LayoutSizePixels + LayoutPaddingPixels;
        public float TotalWidthPixels => LayoutSizePixels.x + LayoutPaddingPixels.x;
        public float TotalHeightPixels => LayoutSizePixels.y + LayoutPaddingPixels.y;

        private RectTransform _rectTransform;
        public RectTransform rectTransform {
            get {
                if (this == null) return null;
                _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        /// <summary>
        /// Can be called safely during OnValidate to apply size changes to the prefab.
        /// </summary>
        protected void ApplySizeDeferred () {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                EditorApplication.delayCall += ApplySize;
            } else {
                ApplySize();
            }
#else
            ApplySize();
#endif
        }

        protected virtual void ApplySize () {
            if (this != null) {
                rectTransform.sizeDelta = LayoutSizePixels;
            }
        }

    }

}