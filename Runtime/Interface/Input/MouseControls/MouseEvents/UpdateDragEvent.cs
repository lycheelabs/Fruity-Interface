using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired every frame during an active drag.
    /// Calls MouseDragging(isFirstFrame: false) on the dragged target.
    /// </summary>
    public class UpdateDragEvent : ControlEvent {
        public DragParams Params;
        
        public void Activate(bool logging) {
            if (FruityUI.DraggedTarget == null) return;
            
            // Safety check: params target should match current dragged target
            if (Params.Target != FruityUI.DraggedTarget) {
                if (logging) Debug.Log("Drag target mismatch, cancelling: " + FruityUI.DraggedTarget);
                FruityUI.DraggedTarget.CancelMouseDrag();
                FruityUI.DraggedTarget = null;
                return;
            }
            
            FruityUI.DraggedTarget.UpdateMouseDragging(false, Params);
        }
    }
}