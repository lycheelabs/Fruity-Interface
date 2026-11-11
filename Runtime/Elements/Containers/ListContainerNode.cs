using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
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
            RemoveDestroyedChildren();

            float containedWidth = 0;
            float containedHeight = 0;

            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }
                containedHeight += node.TotalHeightPixels;
                containedWidth = Mathf.Max(containedWidth, node.TotalWidthPixels);
            }

            var size = new Vector2(Mathf.Max(containedWidth, minimumSize.x), Mathf.Max(containedHeight, minimumSize.y));
            LayoutSizePixels = size;
            rectTransform.sizeDelta = TotalSizePixels;
            var pivotOffset = PivotOffset(TotalSizePixels);

            float y = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }

                var newHeight = node.TotalHeightPixels;
                var shift = y - containedHeight / 2f + newHeight / 2f;
                var newPosition = new Vector3(pivotOffset.x, pivotOffset.y - shift);
                node.rectTransform.localPosition = newPosition;
                y += newHeight;
            }
        }

        private void LayoutHorizontal () {
            RemoveDestroyedChildren();

            float containedWidth = 0;
            float containedHeight = 0;

            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }
                containedWidth += node.TotalWidthPixels;
                containedHeight = Mathf.Max(containedHeight, node.TotalHeightPixels);
            }

            var size = new Vector2(Mathf.Max(containedWidth, minimumSize.x), Mathf.Max(containedHeight, minimumSize.y));
            LayoutSizePixels = size;
            rectTransform.sizeDelta = LayoutSizePixels;
            var pivotOffset = PivotOffset(LayoutSizePixels);

            float x = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }

                var newWidth = node.TotalWidthPixels;
                var shift = x - containedWidth / 2f + newWidth / 2f;
                var newPosition = new Vector3(shift + pivotOffset.x, pivotOffset.y);
                node.rectTransform.localPosition = newPosition;
                x += newWidth;
            }

        }

        private Vector2 PivotOffset (Vector2 size) {
            return (new Vector2(0.5f, 0.5f) - rectTransform.pivot) * size;
        }

        private void RemoveDestroyedChildren () {
            for (int i = ChildNodes.Count - 1; i >= 0; i--) {
                var node = ChildNodes[i];
                if (node == null) {
                    ChildNodes.RemoveAt(i);
                }
            }
        }

    }

}