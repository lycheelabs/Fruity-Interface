using UnityEngine;

namespace LycheeLabs.InputTree {

    /// <summary>
    /// Event fired every frame during an active drag.
    /// Calls MouseDragging(isFirstFrame: false) on the dragged target.
    /// </summary>
    public class UpdateDragEvent : ControlEvent {
        public DragParams Params;
        
        public void Activate(bool logging) {
            if (Interface.DraggedTarget == null) return;
            
            // Safety check: params target should match current dragged target
            if (Params.Target != Interface.DraggedTarget) {
                if (logging) Debug.Log("Drag target mismatch, cancelling: " + Interface.DraggedTarget);
                Interface.DraggedTarget.CancelMouseDrag();
                Interface.DraggedTarget = null;
                return;
            }
            
            Interface.DraggedTarget.UpdateMouseDragging(false, Params);
        }
    }
}
