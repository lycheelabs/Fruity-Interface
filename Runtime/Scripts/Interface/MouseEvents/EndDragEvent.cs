using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired when a drag operation ends (completed or cancelled).
    /// Calls CompleteMouseDrag or CancelMouseDrag and clears FruityUI.DraggedTarget.
    /// </summary>
    public class EndDragEvent : InterfaceEvent {
        public DragParams Params;
        public bool WasCancelled;
        
        public void Activate(bool logging) {
            if (FruityUI.DraggedTarget == null) return;
            
            // Safety check for completion: params target should match
            if (!WasCancelled && Params.Target != FruityUI.DraggedTarget) {
                if (logging) Debug.Log("Drag target mismatch on complete, cancelling: " + FruityUI.DraggedTarget);
                FruityUI.DraggedTarget.CancelMouseDrag();
                FruityUI.DraggedTarget = null;
                return;
            }
            
            if (WasCancelled) {
                if (logging) Debug.Log("Cancel Drag: " + FruityUI.DraggedTarget);
                FruityUI.DraggedTarget.CancelMouseDrag();
            } else {
                if (logging) Debug.Log("Complete Drag: " + FruityUI.DraggedTarget);
                FruityUI.DraggedTarget.CompleteMouseDrag(Params);
            }
            
            FruityUI.DraggedTarget = null;
        }
    }
}