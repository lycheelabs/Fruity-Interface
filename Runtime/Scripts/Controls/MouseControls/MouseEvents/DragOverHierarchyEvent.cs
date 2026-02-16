namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired every frame during a drag to update which DragOverTarget is being dragged over.
    /// Manages MouseDragOver and MouseDragOverEnd calls on DragOverTarget instances.
    /// </summary>
    public class DragOverHierarchyEvent : ControlEvent {

        private static readonly DragOverHierarchy hierarchy = new DragOverHierarchy();

        public DragParams Params;

        public void Activate(bool logging) {
            // Build hierarchy from the raycast result (FruityUI.DraggedOverTarget)
            // The hierarchy filters to only DragOverTarget instances and updates FruityUI.DraggedOverDragTarget
            hierarchy.Build(FruityUI.DraggedOverTarget, null, Params.MouseWorldPosition, Params.DragButton);
            hierarchy.ApplyDiff(logging, Params);
        }

        /// <summary>
        /// Clear drag-over state when drag ends.
        /// </summary>
        public static void ClearState() {
            hierarchy.Clear();
        }

    }

}
