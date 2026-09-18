using UnityEngine;

namespace LycheeLabs.FruityInterface {

    [ExecuteAlways]
    public class HeaderNode : LayoutNode {

        [SerializeField, Min(0)] private float size = 106f;
        [SerializeField] private RectTransform content;

        public float Size {
            get => size;
            set {
                size = Mathf.Max(value, 0);
                RefreshLayoutDeferred();
            }
        }

        protected override void RefreshLayout() {
            rectTransform.anchorMin = new Vector2(0, 0.5f);
            rectTransform.anchorMax = new Vector2(1, 0.5f);
            rectTransform.sizeDelta = new Vector2(0, size);
            LayoutSizePixels = new Vector2(0, size);
            LayoutPaddingPixels = default;

            if (content == null) {
                return;
            }

            content.anchorMin = Vector2.zero;
            content.anchorMax = Vector2.one;
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;
        }

        new private void OnValidate() {
            size = Mathf.Max(size, 0);
            RefreshLayoutDeferred();
        }

    }

}
