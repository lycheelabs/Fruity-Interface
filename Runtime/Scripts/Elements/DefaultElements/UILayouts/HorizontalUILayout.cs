using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class HorizontalUILayout : UILayout {

        public override MouseTarget GetMouseTarget(Vector3 mouseWorldPosition) => null;

        protected override void Layout () {
            float containedWidth = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                containedWidth += node.TotalWidthPixels;
            }

            float x = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                var newWidth = node.TotalHeightPixels;
                var newPosition = new Vector3((x - containedWidth / 2f + newWidth / 2f), 0);
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