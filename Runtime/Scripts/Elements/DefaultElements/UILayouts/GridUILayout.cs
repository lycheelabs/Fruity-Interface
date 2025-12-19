using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class GridUILayout : ContainerNode {

        public int MaxColumns = 5;
        public Vector2 GridCellSize = new Vector2(100, 100);

        protected override void Layout() {
            var numItems = ChildNodes.Count;
            var numColumns = Mathf.Max(1, Mathf.Min(numItems, MaxColumns));
            var numRows = Mathf.Max(1, Mathf.CeilToInt(numItems / (float)MaxColumns));

            for (int i = 0; i < ChildNodes.Count; i++) {
                var row = (i / MaxColumns) * MaxColumns;
                var rowStartIndex = row * MaxColumns;
                var rowEndIndex = Mathf.Min(rowStartIndex + MaxColumns - 1, numItems - 1);
                var rowColumns = rowEndIndex - rowStartIndex + 1;

                var column = i % MaxColumns;
                var xOffset = -(rowColumns - 1f) / 2f;
                var yOffset = -(numRows - 1f) / 2f;
                var position = new Vector3(xOffset + column, -(yOffset + row)) * GridCellSize; 

                var node = ChildNodes[i];
                node.rectTransform.SetAnchorAndPosition(position);
            }

            var containedSize = new Vector2(numColumns, numRows) * GridCellSize;
            LayoutSizePixels = containedSize;
            rectTransform.sizeDelta = TotalSizePixels;
        }

    }

}