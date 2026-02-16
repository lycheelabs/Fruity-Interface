using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages drag-over state hierarchy.
    /// Tracks which DragOverTargets are currently being dragged over and handles state transitions.
    /// </summary>
    public class DragOverHierarchy : MouseTargetHierarchy {

        protected override bool ShouldIncludeTarget(MouseTarget target) {
            // Only DragOverTargets participate
            return target is DragOverTarget;
        }

        protected override void CallUpdate<TParams>(MouseTarget target, bool firstFrame, TParams parameters, bool isLeaf) {
            if (target is DragOverTarget dragOverTarget && parameters is DragParams dragParams) {
                dragOverTarget.MouseDragOver(firstFrame, dragParams);
            }
        }

        protected override void CallEnd(MouseTarget target) {
            if (target is DragOverTarget dragOverTarget) {
                dragOverTarget.MouseDragOverEnd();
            }
        }

        protected override void UpdateGlobalState(MouseTarget target) {
            FruityUI.DraggedOverTarget = target as DragOverTarget;
        }

        protected override void LogTargetChange(MouseTarget target) {
            Debug.Log("DragOver: " + target);
        }
    }

}
