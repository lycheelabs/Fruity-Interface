using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired when a drag operation starts.
    /// Sets FruityUI.DraggedTarget and calls MouseDragging(isFirstFrame: true).
    /// </summary>
    public class StartDragEvent : ControlEvent {
        public DragParams Params;
        
        public void Activate(bool logging) {
            // Cancel any existing drag first
            if (FruityUI.DraggedTarget != null) {
                if (logging) Debug.Log("Cancel Drag: " + FruityUI.DraggedTarget);
                FruityUI.DraggedTarget.CancelMouseDrag();
            }
            
            if (logging) Debug.Log("Start Drag: " + Params.Target);
            
            FruityUI.DraggedTarget = Params.Target;
            FruityUI.DraggedTarget.UpdateMouseDragging(true, Params);
        }
    }
}