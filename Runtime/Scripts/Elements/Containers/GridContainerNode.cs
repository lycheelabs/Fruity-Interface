using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class GridContainerNode : ContainerNode {

        public LayoutOrientation IndexDirection = LayoutOrientation.HORIZONTAL;
        public int WrapAtIndex = 5;

        public Vector2 GridCellSize = new Vector2(100, 100);

        public void InsertChild (GameObject child, int siblingIndex) {
            child.transform.SetParent(transform, false);
            child.transform.SetSiblingIndex(siblingIndex);
        }

        protected override void RefreshLayout() {
            if (ChildNodes.Count == 0) return;

            // Calculate item count (pruned)
            var numItems = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }
                numItems++;
            }

            // Calculate rows and colums
            int rows, columns;
            if (IndexDirection == LayoutOrientation.HORIZONTAL) {
                columns = Mathf.Min(numItems, WrapAtIndex);
                rows = Mathf.CeilToInt(numItems / (float)WrapAtIndex);
            } else {
                rows = Mathf.Min(numItems, WrapAtIndex);
                columns = Mathf.CeilToInt(numItems / (float)WrapAtIndex);
            }
            var xOffset = -(columns - 1f) / 2f;
            var yOffset = -(rows - 1f) / 2f;

            // Position nodes
            int placedIndex = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }

                int row, column;

                if (IndexDirection == LayoutOrientation.HORIZONTAL) {
                    row = placedIndex / WrapAtIndex;
                    column = placedIndex % WrapAtIndex;
                } else {
                    column = placedIndex / WrapAtIndex;
                    row = placedIndex % WrapAtIndex;
                }

                var position = new Vector3(xOffset + column, -(yOffset + row)) * GridCellSize;
                node.rectTransform.SetAnchorAndPosition(position);
                placedIndex++;
            }

            var containedSize = new Vector2(columns, rows) * GridCellSize;
            LayoutSizePixels = containedSize;
            rectTransform.sizeDelta = containedSize;
        }

    }

}