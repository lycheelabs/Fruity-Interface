using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class GridContainerNode : ContainerNode {

        private struct SlotData {
            public int frameCount;
            public float animatedSlot;
            public bool isInitialised => Time.frameCount > frameCount;
        }

        // --------------------------------------------------

        public LayoutOrientation IndexDirection = LayoutOrientation.HORIZONTAL;
        public int WrapAtIndex = 5;

        public Vector2 GridCellSize = new Vector2(100, 100);

        public bool animateSlots;
        public float animateSpeed = 10f;
        private Dictionary<LayoutNode, SlotData> slotDataMap = new();

        protected override void OnChildAdded(LayoutNode newChild) {
            slotDataMap.Add(newChild, new SlotData() { 
                frameCount = Time.frameCount 
            });
        }

        protected override void OnChildRemoved(LayoutNode newChild) {
            slotDataMap.Remove(newChild);
        }

        private SlotData GetData (LayoutNode node) {
            if (!slotDataMap.ContainsKey(node)) {
                slotDataMap[node] = new SlotData() { 
                    frameCount = Time.frameCount 
                };
            }
            return slotDataMap[node];
        }

        // Called every frame
        protected override void RefreshLayout() {
            CalculateGridSize(out var gridSize, out var gridOffset);

            int placedIndex = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (ShouldPrune(node)) { 
                    continue; 
                }

                var data = GetData(node);
                if (!data.isInitialised || !animateSlots) {
                    // Initialise slot of new nodes
                    data.animatedSlot = placedIndex;
                } else {
                    // Animate slot of existing nodes
                    var delta = Time.unscaledDeltaTime * animateSpeed;
                    data.animatedSlot = data.animatedSlot.MoveTowardsDelta(placedIndex, delta);
                }

                // Position nodes (animated)
                PositionNode(node, data.animatedSlot, gridOffset);
                placedIndex++;
                slotDataMap[node] = data;
            }

            var containedSize = gridSize * GridCellSize;
            LayoutSizePixels = containedSize;
            rectTransform.sizeDelta = containedSize;
        }

        // -------------------------------------------------------

        private void CalculateGridSize(out Vector2Int size, out Vector2 offset) {
            var numItems = CountPrunedChildren();
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

        private void PositionNode(LayoutNode node, float slotIndex, Vector2 gridOffset)  {
            var cell = IndexToCell(slotIndex);
            Vector2 position = CellToPosition(cell, gridOffset);
            node.rectTransform.SetAnchorAndPosition(position);
        }

        private Vector2 IndexToCell(float placedIndex) {
            float row, column;
            if (IndexDirection == LayoutOrientation.HORIZONTAL) {
                row = (int)(placedIndex + 0.5f) / WrapAtIndex;
                column = (placedIndex + 0.5f) % WrapAtIndex - 0.5f;
            } else {
                column = (int)(placedIndex + 0.5f) / WrapAtIndex;
                row = (placedIndex + 0.5f) % WrapAtIndex - 0.5f;
            }
            return new Vector2(column, row);
        }

        private Vector2 CellToPosition(Vector2 cell, Vector2 gridOffset) {
            return new Vector3(gridOffset.x + cell.x, -(gridOffset.y + cell.y)) * GridCellSize;
        }

    }

}