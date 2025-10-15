using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class HorizontalUILayout : UILayout {

        protected override void Layout () {
            float containedWidth = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }
                containedWidth += node.TotalWidthPixels;
            }

            float x = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }

                var newWidth = node.TotalWidthPixels;
                var shift = x - containedWidth / 2f + newWidth / 2f;
                var newPosition = new Vector3(shift, 0);
                node.rectTransform.SetAnchorAndPosition(newPosition);
                x += newWidth;
            }

            LayoutSizePixels = new Vector2(containedWidth, LayoutSizePixels.y);
            rectTransform.sizeDelta = TotalSizePixels;
        }

        public void SetHeight (float newHeight) {
            LayoutSizePixels = new Vector2(LayoutSizePixels.x, newHeight);
            rectTransform.sizeDelta = TotalSizePixels;
        }

    }

}