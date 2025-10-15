using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class VerticalUILayout : UILayout {

        protected override void Layout () {
            float containedHeight = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }
                containedHeight += node.TotalHeightPixels;
            }

            float y = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }

                var newHeight = node.TotalHeightPixels;
                var shift = y - containedHeight / 2f + newHeight / 2f;
                var newPosition = new Vector3(0, -shift);
                node.rectTransform.SetAnchorAndPosition(newPosition);
                y += newHeight;
            }

            LayoutSizePixels = new Vector2(LayoutSizePixels.x, containedHeight);
            rectTransform.sizeDelta = TotalSizePixels;
        }

        public void SetWidth (float newWidth) {
            LayoutSizePixels = new Vector2(newWidth, LayoutSizePixels.y);
            rectTransform.sizeDelta = TotalSizePixels;
        }

    }

}