using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class GridContainerNode : ContainerNode {

        public Orientation IndexDirection;
        public int WrapAtIndex = 5;

        public Vector2 GridCellSize = new Vector2(100, 100);

        protected override void RefreshLayout() {
            if (ChildNodes.Count == 0) return;

            var numItems = ChildNodes.Count;
            int rows, columns;

            if (IndexDirection == Orientation.Horizontal) {
                columns = Mathf.Min(numItems, WrapAtIndex);
                rows = Mathf.CeilToInt(numItems / (float)WrapAtIndex);
            } else {
                rows = Mathf.Min(numItems, WrapAtIndex);
                columns = Mathf.CeilToInt(numItems / (float)WrapAtIndex);
            }

            var xOffset = -(columns - 1f) / 2f;
            var yOffset = -(rows - 1f) / 2f;

            for (int i = 0; i < ChildNodes.Count; i++) {
                int row, column;

                if (IndexDirection == Orientation.Horizontal) {
                    row = i / WrapAtIndex;
                    column = i % WrapAtIndex;
                } else {
                    column = i / WrapAtIndex;
                    row = i % WrapAtIndex;
                }

                var position = new Vector3(xOffset + column, -(yOffset + row)) * GridCellSize;

                var node = ChildNodes[i];
                node.rectTransform.SetAnchorAndPosition(position);
            }

            var containedSize = new Vector2(columns, rows) * GridCellSize;
            LayoutSizePixels = containedSize;
            rectTransform.sizeDelta = containedSize;
        }

    }

}