using LycheeLabs.FruityInterface.Elements;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class SpacerNode : LayoutNode {

        [SerializeField] private LayoutOrientation Orientation = LayoutOrientation.VERTICAL;
        [SerializeField] private float Size = 50;

        new private void OnValidate () {
            Size = Mathf.Max(Size, 0);
            RefreshLayoutDeferred();
        }

        public void SetSize (float newSize) {
            Size = Mathf.Max(newSize, 0);
            RefreshLayoutDeferred();
        }

        protected override void RefreshLayout () {
            if (Orientation == LayoutOrientation.VERTICAL) {
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.sizeDelta = new Vector2(0, Size);
                LayoutSizePixels = new Vector2(0, Size);
            } else {
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.sizeDelta = new Vector2(Size, 0);
                LayoutSizePixels = new Vector2(Size, 0);
            }
            LayoutPaddingPixels = default;
        }

    }

}