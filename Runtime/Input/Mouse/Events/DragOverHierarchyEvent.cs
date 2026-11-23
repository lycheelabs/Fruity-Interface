using UnityEngine;

namespace LycheeLabs.InputTree {

    /// <summary>
    /// Event fired every frame during a drag to update which DragOverTarget is being dragged over.
    /// Manages MouseDragOver and MouseDragOverEnd calls on DragOverTarget instances.
    /// </summary>
    public partial class DragOverHierarchyEvent : ControlEvent {

        private static DragOverHierarchy hierarchy = new DragOverHierarchy();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad() {
            hierarchy.ClearSilently();
        }

        internal static void ResetStateForTests() {
            ResetOnLoad();
        }

        public MouseTarget RaycastTarget;
        public InterfaceNode RaycastNode;
        public Vector3 MouseWorldPosition;
        public MouseButton DragButton;

        public void Activate(bool logging) {
            // Build hierarchy from the raycast data passed to the event
            // The hierarchy filters to only DragOverTarget instances and updates Interface.DraggedOverTarget
            hierarchy.Build(RaycastTarget, RaycastNode, MouseWorldPosition, DragButton);
            
            // ApplyDiff needs DragParams for the callbacks - build it from current global state
            var dragParams = new DragParams(
                Interface.DraggedTarget,
                Interface.DraggedOverTarget,
                Vector2.zero,  // Not used by DragOver callbacks
                Interface.RawMouseScreenPosition,
                DragButton
            );
            
            hierarchy.ApplyDiff(logging, dragParams);
        }

        /// <summary>
        /// Clear drag-over state when drag ends.
        /// </summary>
        public static void ClearState() {
            hierarchy.Clear();
        }

    }

}
