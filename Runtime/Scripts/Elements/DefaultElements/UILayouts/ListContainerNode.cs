using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class ListContainerNode : ContainerNode {

        public enum Orientations { Vertical, Horizontal }

        public Orientations Orientation;

        protected override void RefreshLayout () {
            if (Orientation == Orientations.Vertical) {
                LayoutVertical();
            } else {
                LayoutHorizontal();
            }
        }

        private void LayoutVertical () {
            float containedWidth = 0;
            float containedHeight = 0;

            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }
                containedHeight += node.TotalHeightPixels;
                containedWidth = Mathf.Max(containedWidth, node.TotalWidthPixels);
            }

            float y = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }

                var newHeight = node.TotalHeightPixels;
                var shift = y - containedHeight / 2f + newHeight / 2f;
                var newPosition = new Vector3(0, -shift);
                node.rectTransform.SetAnchorAndPosition(newPosition);
                y += newHeight;
            }

            containedWidth = Mathf.Max(containedWidth, minimumSize.x);
            containedHeight = Mathf.Max(containedHeight, minimumSize.y);

            LayoutSizePixels = new Vector2(containedWidth, containedHeight);
            rectTransform.sizeDelta = TotalSizePixels;
        }

        private void LayoutHorizontal () {
            float containedWidth = 0;
            float containedHeight = 0;

            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }
                containedWidth += node.TotalWidthPixels;
                containedHeight = Mathf.Max(containedHeight, node.TotalHeightPixels);
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

            containedWidth = Mathf.Max(containedWidth, minimumSize.x);
            containedHeight = Mathf.Max(containedHeight, minimumSize.y);

            LayoutSizePixels = new Vector2(containedWidth, containedHeight);
            rectTransform.sizeDelta = TotalSizePixels;
        }

    }

}