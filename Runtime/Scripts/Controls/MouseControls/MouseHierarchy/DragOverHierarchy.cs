using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages drag-over state hierarchy.
    /// Tracks which DragOverTargets are currently being dragged over and handles state transitions.
    /// </summary>
    public class DragOverHierarchy : MouseTargetHierarchy {

        protected override bool ShouldIncludeTarget(MouseTarget target) {
            // Only DragOverTargets participate
            return target is DraggedOverTarget;
        }

        protected override void CallUpdate<TParams>(MouseTarget target, bool firstFrame, TParams parameters, bool isLeaf) {
            if (target is DraggedOverTarget dragOverTarget && parameters is DragParams dragParams) {
                dragOverTarget.UpdateMouseDraggedOver(firstFrame, dragParams);
            }
        }

        protected override void CallEnd(MouseTarget target) {
            if (target is DraggedOverTarget dragOverTarget) {
                dragOverTarget.EndMouseDraggedOver();
            }
        }

        protected override void UpdateGlobalState(MouseTarget target) {
            FruityUI.DraggedOverTarget = target as DraggedOverTarget;
        }

        protected override void LogTargetChange(MouseTarget target) {
            Debug.Log("DragOver: " + target);
        }
    }

}
