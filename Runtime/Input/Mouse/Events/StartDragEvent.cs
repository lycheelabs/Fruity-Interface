using UnityEngine;

namespace LycheeLabs.InputTree {

    /// <summary>
    /// Event fired when a drag operation starts.
    /// Sets Interface.DraggedTarget and calls MouseDragging(isFirstFrame: true).
    /// </summary>
    public class StartDragEvent : ControlEvent {
        public DragParams Params;
        
        public void Activate(bool logging) {
            // Cancel any existing drag first
            if (Interface.DraggedTarget != null) {
                if (logging) Debug.Log("Cancel Drag: " + Interface.DraggedTarget);
                Interface.DraggedTarget.CancelMouseDrag();
            }
            
            if (logging) Debug.Log("Start Drag: " + Params.Target);
            
            Interface.DraggedTarget = Params.Target;
            Interface.DraggedTarget.UpdateMouseDragging(true, Params);
        }
    }
}
