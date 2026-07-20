using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class GridContainerNode : ContainerNode {

        private struct SlotData {
            bool initialised;
            float animatedSlot;
        }

        // --------------------------------------------------

        public LayoutOrientation IndexDirection = LayoutOrientation.HORIZONTAL;
        public int WrapAtIndex = 5;

        public Vector2 GridCellSize = new Vector2(100, 100);

        public bool animateSlots;
        public float animateSpeed = 1f;
        private Dictionary<LayoutNode, SlotData> slotDataMap = new();

        protected override void OnChildAdded(LayoutNode newChild) {
            slotDataMap.Add(newChild, new SlotData());
        }

        protected override void OnChildRemoved(LayoutNode newChild) {
            slotDataMap.Remove(newChild);
        }

        // Called every frame
        protected override void RefreshLayout() {

            // Calculate items and grid size
            int numItems = CountPrunedItems();
            CalculateGridSize(numItems, out var gridSize, out var gridOffset);

            // Position nodes
            int placedIndex = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }
                if (!slotDataMap.ContainsKey(node)) {
                    slotDataMap[node] = new SlotData();
                }

                var cell = IndexToCell(placedIndex);
                Vector2 position = CellToPosition(cell, gridOffset);
                node.rectTransform.SetAnchorAndPosition(position);
                placedIndex++;
            }

            var containedSize = gridSize * GridCellSize;
            LayoutSizePixels = containedSize;
            rectTransform.sizeDelta = containedSize;
        }

        // -------------------------------------------------------

        private int CountPrunedItems() {
            var numItems = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node || !node.gameObject.activeSelf) { continue; }
                numItems++;
            }

            return numItems;
        }

        private void CalculateGridSize(int numItems, out Vector2Int size, out Vector2 offset) {
            if (IndexDirection == LayoutOrientation.HORIZONTAL) {
                var columns = Mathf.Min(numItems, WrapAtIndex);
                var rows = Mathf.CeilToInt(numItems / (float)WrapAtIndex);
                size = new Vector2Int(columns, rows);
            }
            else {
                var rows = Mathf.Min(numItems, WrapAtIndex);
                var columns = Mathf.CeilToInt(numItems / (float)WrapAtIndex);
                size = new Vector2Int(columns, rows);
            }
            offset = new Vector2(-(size.x - 1f) / 2f, -(size.y - 1f) / 2f);
        }

        private Vector2Int IndexToCell(int placedIndex) {
            int row, column;
            if (IndexDirection == LayoutOrientation.HORIZONTAL) {
                row = placedIndex / WrapAtIndex;
                column = placedIndex % WrapAtIndex;
            } else {
                column = placedIndex / WrapAtIndex;
                row = placedIndex % WrapAtIndex;
            }
            Vector2Int cell = new Vector2Int(column, row);
            return cell;
        }

        private Vector2 CellToPosition(Vector2Int cell, Vector2 gridOffset) {
            return new Vector3(gridOffset.x + cell.x, -(gridOffset.y + cell.y)) * GridCellSize;
        }

    }

}